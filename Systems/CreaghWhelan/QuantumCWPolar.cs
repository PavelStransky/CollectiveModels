using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    public class QuantumCWPolar : CW, IQuantumSystem {
        // Planck constant
        private double hbar;                    // [Js]

        /// <summary>
        /// Planckova konstanta [Js]
        /// </summary>
        public double Hbar { get { return this.hbar; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="mu">Parametr MU</param>
        /// <param name="hbar">Planckova konstanta</param>
        public QuantumCWPolar(double a, double b, double c, double mu, double hbar, int power)
            : base(a, b, c, mu, power) {
            this.hbar = hbar;
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        protected QuantumCWPolar() { }

        #region Implementace IQuantumSystem
        // Systém s vlastními hodnotami
        protected EigenSystem eigenSystem;

        /// <summary>
        /// Systém vlastních hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Druhý invariant
        /// </summary>
        /// <param name="type">Typ Peresova operátoru</param>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        public Vector PeresInvariant(int type) {
            return null;
        }

        /// <summary>
        /// Radial part of the wave function
        /// </summary>
        /// <param name="l">Principal quantum number</param>
        /// <param name="mu">Second quantum number</param>
        /// <param name="x">Value</param>
        public double PsiR(double x, int n, int m) {
            double s = (this.eigenSystem.BasisIndex as CWPolarBasisIndex).Omega / this.hbar;

            double xi2 = x / s; xi2 *= xi2;
            m = System.Math.Abs(m);

            double normLog = 0.5 * (System.Math.Log(2.0) + SpecialFunctions.FactorialILog(n) - SpecialFunctions.FactorialILog(n + m)) - (m + 1) * System.Math.Log(s);
            double l = 0.0;
            double e = 0.0;
            SpecialFunctions.Laguerre(out l, out e, xi2, n, m);

            if(l == 0.0 || x == 0.0)
                return 0.0;

            double lLog = System.Math.Log(System.Math.Abs(l));
            double result = normLog + m * System.Math.Log(x) - xi2 / 2.0 + lLog + e;
            result = l < 0.0 ? -System.Math.Exp(result) : System.Math.Exp(result);

            return result;
        }

        /// <summary>
        /// Reálná èást vlnové funkce v souøadnicích x, y
        /// </summary>
        /// <param name="x">Souøadnice x</param>
        /// <param name="y">Souøadnice y</param>
        /// <param name="i">Index vlnové funkce</param>
        private double PsiXYRe(double x, double y, int i) {
            CWPolarBasisIndex index = this.eigenSystem.BasisIndex as CWPolarBasisIndex;
            int n = index.N[i];
            int m = index.M[i];

            double beta = System.Math.Sqrt(x * x + y * y);
            double gamma = (x > 0 ? System.Math.Atan(y / x) : System.Math.PI - System.Math.Atan(y / x));

            return this.PsiR(beta, n, m) * System.Math.Cos(m * gamma) / System.Math.Sqrt(2.0 * System.Math.PI);
        }

        /// <summary>
        /// Imaginární èást vlnové funkce v souøadnicích x, y
        /// </summary>
        /// <param name="x">Souøadnice x</param>
        /// <param name="y">Souøadnice y</param>
        /// <param name="i">Index vlnové funkce</param>
        private double PsiXYIm(double x, double y, int i) {
            CWPolarBasisIndex index = this.eigenSystem.BasisIndex as CWPolarBasisIndex;
            int n = index.N[i];
            int m = index.M[i];

            double beta = System.Math.Sqrt(x * x + y * y);
            double gamma = (x > 0 ? System.Math.Atan(y / x) : System.Math.PI - System.Math.Atan(y / x));

            return this.PsiR(beta, n, m) * System.Math.Sin(m * gamma) / System.Math.Sqrt(2.0 * System.Math.PI);
        }

        /// <summary>
        /// Vrátí matici <n|V|n> amplitudy vlastní funkce n
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="rx">Rozmìry ve smìru x</param>
        /// <param name="ry">Rozmìry ve smìru y</param>
        public virtual Matrix[] AmplitudeMatrix(int[] n, IOutputWriter writer, DiscreteInterval intx, DiscreteInterval inty) {
            int numx = intx.Num;
            int numy = inty.Num;

            int numn = n.Length;

            // Reálná a imaginární èást (proto 2 numn)
            Matrix[] result = new Matrix[2 * numn];
            for(int i = 0; i < 2 * numn; i++)
                result[i] = new Matrix(numx, numy);

            int length = this.eigenSystem.BasisIndex.Length;
            int length100 = length / 100;

            DateTime startTime = DateTime.Now;

            for(int k = 0; k < length; k++) {
                BasisCache2D cacheRe = new BasisCache2D(intx, inty, k, this.PsiXYRe);
                BasisCache2D cacheIm = new BasisCache2D(intx, inty, k, this.PsiXYIm);

                for(int l = 0; l < numn; l++) {
                    Vector ev = this.eigenSystem.GetEigenVector(n[l]);

                    for(int i = 0; i < numx; i++)
                        for(int j = 0; j < numy; j++) {
                            result[l][i, j] += ev[k] * cacheRe[i, j];
                            result[l + numn][i, j] += ev[k] * cacheIm[i, j];
                        }
                }

                if(writer != null)
                    if((k + 1) % length100 == 0) {
                        writer.Write('.');

                        if(((k + 1) / length100) % 10 == 0) {
                            writer.Write((k + 1) / length100);
                            writer.Write("% ");
                            writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));
                            startTime = DateTime.Now;
                        }
                    }
            }

            return result;
        }

        /// <summary>
        /// Vrátí matici hustot pro vlastní funkce
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="interval">Rozmìry v jednotlivých smìrech (uspoøádané ve tvaru [minx, maxx,] numx, ...)</param>
        public object ProbabilityDensity(int[] n, IOutputWriter writer, params Vector[] interval) {
            DiscreteInterval intx = new DiscreteInterval(interval[0]);
            DiscreteInterval inty = new DiscreteInterval(interval[1]);

            Matrix[] amplitude = this.AmplitudeMatrix(n, writer, intx, inty);

            int numn = amplitude.Length / 2;
            int numx = amplitude[0].LengthX;
            int numy = amplitude[0].LengthY;

            Matrix[] result = new Matrix[numn];

            for(int l = 0; l < numn; l++) {
                result[l] = new Matrix(numx, numy);

                for(int i = 0; i < numx; i++)
                    for(int j = 0; j < numy; j++)
                        result[l][i, j] = amplitude[l][i, j] * amplitude[l][i, j] + amplitude[l + numn][i, j] * amplitude[l + numn][i, j];
            }

            return result;
        }

        /// <summary>
        /// Vrátí hustotu vlnové funkce v daném bodì
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="x">Bod</param>
        public double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] x) {
            return 0;
        }

        /// <summary>
        /// Vytvoøí instanci tøídy LHOPolarIndex
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public BasisIndex CreateBasisIndex(Vector basisParams) {
            basisParams.Length++;
            basisParams.LastItem = this.Power;
            return new CWPolarBasisIndex(basisParams);
        }

        /// <summary>
        /// Napoèítá Hamiltonovu matici v dané bázi
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="basisIndex">Parametry báze</param>
        public void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            CWPolarBasisIndex index = basisIndex as CWPolarBasisIndex;
            int maxE = index.MaxE;

            double omega = index.Omega;
            double s = System.Math.Sqrt(this.hbar / omega);

            double k1 = 0.5 * s * this.A;
            double k2 = s * s * (0.5 * this.Mu - (this.Power == 2 ? 1.0 : 2.0) - index.A0);
            double k2c2 = -0.5 * s * s * (0.5 * this.Mu + (this.Power == 2 ? 1.0 : 2.0));
            double k3c1 = 1.0 / 8.0 * this.B * System.Math.Pow(s, 3);
            double k3c3 = -k3c1;

            double k4 = System.Math.Pow(s, 4) * (this.Power == 2 ? (3.0 + this.C) / 8.0 : (18.0 + this.C) / 8.0);
            double k4c2 = 0.5 * System.Math.Pow(s, 4) * (this.Power == 2 ? 0.5 : 3.0);
            double k4c4 = 0.5 * System.Math.Pow(s, 4) * (this.Power == 2 ? (1.0 - this.C) / 8.0 : (6.0 - this.C) / 8.0);

            double k6 = -5.0 / 4.0 * System.Math.Pow(s, 6);
            double k6c2 = -0.5 * 15.0 / 8.0 * System.Math.Pow(s, 6);
            double k6c4 = -0.5 * 3.0 / 4.0 * System.Math.Pow(s, 6);
            double k6c6 = -0.5 * System.Math.Pow(s, 6) / 8.0;

            double k8 = 35.0 / 128.0 * System.Math.Pow(s, 8);
            double k8c2 = 0.5 * 56.0 / 128.0 * System.Math.Pow(s, 8);
            double k8c4 = 0.5 * 28.0 / 128.0 * System.Math.Pow(s, 8);
            double k8c6 = 0.5 * 8.0 / 128.0 * System.Math.Pow(s, 8);
            double k8c8 = 0.5 / 128.0 * System.Math.Pow(s, 8);

            int length = index.Length;
            int bandWidth = index.BandWidth;

            DateTime startTime = DateTime.Now;

            if(writer != null)
                writer.Write(string.Format("Pøíprava H ({0} x {1})...", length, length));

            for(int i = 0; i < length; i++) {
                int ni = index.N[i];
                int mi = index.M[i];

                int li = System.Math.Abs(mi);

                for(int j = i; j < length; j++) {
                    int nj = index.N[j];
                    int mj = index.M[j];

                    int lj = System.Math.Abs(mj);

                    int diffn = System.Math.Abs(ni - nj);
                    double n = System.Math.Min(ni, nj);         // Musíme poèítat jako double (jinak dojde k pøeteèení)

                    int diffm = System.Math.Abs(mi - mj);
                    double l = System.Math.Min(li, lj);
                    int diffl = System.Math.Abs(li - lj);

                    int sign = li == lj ? 0 : ((li > lj && ni <= nj) || (li < lj && ni >= nj) ? -1 : 1);

                    double sum = 0.0;

                    if(this.Power == 2) {
                        if(diffm == 0) {
                            if(diffn == 0) {
                                // <nm|1|nm>
                                sum += 1;
                                // <nm|H0|nm>
                                sum += this.hbar * omega * (2.0 * n + l + 1.0);
                                // (mu/2-1-a0)<nm|r^2|nm>
                                sum += k2 * (2.0 * n + l + 1.0);
                                // 1/2<nm|r^4|nm>
                                sum += k4 * (n * (n - 1.0) + (n + l + 1.0) * (5.0 * n + l + 2.0));
                            }

                            else if(diffn == 1) {
                                // (mu/2-1-a0)<n+1,m|r^2|nm>
                                sum -= k2 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0));
                                // 1/2<n+1,m|r^4|nm>
                                sum -= 2.0 * k4 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0)) * (2.0 * n + l + 2.0);
                            }

                            else if(diffn == 2)
                                // 1/2<n+2,m|r^4|nm>
                                sum += k4 * System.Math.Sqrt((n + l + 2.0) * (n + l + 1.0) * (n + 2.0) * (n + 1.0));
                        }

                        if(diffm == 1) {
                            if(diffn == 0 && sign < 0) {
                                // A<n,m+1|r cos(fi)|nm>
                                sum += k1 * System.Math.Sqrt(n + l + 1.0);
                                // B/4<n,m+1|r^3 cos(fi)|nm>
                                sum += k3c1 * (3.0 * n + l + 2.0) * System.Math.Sqrt(n + l + 1.0);
                            }
                            else if(diffn == 1 && sign < 0) {
                                // A<n-1,m+1|r cos(fi)|nm>
                                sum -= k1 * System.Math.Sqrt(n + 1.0);
                                // B/4<n-1,m+1|r^3 cos(fi)|nm>
                                sum -= k3c1 * (3.0 * n + 2.0 * l + 4.0) * System.Math.Sqrt(n + 1.0);
                            }
                            else if(diffn == 2 && sign < 0)
                                // B/4<n-2,m+1|r^3 cos(fi)|nm>
                                sum += k3c1 * System.Math.Sqrt((n + 2.0) * (n + 1.0) * (n + l + 2));
                            else if(diffn == 1 && sign > 0)
                                // B/4<n+1,m+1|r^3 cos(fi)|nm>
                                sum -= k3c1 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0));
                        }

                        if(diffm == 2) {
                            if(diffl == 2) {
                                if(diffn == 0 && sign < 0) {
                                    // -(mu/2+1)<n,m+2|r^2 cos(2fi)|nm>
                                    sum += k2c2 * System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0));
                                    // 1/4<n,m+2|r^4 cos(2fi)|nm>
                                    sum += k4c2 * (4.0 * n + l + 3.0) * System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0));
                                }
                                else if(diffn == 1 && sign < 0) {
                                    // -(mu/2+1)<n-1,m+2|r^2 cos(2fi)|nm>
                                    sum -= 2.0 * k2c2 * System.Math.Sqrt((n + 1.0) * (n + l + 2.0));
                                    // 1/4<n-1,m+2|r^4 cos(2fi)|nm>
                                    sum -= 3.0 * k4c2 * (2.0 * n + l + 3.0) * System.Math.Sqrt((n + 1.0) * (n + l + 2.0));
                                }
                                else if(diffn == 2 && sign < 0) {
                                    // -(mu/2+1)<n-2,m+2|r^2 cos(2fi)|nm>
                                    sum += k2c2 * System.Math.Sqrt((n + 2.0) * (n + 1.0));
                                    // 1/4<n-2,m+2|r^4 cos(2fi)|nm>
                                    sum += k4c2 * (4.0 * n + 3.0 * l + 9.0) * System.Math.Sqrt((n + 2.0) * (n + 1.0));
                                }
                                else if(diffn == 3 && sign < 0) {
                                    // 1/4<n-3,m+2|r^4 cos(2fi)|nm>
                                    sum -= k4c2 * System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0) * (n + l + 3.0));
                                }
                                else if(diffn == 1 && sign > 0) {
                                    // 1/4<n+1,m+2|r^4 cos(2fi)|nm>
                                    sum -= k4c2 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                }
                            }
                            else if(diffl == 0) {
                                if(diffn == 0) {
                                    // -(mu/2+1)<n,1|r^2 cos(2fi)|n,-1>=-1/2(mu/2+1)<nm|r^2|nm>
                                    sum += k2c2 * (2.0 * n + l + 1.0);
                                    // 1/4<n,m+2|r^4 cos(2fi)|nm> = 1/8<nm|r^4|nm>
                                    sum += k4c2 * (n * (n - 1.0) + (n + l + 1.0) * (5.0 * n + l + 2.0));
                                }
                                else if(diffn == 1) {
                                    // -(mu/2+1)<n+1,1|r^2 cos(2fi)|n,-1>=-1/2(mu/2+1)<n+1,m|r^2|nm>
                                    sum -= k2c2 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0));
                                    // 1/4<n+1,m+2|r^4 cos(2fi)|nm> = 1/8<n+1,m|r^4|nm>
                                    sum -= 2.0 * k4c2 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0)) * (2.0 * n + l + 2.0);
                                }
                                else if(diffn == 2) {
                                    // 1/4<n+2,m+2|r^4 cos(2fi)|nm> = 1/8<n+2,m|r^4|nm>
                                    sum += k4c2 * System.Math.Sqrt((n + l + 2.0) * (n + l + 1.0) * (n + 2.0) * (n + 1.0));
                                }
                            }
                        }

                        if(diffm == 3) {
                            if(diffl == 3) {
                                if(diffn == 0 && sign < 0)
                                    // -B/4<n,m+3|r^3 cos(3fi)|nm>
                                    sum += k3c3 * System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                else if(diffn == 1 && sign < 0)
                                    // -B/4<n-1,m+3|r^3 cos(3fi)|nm>
                                    sum -= 3.0 * k3c3 * System.Math.Sqrt((n + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                else if(diffn == 2 && sign < 0)
                                    // -B/4<n-2,m+3|r^3 cos(3fi)|nm>
                                    sum += 3.0 * k3c3 * System.Math.Sqrt((n + 2.0) * (n + 1.0) * (n + l + 3.0));
                                else if(diffn == 3 && sign < 0)
                                    // -B/4<n-3,m+3|r^3 cos(3fi)|nm>
                                    sum -= k3c3 * System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0));
                            }
                            else if(diffl == 1) {
                                if(diffn == 0 && sign < 0)
                                    // -B/4<n,m+3|r^3 cos(3fi)|nm> = -B/4<n,m+1|r^3 cos(fi)|nm>
                                    sum += k3c3 * (3.0 * n + l + 2.0) * System.Math.Sqrt(n + l + 1.0);
                                else if(diffn == 1 && sign < 0)
                                    // -B/4<n-1,m+3|r^3 cos(3fi)|nm> = -B/4<n-1,m+1|r^3 cos(fi)|nm>
                                    sum -= k3c3 * (3.0 * n + 2.0 * l + 4.0) * System.Math.Sqrt(n + 1.0);
                                else if(diffn == 2 && sign < 0)
                                    // -B/4<n-2,m+3|r^3 cos(3fi)|nm> = -B/4<n-2,m+1|r^3 cos(fi)|nm>
                                    sum += k3c3 * System.Math.Sqrt((n + 2.0) * (n + 1.0) * (n + l + 2));
                                else if(diffn == 1 && sign > 0)
                                    // -B/4<n+1,m+3|r^3 cos(3 fi)|nm> = -B/4<n+1,m+1|r^3 cos(fi)|nm>
                                    sum -= k3c3 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0));
                            }
                        }

                        if(diffm == 4) {
                            if(diffl == 4) {
                                if(diffn == 0 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n,m+4|r^4 cos(4fi)|nm>
                                    sum += sqrt * k4c4;
                                }
                                else if(diffn == 1 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-1,m+4|r^4 cos(4fi)|nm>
                                    sum -= sqrt * 4.0 * k4c4;
                                }
                                else if(diffn == 2 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-2,m+4|r^4 cos(4fi)|nm>
                                    sum += sqrt * 6.0 * k4c4;
                                }
                                else if(diffn == 3 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 4.0));
                                    // <n-3,m+4|r^4 cos(4fi)|nm>
                                    sum -= sqrt * 4.0 * k4c4;
                                }
                                else if(diffn == 4 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0));
                                    // <n-4,m+4|r^4 cos(4fi)|nm>
                                    sum += sqrt * k4c4;
                                }
                            }
                            else if(diffl == 2) {
                                if(diffn == 0 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0));
                                    // <n,m+2|r^4 cos(4fi)|nm>
                                    sum += sqrt * k4c4
                                        * (4.0 * n + l + 3.0);
                                }
                                else if(diffn == 1 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 2.0));
                                    // <n-1,m+2|r^4 cos(4fi)|nm>
                                    sum -= sqrt * 3.0 * k4c4
                                        * (2.0 * n + l + 3.0);
                                }
                                else if(diffn == 2 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 2.0) * (n + 1.0));
                                    // <n-2,m+2|r^4 cos(4fi)|nm>
                                    sum += sqrt * k4c4
                                        * (4.0 * n + 3.0 * l + 9.0);
                                }
                                else if(diffn == 3 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0) * (n + l + 3.0));
                                    // <n-3,m+2|r^4 cos(4fi)|nm>
                                    sum -= sqrt * k4c4;

                                }
                                else if(diffn == 1 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                    // <n+1,m+2|r^4 cos(4fi)|nm>
                                    sum -= sqrt * k4c4;
                                }
                            }
                            else if(diffl == 0) {
                                if(diffn == 0) {
                                    // <nm|r^4 cos(4fi)|nm>
                                    sum += k4c4 * (n * (n - 1.0) + (n + l + 1.0) * (5.0 * n + l + 2.0));
                                }
                                else if(diffn == 1) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0));
                                    // <n+1,m|r^4 cos(4fi)|nm>
                                    sum -= sqrt * 2.0 * k4c4
                                        * (2.0 * n + l + 2.0);
                                }
                                else if(diffn == 2) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0));
                                    // <n+2,m|r^4 cos(4fi)|nm>
                                    sum += sqrt * k4c4;
                                }
                            }
                        }
                    }
                    else {
                        if(diffm == 0) {
                            if(diffn == 0) {
                                // <nm|1|nm>
                                sum += 1;
                                // <nm|H0|nm>
                                sum += this.hbar * omega * (2.0 * n + l + 1.0);
                                // <nm|r^2|nm>
                                sum += k2 * (2.0 * n + l + 1.0);
                                // <nm|r^4|nm>
                                sum += k4 * (n * (n - 1.0) + (n + l + 1.0) * (5.0 * n + l + 2.0));
                                // <nm|r^6|nm>
                                sum += k6 * (n * (n - 1.0) * (n - 2.0) 
                                    + 9.0 * n * (n - 1.0) * (n + l + 1.0) 
                                    + 9.0 * n * (n + l + 1.0) * (n + l + 2.0) 
                                    + (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                // <nm|r^8|nm>
                                sum += k8 * (n * (n - 1.0) * (n - 2.0) * (n - 3.0)
                                    + 16.0 * n * (n - 1.0) * (n - 2.0) * (n + l + 1.0)
                                    + 36 * n * (n - 1.0) * (n + l + 1.0) * (n + l + 2.0)
                                    + 16 * n * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0)
                                    + (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                            }

                            else if(diffn == 1) {
                                double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0));
                                // <n+1,m|r^2|nm>
                                sum -= sqrt * k2;
                                // <n+1,m|r^4|nm>
                                sum -= sqrt * 2.0 * k4 
                                    * (2.0 * n + l + 2.0);
                                // <n+1,m|r^6|nm>
                                sum -= sqrt * 3.0 * k6 
                                    * (n * (n - 1.0)
                                    + 3.0 * n * (n + l + 2.0)
                                    + (n + l + 2.0) * (n + l + 3.0));
                                // <n+1,m|r^6|nm>
                                sum -= sqrt * 4.0 * k8 
                                    * (n * (n - 1.0) * (n - 2.0)
                                    + 6.0 * n * (n - 1.0) * (n + l + 2.0)
                                    + 6.0 * n * (n + l + 2.0) * (n + l + 3.0)
                                    + (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                            }

                            else if(diffn == 2) {
                                double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0));
                                // <n+2,m|r^4|nm>
                                sum += sqrt * k4;
                                // <n+2,m|r^6|nm>
                                sum += sqrt * 3.0 * k6 
                                    * (2.0 * n + l + 3.0); 
                                // <n+2,m|r^8|nm>
                                sum += sqrt * 2.0 * k8
                                    * (3.0 * n * (n - 1.0)
                                    + 8.0 * n * (n + l + 3.0)
                                    + 3.0 * (n + l + 3.0) * (n + l + 4));
                            }

                            else if(diffn == 3) {
                                double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                // <n+3,m|r^6|nm>
                                sum -= sqrt * k6;
                                // <n+3,m|r^8|nm>
                                sum -= sqrt * 4.0 * k8
                                    * (2.0 * n + l + 4.0);
                            }

                            else if(diffn == 4) {
                                double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0)
                                    * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                // <n+4,m|r^8|nm>
                                sum += sqrt * k8;
                            }
                        }

                        if(diffm == 1) {
                            if(diffn == 0 && sign < 0) {
                                // <n,m+1|r cos(fi)|nm>
                                sum += k1 * System.Math.Sqrt(n + l + 1.0);
                                // <n,m+1|r^3 cos(fi)|nm>
                                sum += k3c1 * (3.0 * n + l + 2.0) * System.Math.Sqrt(n + l + 1.0);
                            }
                            else if(diffn == 1 && sign < 0) {
                                // <n-1,m+1|r cos(fi)|nm>
                                sum -= k1 * System.Math.Sqrt(n + 1.0);
                                // <n-1,m+1|r^3 cos(fi)|nm>
                                sum -= k3c1 * (3.0 * n + 2.0 * l + 4.0) * System.Math.Sqrt(n + 1.0);
                            }
                            else if(diffn == 2 && sign < 0)
                                // <n-2,m+1|r^3 cos(fi)|nm>
                                sum += k3c1 * System.Math.Sqrt((n + 2.0) * (n + 1.0) * (n + l + 2.0));
                            else if(diffn == 1 && sign > 0)
                                // <n+1,m+1|r^3 cos(fi)|nm>
                                sum -= k3c1 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0));
                        }

                        if(diffm == 2) {
                            if(diffl == 2) {
                                if(diffn == 0 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0));
                                    // <n,m+2|r^2 cos(2fi)|nm>
                                    sum += sqrt * k2c2;
                                    // <n,m+2|r^4 cos(2fi)|nm>
                                    sum += sqrt * k4c2
                                        * (4.0 * n + l + 3.0);
                                    // <n,m+2|r^6 cos(2fi)|nm>
                                    sum += sqrt * k6c2
                                        * (6.0 * n * (n - 1.0)
                                        + 8.0 * n * (n + l + 3.0)
                                        + (n + l + 3.0) * (n + l + 4.0));
                                    // <n,m+2|r^8 cos(2fi)|nm>
                                    sum += sqrt * k8c2
                                        * (10.0 * n * (n - 1.0) * (n - 2.0)
                                        + 30.0 * n * (n - 1.0) * (n + l + 3.0)
                                        + 15.0 * n * (n + l + 3.0) * (n + l + 4.0)
                                        + (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 1 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 2.0));
                                    // <n-1,m+2|r^2 cos(2fi)|nm>
                                    sum -= sqrt * 2.0 * k2c2;
                                    // <n-1,m+2|r^4 cos(2fi)|nm>
                                    sum -= sqrt * 3.0 * k4c2
                                        * (2.0 * n + l + 3.0);
                                    // <n-1,m+2|r^6 cos(2fi)|nm>
                                    sum -= sqrt * 4.0 * k6c2
                                        * (n * (n - 1.0)
                                        + 3.0 * n * (n + l + 3.0)
                                        + (n + l + 3.0) * (n + l + 4.0));
                                    // <n-1,m+2|r^8 cos(2fi)|nm>
                                    sum -= sqrt * 5.0 * k8c2
                                        * (n * (n - 1.0) * (n - 2.0)
                                        + 6.0 * n * (n - 1.0) * (n + l + 3.0)
                                        + 6.0 * n * (n + l + 3.0) * (n + l + 4.0)
                                        + (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 2 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 2.0) * (n + 1.0));
                                    // <n-2,m+2|r^2 cos(2fi)|nm>
                                    sum += sqrt * k2c2;
                                    // <n-2,m+2|r^4 cos(2fi)|nm>
                                    sum += sqrt * k4c2
                                        * (4.0 * n + 3.0 * l + 9.0);
                                    // <n-2,m+2|r^6 cos(2fi)|nm>
                                    sum += sqrt * k6c2
                                        * (n * (n - 1.0)
                                        + 8.0 * n * (n + l + 3.0)
                                        + 6.0 * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-2,m+2|r^8 cos(2fi)|nm>
                                    sum += sqrt * k8c2
                                        * (n * (n - 1.0) * (n - 2.0)
                                        + 15.0 * n * (n - 1.0) * (n + l + 3.0)
                                        + 30.0 * n * (n + l + 3.0) * (n + l + 4.0)
                                        + 10.0 * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 3 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0) * (n + l + 3.0));
                                    // <n-3,m+2|r^4 cos(2fi)|nm>
                                    sum -= sqrt * k4c2;
                                    // <n-3,m+2|r^6 cos(2fi)|nm>
                                    sum -= sqrt * 2.0 * k6c2
                                        * (3.0 * n + 2.0 * l + 8.0);
                                    // <n-3,m+2|r^8 cos(2fi)|nm>
                                    sum -= sqrt * k8c2
                                        * (3 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 4.0)
                                        + 10.0 * (n + l + 4.0) * (n + l + 5.0));

                                }
                                else if(diffn == 4 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-4,m+2|r^6 cos(2fi)|nm>
                                    sum += sqrt * k6c2;
                                    // <n-4,m+2|r^8 cos(2fi)|nm>
                                    sum += sqrt * k8c2
                                        * (8.0 * n + 5.0 * l + 25.0);
                                }
                                else if(diffn == 5 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                    // <n-4,m+2|r^8 cos(2fi)|nm>
                                    sum -= sqrt * k8c2;
                                }
                                else if(diffn == 1 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                    // <n+1,m+2|r^4 cos(2fi)|nm>
                                    sum -= sqrt * k4c2;
                                    // <n+1,m+2|r^6 cos(2fi)|nm>
                                    sum -= sqrt * 2.0 * k6c2
                                        * (3.0 * n + l + 4.0);
                                    // <n+1,m+2|r^8 cos(2fi)|nm>
                                    sum -= sqrt * k8c2
                                        * (10.0 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 4.0)
                                        + 3.0 * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 2 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n+2,m+2|r^6 cos(2fi)|nm>
                                    sum += sqrt * k6c2;
                                    // <n+2,m+2|r^8 cos(2fi)|nm>
                                    sum += sqrt * k8c2
                                        * (8.0 * n + 3.0 * l + 15.0);
                                }
                                else if(diffn == 3 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                    // <n+2,m+2|r^8 cos(2fi)|nm>
                                    sum -= sqrt * k8c2;
                                }
                            }
                            else if(diffl == 0) {
                                if(diffn == 0) {
                                    // <nm|r^2 cos(2fi)|nm>
                                    sum += k2c2 * (2.0 * n + l + 1.0);
                                    // <nm|r^4 cos(2fi)|nm>
                                    sum += k4c2 * (n * (n - 1.0) + (n + l + 1.0) * (5.0 * n + l + 2.0));
                                    // <nm|r^6 cos(2fi)|nm>
                                    sum += k6c2 * (n * (n - 1.0) * (n - 2.0)
                                        + 9.0 * n * (n - 1.0) * (n + l + 1.0)
                                        + 9.0 * n * (n + l + 1.0) * (n + l + 2.0)
                                        + (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                    // <nm|r^8 cos(2fi)|nm>
                                    sum += k8c2 * (n * (n - 1.0) * (n - 2.0) * (n - 3.0)
                                        + 16.0 * n * (n - 1.0) * (n - 2.0) * (n + l + 1.0)
                                        + 36 * n * (n - 1.0) * (n + l + 1.0) * (n + l + 2.0)
                                        + 16 * n * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0)
                                        + (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
 
                                }
                                else if(diffn == 1) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0));
                                    // <n+1,m|r^2 cos(2fi)|nm>
                                    sum -= sqrt * k2c2;
                                    // <n+1,m|r^4 cos(2fi)|nm>
                                    sum -= sqrt * 2.0 * k4c2
                                        * (2.0 * n + l + 2.0);
                                    // <n+1,m|r^6 cos(2fi)|nm>
                                    sum -= sqrt * 3.0 * k6c2
                                        * (n * (n - 1.0)
                                        + 3.0 * n * (n + l + 2.0)
                                        + (n + l + 2.0) * (n + l + 3.0));
                                    // <n+1,m|r^6 cos(2fi)|nm>
                                    sum -= sqrt * 4.0 * k8c2
                                        * (n * (n - 1.0) * (n - 2.0)
                                        + 6.0 * n * (n - 1.0) * (n + l + 2.0)
                                        + 6.0 * n * (n + l + 2.0) * (n + l + 3.0)
                                        + (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                }
                                else if(diffn == 2) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0));
                                    // <n+2,m|r^4 cos(2fi)|nm>
                                    sum += sqrt * k4c2;
                                    // <n+2,m|r^6 cos(2fi)|nm>
                                    sum += sqrt * 3.0 * k6c2
                                        * (2.0 * n + l + 3.0);
                                    // <n+2,m|r^8 cos(2fi)|nm>
                                    sum += sqrt * 2.0 * k8c2
                                        * (3.0 * n * (n - 1.0)
                                        + 8.0 * n * (n + l + 3.0)
                                        + 3.0 * (n + l + 3.0) * (n + l + 4));
                                }

                                else if(diffn == 3) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                    // <n+3,m|r^6  cos(2fi)|nm>
                                    sum -= sqrt * k6c2;
                                    // <n+3,m|r^8  cos(2fi)|nm>
                                    sum -= sqrt * 4.0 * k8c2
                                        * (2.0 * n + l + 4.0);
                                }

                                else if(diffn == 4) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0)
                                        * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n+4,m|r^8  cos(2fi)|nm>
                                    sum += sqrt * k8c2;
                                }
                            }
                        }

                        if(diffm == 3) {
                            if(diffl == 3) {
                                if(diffn == 0 && sign < 0)
                                    // <n,m+3|r^3 cos(3fi)|nm>
                                    sum += k3c3 * System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                else if(diffn == 1 && sign < 0)
                                    // <n-1,m+3|r^3 cos(3fi)|nm>
                                    sum -= 3.0 * k3c3 * System.Math.Sqrt((n + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                else if(diffn == 2 && sign < 0)
                                    // <n-2,m+3|r^3 cos(3fi)|nm>
                                    sum += 3.0 * k3c3 * System.Math.Sqrt((n + 2.0) * (n + 1.0) * (n + l + 3.0));
                                else if(diffn == 3 && sign < 0)
                                    // <n-3,m+3|r^3 cos(3fi)|nm>
                                    sum -= k3c3 * System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0));
                            }
                            else if(diffl == 1) {
                                if(diffn == 0 && sign < 0)
                                    // <n,m+1|r^3 cos(3fi)|nm>
                                    sum += k3c3 * (3.0 * n + l + 2.0) * System.Math.Sqrt(n + l + 1.0);
                                else if(diffn == 1 && sign < 0)
                                    // <n-1,m+1|r^3 cos(3fi)|nm>
                                    sum -= k3c3 * (3.0 * n + 2.0 * l + 4.0) * System.Math.Sqrt(n + 1.0);
                                else if(diffn == 2 && sign < 0)
                                    // <n-2,m+1|r^3 cos(3fi)|nm>
                                    sum += k3c3 * System.Math.Sqrt((n + 2.0) * (n + 1.0) * (n + l + 2));
                                else if(diffn == 1 && sign > 0)
                                    // <n+1,m+3|r^3 cos(3fi)|nm>
                                    sum -= k3c3 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0));
                            }
                        }

                        if(diffm == 4) {
                            if(diffl == 4) {
                                if(diffn == 0 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n,m+4|r^4 cos(4fi)|nm>
                                    sum += sqrt * k4c4;
                                    // <n,m+4|r^6 cos(4fi)|nm>
                                    sum += sqrt * k6c4
                                        * (6.0 * n + l + 5.0);
                                    // <n,m+4|r^8 cos(4fi)|nm>
                                    sum += sqrt * k8c4
                                        * (15.0 * n * (n - 1.0)
                                        + 12.0 * n * (n + l + 5.0)
                                        + (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 1 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-1,m+4|r^4 cos(4fi)|nm>
                                    sum -= sqrt * 4.0 * k4c4;
                                    // <n-1,m+4|r^6 cos(4fi)|nm>
                                    sum -= sqrt * 5.0 * k6c4
                                        * (3.0 * n + l + 5.0);
                                    // <n-1,m+4|r^8 cos(4fi)|nm>
                                    sum -= sqrt * 2.0 * k8c4
                                        * (10.0 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 5.0)
                                        + 3.0 * (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 2 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-2,m+4|r^4 cos(4fi)|nm>
                                    sum += sqrt * 6.0 * k4c4;
                                    // <n-2,m+4|r^6 cos(4fi)|nm>
                                    sum += sqrt * 10.0 * k6c4
                                        * (2.0 * n + l + 5.0);
                                    // <n-2,m+4|r^8 cos(4fi)|nm>
                                    sum += sqrt * 5.0 * k8c4
                                        * (3.0 * n * (n - 1.0)
                                        + 8.0 * n * (n + l + 5.0)
                                        + 3.0 * (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 3 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 4.0));
                                    // <n-3,m+4|r^4 cos(4fi)|nm>
                                    sum -= sqrt * 4.0 * k4c4;
                                    // <n-3,m+4|r^6 cos(4fi)|nm>
                                    sum -= sqrt * 5.0 * k6c4 
                                        * (3.0 * n + 2.0 * l + 10.0);
                                    // <n-3,m+4|r^8 cos(4fi)|nm>
                                    sum -= sqrt * 2.0 * k8c4
                                        * (3.0 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 5.0)
                                        + 10.0 * (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 4 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0));
                                    // <n-4,m+4|r^4 cos(4fi)|nm>
                                    sum += sqrt * k4c4;
                                    // <n-4,m+4|r^6 cos(4fi)|nm>
                                    sum += sqrt * k6c4
                                        * (6.0 * n + 5.0 * l + 25.0);
                                    // <n-4,m+4|r^8 cos(4fi)|nm>
                                    sum += sqrt * k8c4
                                        * (n * (n - 1.0)
                                        + 12.0 * n * (n + l + 5.0)
                                        + 15.0 * (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 5 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + l + 5.0));
                                    // <n-5,m+4|r^6 cos(4fi)|nm>
                                    sum -= sqrt * k6c4;
                                    // <n-5,m+4|r^8 cos(4fi)|nm>
                                    sum -= sqrt * 2.0 * k8c4
                                        * (4.0 * n + 3.0 * l + 18.0);
                                }
                                else if(diffn == 6 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + 6.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n-6,m+4|r^8 cos(4fi)|nm>
                                    sum += sqrt * k8c4;
                                }
                                else if(diffn == 1 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                    // <n+1,m+4|r^6 cos(4fi)|nm>
                                    sum -= sqrt * k6c4;
                                    // <n+1,m+4|r^8 cos(4fi)|nm>
                                    sum -= sqrt * 2.0 * k8c4
                                        * (4.0 * n + l + 6.0);
                                }
                                else if(diffn == 2 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n+2,m+4|r^8 cos(4fi)|nm>
                                    sum += sqrt * k8c4;
                                }
                            }
                            else if(diffl == 2) {
                                if(diffn == 0 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0));
                                    // <n,m+2|r^4 cos(4fi)|nm>
                                    sum += sqrt * k4c4
                                        * (4.0 * n + l + 3.0);
                                    // <n,m+2|r^6 cos(4fi)|nm>
                                    sum += sqrt * k6c4
                                        * (6.0 * n * (n - 1.0)
                                        + 8.0 * n * (n + l + 3.0)
                                        + (n + l + 3.0) * (n + l + 4.0));
                                    // <n,m+2|r^8 cos(4fi)|nm>
                                    sum += sqrt * k8c4
                                        * (10.0 * n * (n - 1.0) * (n - 2.0)
                                        + 30.0 * n * (n - 1.0) * (n + l + 3.0)
                                        + 15.0 * n * (n + l + 3.0) * (n + l + 4.0)
                                        + (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 1 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 2.0));
                                    // <n-1,m+2|r^4 cos(4fi)|nm>
                                    sum -= sqrt * 3.0 * k4c4
                                        * (2.0 * n + l + 3.0);
                                    // <n-1,m+2|r^6 cos(4fi)|nm>
                                    sum -= sqrt * 4.0 * k6c4
                                        * (n * (n - 1.0)
                                        + 3.0 * n * (n + l + 3.0)
                                        + (n + l + 3.0) * (n + l + 4.0));
                                    // <n-1,m+2|r^8 cos(4fi)|nm>
                                    sum -= sqrt * 5.0 * k8c4
                                        * (n * (n - 1.0) * (n - 2.0)
                                        + 6.0 * n * (n - 1.0) * (n + l + 3.0)
                                        + 6.0 * n * (n + l + 3.0) * (n + l + 4.0)
                                        + (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 2 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 2.0) * (n + 1.0));
                                    // <n-2,m+2|r^4 cos(4fi)|nm>
                                    sum += sqrt * k4c4
                                        * (4.0 * n + 3.0 * l + 9.0);
                                    // <n-2,m+2|r^6 cos(4fi)|nm>
                                    sum += sqrt * k6c4
                                        * (n * (n - 1.0)
                                        + 8.0 * n * (n + l + 3.0)
                                        + 6.0 * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-2,m+2|r^8 cos(4fi)|nm>
                                    sum += sqrt * k8c4
                                        * (n * (n - 1.0) * (n - 2.0)
                                        + 15.0 * n * (n - 1.0) * (n + l + 3.0)
                                        + 30.0 * n * (n + l + 3.0) * (n + l + 4.0)
                                        + 10.0 * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 3 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0) * (n + l + 3.0));
                                    // <n-3,m+2|r^4 cos(4fi)|nm>
                                    sum -= sqrt * k4c4;
                                    // <n-3,m+2|r^6 cos(4fi)|nm>
                                    sum -= sqrt * 2.0 * k6c4
                                        * (3.0 * n + 2.0 * l + 8.0);
                                    // <n-3,m+2|r^8 cos(4fi)|nm>
                                    sum -= sqrt * k8c4
                                        * (3 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 4.0)
                                        + 10.0 * (n + l + 4.0) * (n + l + 5.0));

                                }
                                else if(diffn == 4 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-4,m+2|r^6 cos(4fi)|nm>
                                    sum += sqrt * k6c4;
                                    // <n-4,m+2|r^8 cos(4fi)|nm>
                                    sum += sqrt * k8c4
                                        * (8.0 * n + 5.0 * l + 25.0);
                                }
                                else if(diffn == 5 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                    // <n-4,m+2|r^8 cos(4fi)|nm>
                                    sum -= sqrt * k8c4;
                                }
                                else if(diffn == 1 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                    // <n+1,m+2|r^4 cos(4fi)|nm>
                                    sum -= sqrt * k4c4;
                                    // <n+1,m+2|r^6 cos(4fi)|nm>
                                    sum -= sqrt * 2.0 * k6c4
                                        * (3.0 * n + l + 4.0);
                                    // <n+1,m+2|r^8 cos(4fi)|nm>
                                    sum -= sqrt * k8c4
                                        * (10.0 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 4.0)
                                        + 3.0 * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 2 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n+2,m+2|r^6 cos(4fi)|nm>
                                    sum += sqrt * k6c4;
                                    // <n+2,m+2|r^8 cos(4fi)|nm>
                                    sum += sqrt * k8c4
                                        * (8.0 * n + 3.0 * l + 15.0);
                                }
                                else if(diffn == 3 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                    // <n+2,m+2|r^8 cos(4fi)|nm>
                                    sum -= sqrt * k8c4;
                                }
                            }
                            else if(diffl == 0) {
                                if(diffn == 0) {
                                    // <nm|r^4 cos(4fi)|nm>
                                    sum += k4c4 * (n * (n - 1.0) + (n + l + 1.0) * (5.0 * n + l + 2.0));
                                    // <nm|r^6 cos(4fi)|nm>
                                    sum += k6c4 * (n * (n - 1.0) * (n - 2.0)
                                        + 9.0 * n * (n - 1.0) * (n + l + 1.0)
                                        + 9.0 * n * (n + l + 1.0) * (n + l + 2.0)
                                        + (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                    // <nm|r^8 cos(4fi)|nm>
                                    sum += k8c4 * (n * (n - 1.0) * (n - 2.0) * (n - 3.0)
                                        + 16.0 * n * (n - 1.0) * (n - 2.0) * (n + l + 1.0)
                                        + 36 * n * (n - 1.0) * (n + l + 1.0) * (n + l + 2.0)
                                        + 16 * n * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0)
                                        + (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));

                                }
                                else if(diffn == 1) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0));
                                    // <n+1,m|r^4 cos(4fi)|nm>
                                    sum -= sqrt * 2.0 * k4c4
                                        * (2.0 * n + l + 2.0);
                                    // <n+1,m|r^6 cos(4fi)|nm>
                                    sum -= sqrt * 3.0 * k6c4
                                        * (n * (n - 1.0)
                                        + 3.0 * n * (n + l + 2.0)
                                        + (n + l + 2.0) * (n + l + 3.0));
                                    // <n+1,m|r^6 cos(4fi)|nm>
                                    sum -= sqrt * 4.0 * k8c4
                                        * (n * (n - 1.0) * (n - 2.0)
                                        + 6.0 * n * (n - 1.0) * (n + l + 2.0)
                                        + 6.0 * n * (n + l + 2.0) * (n + l + 3.0)
                                        + (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                }
                                else if(diffn == 2) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0));
                                    // <n+2,m|r^4 cos(4fi)|nm>
                                    sum += sqrt * k4c4;
                                    // <n+2,m|r^6 cos(4fi)|nm>
                                    sum += sqrt * 3.0 * k6c4
                                        * (2.0 * n + l + 3.0);
                                    // <n+2,m|r^8 cos(4fi)|nm>
                                    sum += sqrt * 2.0 * k8c4
                                        * (3.0 * n * (n - 1.0)
                                        + 8.0 * n * (n + l + 3.0)
                                        + 3.0 * (n + l + 3.0) * (n + l + 4));
                                }

                                else if(diffn == 3) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                    // <n+3,m|r^6  cos(4fi)|nm>
                                    sum -= sqrt * k6c4;
                                    // <n+3,m|r^8  cos(4fi)|nm>
                                    sum -= sqrt * 4.0 * k8c4
                                        * (2.0 * n + l + 4.0);
                                }

                                else if(diffn == 4) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0)
                                        * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n+4,m|r^8 cos(4fi)|nm>
                                    sum += sqrt * k8c4;
                                }
                            }
                        }

                        if(diffm == 6) {
                            if(diffl == 6) {
                                if(diffn == 0 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n,m+6|r^6 cos(6fi)|nm>
                                    sum += sqrt * k6c6;
                                    // <n,m+6|r^8 cos(6fi)|nm>
                                    sum += sqrt * k8c6
                                        * (8.0 * n + l + 7.0);
                                }
                                else if(diffn == 1 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n-1,m+6|r^6 cos(6fi)|nm>
                                    sum -= sqrt * 6.0 * k6c6;
                                    // <n-1,m+6|r^8 cos(6fi)|nm>
                                    sum -= sqrt * 7.0 * k8c6
                                        * (4.0 * n + l + 7.0);
                                }
                                else if(diffn == 2 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n-2,m+6|r^6 cos(6fi)|nm>
                                    sum += sqrt * 15.0 * k6c6;
                                    // <n-2,m+6|r^8 cos(6fi)|nm>
                                    sum += sqrt * 7.0 * k8c6
                                        * (8.0 * n + 3.0 * l + 21.0);
                                }
                                else if(diffn == 3 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n-3,m+6|r^6 cos(6fi)|nm>
                                    sum -= sqrt * 20.0 * k6c6;
                                    // <n-3,m+6|r^8 cos(6fi)|nm>
                                    sum -= sqrt * 35.0 * k8c6
                                        * (2.0 * n + l + 7.0);
                                }
                                else if(diffn == 4 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n-4,m+6|r^6 cos(6fi)|nm>
                                    sum += sqrt * 15.0 * k6c6;
                                    // <n-4,m+6|r^8 cos(6fi)|nm>
                                    sum += sqrt * 7.0 * k8c6
                                        * (8.0 * n + 5.0 * l + 35.0);
                                }
                                else if(diffn == 5 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + l + 6.0));
                                    // <n-5,m+6|r^6 cos(6fi)|nm>
                                    sum -= sqrt * 6.0 * k6c6;
                                    // <n-5,m+6|r^8 cos(6fi)|nm>
                                    sum -= sqrt * 7.0 * k8c6
                                        * (4.0 * n + 3.0 * l + 21.0);
                                }
                                else if(diffn == 6 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + 6.0));
                                    // <n-6,m+6|r^6 cos(6fi)|nm>
                                    sum += sqrt * k6c6;
                                    // <n-6,m+6|r^8 cos(6fi)|nm>
                                    sum += sqrt * k8c6
                                        * (8.0 * n + 7.0 * l + 49.0);
                                }
                                else if(diffn == 7 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + 6.0) * (n + 7.0) * (n + l + 7.0));
                                    // <n-7,m+6|r^8 cos(6fi)|nm>
                                    sum -= sqrt * k8c6;
                                }
                                else if(diffn == 1 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0) * (n + l + 7.0));
                                    // <n+1,m+6|r^6 cos(6fi)|nm>
                                    sum -= sqrt * k8c6;
                                }
                            }
                            if(diffl == 4) {
                                if(diffn == 0 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n,m+4|r^6 cos(6fi)|nm>
                                    sum += sqrt * k6c6
                                        * (6.0 * n + l + 5.0);
                                    // <n,m+4|r^8 cos(6fi)|nm>
                                    sum += sqrt * k8c6
                                        * (15.0 * n * (n - 1.0)
                                        + 12.0 * n * (n + l + 5.0)
                                        + (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 1 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-1,m+4|r^6 cos(6fi)|nm>
                                    sum -= sqrt * 5.0 * k6c6
                                        * (3.0 * n + l + 5.0);
                                    // <n-1,m+4|r^8 cos(6fi)|nm>
                                    sum -= sqrt * 2.0 * k8c6
                                        * (10.0 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 5.0)
                                        + 3.0 * (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 2 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-2,m+4|r^6 cos(6fi)|nm>
                                    sum += sqrt * 10.0 * k6c6
                                        * (2.0 * n + l + 5.0);
                                    // <n-2,m+4|r^8 cos(6fi)|nm>
                                    sum += sqrt * 5.0 * k8c6
                                        * (3.0 * n * (n - 1.0)
                                        + 8.0 * n * (n + l + 5.0)
                                        + 3.0 * (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 3 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 4.0));
                                    // <n-3,m+4|r^6 cos(6fi)|nm>
                                    sum -= sqrt * 5.0 * k6c6
                                        * (3.0 * n + 2.0 * l + 10.0);
                                    // <n-3,m+4|r^8 cos(6fi)|nm>
                                    sum -= sqrt * 2.0 * k8c6
                                        * (3.0 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 5.0)
                                        + 10.0 * (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 4 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0));
                                    // <n-4,m+4|r^6 cos(6fi)|nm>
                                    sum += sqrt * k6c6
                                        * (6.0 * n + 5.0 * l + 25.0);
                                    // <n-4,m+4|r^8 cos(6fi)|nm>
                                    sum += sqrt * k8c6
                                        * (n * (n - 1.0)
                                        + 12.0 * n * (n + l + 5.0)
                                        + 15.0 * (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 5 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + l + 5.0));
                                    // <n-5,m+4|r^6 cos(6fi)|nm>
                                    sum -= sqrt * k6c6;
                                    // <n-5,m+4|r^8 cos(6fi)|nm>
                                    sum -= sqrt * 2.0 * k8c6
                                        * (4.0 * n + 3.0 * l + 18.0);
                                }
                                else if(diffn == 6 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + 6.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n-6,m+4|r^8 cos(6fi)|nm>
                                    sum += sqrt * k8c6;
                                }
                                else if(diffn == 1 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                    // <n+1,m+4|r^6 cos(6fi)|nm>
                                    sum -= sqrt * k6c6;
                                    // <n+1,m+4|r^8 cos(6fi)|nm>
                                    sum -= sqrt * 2.0 * k8c6
                                        * (4.0 * n + l + 6.0);
                                }
                                else if(diffn == 2 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n+2,m+4|r^8 cos(6fi)|nm>
                                    sum += sqrt * k8c6;
                                }
                            }
                            else if(diffl == 2) {
                                if(diffn == 0 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0));
                                    // <n,m+2|r^6 cos(6fi)|nm>
                                    sum += sqrt * k6c6
                                        * (6.0 * n * (n - 1.0)
                                        + 8.0 * n * (n + l + 3.0)
                                        + (n + l + 3.0) * (n + l + 4.0));
                                    // <n,m+2|r^8 cos(6fi)|nm>
                                    sum += sqrt * k8c6
                                        * (10.0 * n * (n - 1.0) * (n - 2.0)
                                        + 30.0 * n * (n - 1.0) * (n + l + 3.0)
                                        + 15.0 * n * (n + l + 3.0) * (n + l + 4.0)
                                        + (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 1 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 2.0));
                                    // <n-1,m+2|r^6 cos(6fi)|nm>
                                    sum -= sqrt * 4.0 * k6c6
                                        * (n * (n - 1.0)
                                        + 3.0 * n * (n + l + 3.0)
                                        + (n + l + 3.0) * (n + l + 4.0));
                                    // <n-1,m+2|r^8 cos(6fi)|nm>
                                    sum -= sqrt * 5.0 * k8c6
                                        * (n * (n - 1.0) * (n - 2.0)
                                        + 6.0 * n * (n - 1.0) * (n + l + 3.0)
                                        + 6.0 * n * (n + l + 3.0) * (n + l + 4.0)
                                        + (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 2 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 2.0) * (n + 1.0));
                                    // <n-2,m+2|r^6 cos(6fi)|nm>
                                    sum += sqrt * k6c6
                                        * (n * (n - 1.0)
                                        + 8.0 * n * (n + l + 3.0)
                                        + 6.0 * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-2,m+2|r^8 cos(6fi)|nm>
                                    sum += sqrt * k8c6
                                        * (n * (n - 1.0) * (n - 2.0)
                                        + 15.0 * n * (n - 1.0) * (n + l + 3.0)
                                        + 30.0 * n * (n + l + 3.0) * (n + l + 4.0)
                                        + 10.0 * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 3 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0) * (n + l + 3.0));
                                    // <n-3,m+2|r^6 cos(6fi)|nm>
                                    sum -= sqrt * 2.0 * k6c6
                                        * (3.0 * n + 2.0 * l + 8.0);
                                    // <n-3,m+2|r^8 cos(6fi)|nm>
                                    sum -= sqrt * k8c6
                                        * (3 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 4.0)
                                        + 10.0 * (n + l + 4.0) * (n + l + 5.0));

                                }
                                else if(diffn == 4 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-4,m+2|r^6 cos(6fi)|nm>
                                    sum += sqrt * k6c6;
                                    // <n-4,m+2|r^8 cos(6fi)|nm>
                                    sum += sqrt * k8c6
                                        * (8.0 * n + 5.0 * l + 25.0);
                                }
                                else if(diffn == 5 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                    // <n-4,m+2|r^8 cos(6fi)|nm>
                                    sum -= sqrt * k8c6;
                                }
                                else if(diffn == 1 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                    // <n+1,m+2|r^6 cos(6fi)|nm>
                                    sum -= sqrt * 2.0 * k6c6
                                        * (3.0 * n + l + 4.0);
                                    // <n+1,m+2|r^8 cos(6fi)|nm>
                                    sum -= sqrt * k8c6
                                        * (10.0 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 4.0)
                                        + 3.0 * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 2 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n+2,m+2|r^6 cos(6fi)|nm>
                                    sum += sqrt * k6c6;
                                    // <n+2,m+2|r^8 cos(6fi)|nm>
                                    sum += sqrt  * k8c6
                                        * (8.0 * n + 3.0 * l + 15.0);
                                }
                                else if(diffn == 3 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                    // <n+2,m+2|r^8 cos(6fi)|nm>
                                    sum -= sqrt * k8c6;
                                }
                            }
                            else if(diffl == 0) {
                                if(diffn == 0) {
                                    // <nm|r^6 cos(6fi)|nm>
                                    sum += k6c6 * (n * (n - 1.0) * (n - 2.0)
                                        + 9.0 * n * (n - 1.0) * (n + l + 1.0)
                                        + 9.0 * n * (n + l + 1.0) * (n + l + 2.0)
                                        + (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                    // <nm|r^8 cos(6fi)|nm>
                                    sum += k8c6 * (n * (n - 1.0) * (n - 2.0) * (n - 3.0)
                                        + 16.0 * n * (n - 1.0) * (n - 2.0) * (n + l + 1.0)
                                        + 36 * n * (n - 1.0) * (n + l + 1.0) * (n + l + 2.0)
                                        + 16 * n * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0)
                                        + (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));

                                }
                                else if(diffn == 1) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0));
                                    // <n+1,m|r^6 cos(6fi)|nm>
                                    sum -= sqrt * 3.0 * k6c6
                                        * (n * (n - 1.0)
                                        + 3.0 * n * (n + l + 2.0)
                                        + (n + l + 2.0) * (n + l + 3.0));
                                    // <n+1,m|r^6 cos(6fi)|nm>
                                    sum -= sqrt * 4.0 * k8c6
                                        * (n * (n - 1.0) * (n - 2.0)
                                        + 6.0 * n * (n - 1.0) * (n + l + 2.0)
                                        + 6.0 * n * (n + l + 2.0) * (n + l + 3.0)
                                        + (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                }
                                else if(diffn == 2) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0));
                                    // <n+2,m|r^6 cos(6fi)|nm>
                                    sum += sqrt * 3.0 * k6c6
                                        * (2.0 * n + l + 3.0);
                                    // <n+2,m|r^8 cos(6fi)|nm>
                                    sum += sqrt * 2.0 * k8c6
                                        * (3.0 * n * (n - 1.0)
                                        + 8.0 * n * (n + l + 3.0)
                                        + 3.0 * (n + l + 3.0) * (n + l + 4));
                                }

                                else if(diffn == 3) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                    // <n+3,m|r^6  cos(6fi)|nm>
                                    sum -= sqrt * k6c6;
                                    // <n+3,m|r^8  cos(6fi)|nm>
                                    sum -= sqrt * 4.0 * k8c6
                                        * (2.0 * n + l + 4.0);
                                }

                                else if(diffn == 4) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0)
                                        * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n+4,m|r^8 cos(6fi)|nm>
                                    sum += sqrt * k8c6;
                                }
                            }
                        }

                        if(diffm == 8) {
                            if(diffl == 8) {
                                if(diffn == 0 && sign < 0) {
                                    // <n,m+8|r^8 cos(8fi)|nm>
                                    sum += k8c8 * System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0) * (n + l + 7.0) * (n + l + 8.0));
                                }
                                else if(diffn == 1 && sign < 0) {
                                    // <n-1,m+8|r^8 cos(8fi)|nm>
                                    sum -= 8.0 * k8c8 * System.Math.Sqrt((n + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0) * (n + l + 7.0) * (n + l + 8.0));
                                }
                                else if(diffn == 2 && sign < 0) {
                                    // <n-2,m+8|r^8 cos(8fi)|nm>
                                    sum += 28.0 * k8c8 * System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0) * (n + l + 7.0) * (n + l + 8.0));
                                }
                                else if(diffn == 3 && sign < 0) {
                                    // <n-3,m+8|r^8 cos(8fi)|nm>
                                    sum -= 56.0 * k8c8 * System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0) * (n + l + 7.0) * (n + l + 8.0));
                                }
                                else if(diffn == 4 && sign < 0) {
                                    // <n-4,m+8|r^8 cos(8fi)|nm>
                                    sum += 70.0 * k8c8 * System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + l + 5.0) * (n + l + 6.0) * (n + l + 7.0) * (n + l + 8.0));
                                }
                                else if(diffn == 5 && sign < 0) {
                                    // <n-5,m+8|r^8 cos(8fi)|nm>
                                    sum -= 56.0 * k8c8 * System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + l + 6.0) * (n + l + 7.0) * (n + l + 8.0));
                                }
                                else if(diffn == 6 && sign < 0) {
                                    // <n-6,m+8|r^8 cos(8fi)|nm>
                                    sum += 28.0 * k8c8 * System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + 6.0) * (n + l + 7.0) * (n + l + 8.0));
                                }
                                else if(diffn == 7 && sign < 0) {
                                    // <n-7,m+8|r^8 cos(8fi)|nm>
                                    sum -= 8.0 * k8c8 * System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + 6.0) * (n + 7.0) * (n + l + 8.0));
                                }
                                else if(diffn == 8 && sign < 0) {
                                    // <n-8,m+8|r^8 cos(8fi)|nm>
                                    sum += k8c8 * System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + 6.0) * (n + 7.0) * (n + 8.0));
                                }
                            }

                            else if(diffl == 6) {
                                if(diffn == 0 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n,m+6|r^8 cos(8fi)|nm>
                                    sum += sqrt * k8c8
                                        * (8.0 * n + l + 7.0);
                                }
                                else if(diffn == 1 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n-1,m+6|r^8 cos(8fi)|nm>
                                    sum -= sqrt * 7.0 * k8c8
                                        * (4.0 * n + l + 7.0);
                                }
                                else if(diffn == 2 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n-2,m+6|r^8 cos(8fi)|nm>
                                    sum += sqrt * 7.0 * k8c8
                                        * (8.0 * n + 3.0 * l + 21.0);
                                }
                                else if(diffn == 3 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n-3,m+6|r^8 cos(8fi)|nm>
                                    sum -= sqrt * 35.0 * k8c8
                                        * (2.0 * n + l + 7.0);
                                }
                                else if(diffn == 4 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n-4,m+6|r^8 cos(8fi)|nm>
                                    sum += sqrt * 7.0 * k8c8
                                        * (8.0 * n + 5.0 * l + 35.0);
                                }
                                else if(diffn == 5 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + l + 6.0));
                                    // <n-5,m+6|r^8 cos(8fi)|nm>
                                    sum -= sqrt * 7.0 * k8c8
                                        * (4.0 * n + 3.0 * l + 21.0);
                                }
                                else if(diffn == 6 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + 6.0));
                                    // <n-6,m+6|r^8 cos(8fi)|nm>
                                    sum += sqrt * k8c8
                                        * (8.0 * n + 7.0 * l + 49.0);
                                }
                                else if(diffn == 7 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + 6.0) * (n + 7.0) * (n + l + 7.0));
                                    // <n-7,m+6|r^8 cos(8fi)|nm>
                                    sum -= sqrt * k8c8;
                                }
                                else if(diffn == 1 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0) * (n + l + 7.0));
                                    // <n+1,m+6|r^6 cos(8fi)|nm>
                                    sum -= sqrt * k8c8;
                                }
                            }
                            if(diffl == 4) {
                                if(diffn == 0 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n,m+4|r^8 cos(8fi)|nm>
                                    sum += sqrt * k8c8
                                        * (15.0 * n * (n - 1.0)
                                        + 12.0 * n * (n + l + 5.0)
                                        + (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 1 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-1,m+4|r^8 cos(8fi)|nm>
                                    sum -= sqrt * 2.0 * k8c8
                                        * (10.0 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 5.0)
                                        + 3.0 * (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 2 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-2,m+4|r^8 cos(8fi)|nm>
                                    sum += sqrt * 5.0 * k8c8
                                        * (3.0 * n * (n - 1.0)
                                        + 8.0 * n * (n + l + 5.0)
                                        + 3.0 * (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 3 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 4.0));
                                    // <n-3,m+4|r^8 cos(8fi)|nm>
                                    sum -= sqrt * 2.0 * k8c8
                                        * (3.0 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 5.0)
                                        + 10.0 * (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 4 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0));
                                    // <n-4,m+4|r^8 cos(8fi)|nm>
                                    sum += sqrt * k8c8
                                        * (n * (n - 1.0)
                                        + 12.0 * n * (n + l + 5.0)
                                        + 15.0 * (n + l + 5.0) * (n + l + 6.0));
                                }
                                else if(diffn == 5 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + l + 5.0));
                                    // <n-5,m+4|r^8 cos(8fi)|nm>
                                    sum -= sqrt * 2.0 * k8c8
                                        * (4.0 * n + 3.0 * l + 18.0);
                                }
                                else if(diffn == 6 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + 6.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n-6,m+4|r^8 cos(8fi)|nm>
                                    sum += sqrt * k8c8;
                                }
                                else if(diffn == 1 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                    // <n+1,m+4|r^8 cos(8fi)|nm>
                                    sum -= sqrt * 2.0 * k8c8
                                        * (4.0 * n + l + 6.0);
                                }
                                else if(diffn == 2 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0) * (n + l + 6.0));
                                    // <n+2,m+4|r^8 cos(8fi)|nm>
                                    sum += sqrt * k8c8;
                                }
                            }
                            else if(diffl == 2) {
                                if(diffn == 0 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0));
                                    // <n,m+2|r^8 cos(8fi)|nm>
                                    sum += sqrt * k8c8
                                        * (10.0 * n * (n - 1.0) * (n - 2.0)
                                        + 30.0 * n * (n - 1.0) * (n + l + 3.0)
                                        + 15.0 * n * (n + l + 3.0) * (n + l + 4.0)
                                        + (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 1 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 2.0));
                                    // <n-1,m+2|r^8 cos(8fi)|nm>
                                    sum -= sqrt * 5.0 * k8c8
                                        * (n * (n - 1.0) * (n - 2.0)
                                        + 6.0 * n * (n - 1.0) * (n + l + 3.0)
                                        + 6.0 * n * (n + l + 3.0) * (n + l + 4.0)
                                        + (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 2 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 2.0) * (n + 1.0));
                                    // <n-2,m+2|r^8 cos(8fi)|nm>
                                    sum += sqrt * k8c8
                                        * (n * (n - 1.0) * (n - 2.0)
                                        + 15.0 * n * (n - 1.0) * (n + l + 3.0)
                                        + 30.0 * n * (n + l + 3.0) * (n + l + 4.0)
                                        + 10.0 * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 3 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0) * (n + l + 3.0));
                                    // <n-3,m+2|r^8 cos(8fi)|nm>
                                    sum -= sqrt * k8c8
                                        * (3 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 4.0)
                                        + 10.0 * (n + l + 4.0) * (n + l + 5.0));

                                }
                                else if(diffn == 4 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n-4,m+2|r^8 cos(8fi)|nm>
                                    sum += sqrt * k8c8
                                        * (8.0 * n + 5.0 * l + 25.0);
                                }
                                else if(diffn == 5 && sign < 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0) * (n + 5.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                    // <n-4,m+2|r^8 cos(8fi)|nm>
                                    sum -= sqrt * k8c8;
                                }
                                else if(diffn == 1 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                    // <n+1,m+2|r^8 cos(8fi)|nm>
                                    sum -= sqrt * k8c8
                                        * (10.0 * n * (n - 1.0)
                                        + 15.0 * n * (n + l + 4.0)
                                        + 3.0 * (n + l + 4.0) * (n + l + 5.0));
                                }
                                else if(diffn == 2 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n+2,m+2|r^8 cos(8fi)|nm>
                                    sum += sqrt * k8c8
                                        * (8.0 * n + 3.0 * l + 15.0);
                                }
                                else if(diffn == 3 && sign > 0) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0) * (n + l + 5.0));
                                    // <n+2,m+2|r^8 cos(8fi)|nm>
                                    sum -= sqrt * k8c8;
                                }
                            }
                            else if(diffl == 0) {
                                if(diffn == 0) {
                                    // <nm|r^8 cos(8fi)|nm>
                                    sum += k8c8 * (n * (n - 1.0) * (n - 2.0) * (n - 3.0)
                                        + 16.0 * n * (n - 1.0) * (n - 2.0) * (n + l + 1.0)
                                        + 36 * n * (n - 1.0) * (n + l + 1.0) * (n + l + 2.0)
                                        + 16 * n * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0)
                                        + (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));

                                }
                                else if(diffn == 1) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + l + 1.0));
                                    // <n+1,m|r^6 cos(8fi)|nm>
                                    sum -= sqrt * 4.0 * k8c8
                                        * (n * (n - 1.0) * (n - 2.0)
                                        + 6.0 * n * (n - 1.0) * (n + l + 2.0)
                                        + 6.0 * n * (n + l + 2.0) * (n + l + 3.0)
                                        + (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                }
                                else if(diffn == 2) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + l + 1.0) * (n + l + 2.0));
                                    // <n+2,m|r^8 cos(8fi)|nm>
                                    sum += sqrt * 2.0 * k8c8
                                        * (3.0 * n * (n - 1.0)
                                        + 8.0 * n * (n + l + 3.0)
                                        + 3.0 * (n + l + 3.0) * (n + l + 4));
                                }

                                else if(diffn == 3) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                                    // <n+3,m|r^8 cos(8fi)|nm>
                                    sum -= sqrt * 4.0 * k8c8
                                        * (2.0 * n + l + 4.0);
                                }

                                else if(diffn == 4) {
                                    double sqrt = System.Math.Sqrt((n + 1.0) * (n + 2.0) * (n + 3.0) * (n + 4.0)
                                        * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0) * (n + l + 4.0));
                                    // <n+4,m|r^8 cos(8fi)|nm>
                                    sum += sqrt * k8c8;
                                }
                            }
                        }
                    }

                    if(sum != 0.0) {
                        matrix[i, j] = sum;
                        matrix[j, i] = sum;
                    }
                }
            }

            if(writer != null)
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));
        }
#endregion

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public override void Export(Export export) {
            base.Export(export);

            IEParam param = new IEParam();

            param.Add(this.hbar, "HBar");
            param.Add(this.eigenSystem, "EigenSystem");

            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public QuantumCWPolar(Core.Import import)
            : base(import) {
            IEParam param = new IEParam(import);

            this.hbar = (double)param.Get(0.1);

            this.eigenSystem = (EigenSystem)param.Get();
            this.eigenSystem.SetParrentQuantumSystem(this);
        }
        #endregion
    }
}
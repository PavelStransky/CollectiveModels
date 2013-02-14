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
        /// <param name="mu">Parametr MU</param>
        /// <param name="hbar">Planckova konstanta</param>
        public QuantumCWPolar(double a, double b, double mu, double hbar, int power)
            : base(a, b, mu, power) {
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

            double k4 = 0.5 * System.Math.Pow(s, 4);
            double k3 = 0.25 * this.B * System.Math.Pow(s, 3);
            double k2 = s * s * (0.5 * this.Mu - 1.0 - index.A0);
//            double k2 = s * s * (0.5 * this.Mu - 1.0);
            double k2c = -s * s * (0.5 * this.Mu + 1.0);
            double k1 = s * this.A;

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

                    int diffl = System.Math.Abs(mi - mj);
                    double l = System.Math.Min(li, lj);
                    int diffal = System.Math.Abs(li - lj);

                    double sum = 0.0;

                    // Výbìrové pravidlo
                    if(diffl == 0) {
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

                    if(diffl == 1 && ((li > lj && ni <= nj) || (li < lj && ni >= nj))) {
                        if(diffn == 0)
                            // A<n,m+1|r cos(fi)|nm>
                            sum += 0.5 * k1 * System.Math.Sqrt(n + l + 1.0);
                        else if(diffn == 1)
                            // A<n-1,m+1|r cos(fi)|nm>
                            sum -= 0.5 * k1 * System.Math.Sqrt(n + 1.0);
                    }

                    if(diffl == 2 && diffal == 2 && ((li > lj && ni <= nj) || (li < lj && ni >= nj))) {
                        if(diffn == 0)
                            // -(mu/2+1)<n,m+2|r^2 cos(2fi)|nm>
                            sum += 0.5 * k2c * System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0));
                        else if(diffn == 1)
                            // -(mu/2+1)<n-1,m+2|r^2 cos(2fi)|nm>
                            sum -= k2c * System.Math.Sqrt((n + 1.0) * (n + l + 2.0));
                        else if(diffn == 2)
                            // -(mu/2+1)<n-2,m+2|r^2 cos(2fi)|nm>
                            sum += 0.5 * k2c * System.Math.Sqrt((n + 2.0) * (n + 1.0));
                    }
                    if(diffl == 2 && diffal == 0) {
                        if(diffn == 0) {
                            // -(mu/2+1)<n,1|r^2 cos(2fi)|n,-1>=-1/2(mu/2+1)<nm|r^2|nm>
                            sum += 0.5 * k2c * (2.0 * n + l + 1.0);
                        }
                        else if(diffn == 1) {
                            // -(mu/2+1)<n+1,1|r^2 cos(2fi)|n,-1>=-1/2(mu/2+1)<n+1,m|r^2|nm>
                            sum -= 0.5 * k2c * System.Math.Sqrt((n + 1.0) * (n + l + 1.0));
                        }
                    }

                    if(diffl == 1 && ((li > lj && ni <= nj) || (li < lj && ni >= nj))) {
                        if(diffn == 0)
                            // B/4<n,m+1|r^3 cos(fi)|nm>
                            sum += 0.5 * k3 * (3.0 * n + l + 2.0) * System.Math.Sqrt(n + l + 1.0);
                        else if(diffn == 1)
                            // B/4<n-1,m+1|r^3 cos(fi)|nm>
                            sum -= 0.5 * k3 * (3.0 * n + 2.0 * l + 4.0) * System.Math.Sqrt(n + 1.0);
                        else if(diffn == 2)
                            // B/4<n-2,m+1|r^3 cos(fi)|nm>
                            sum += 0.5 * k3 * System.Math.Sqrt((n + 2.0) * (n + 1.0) * (n + l + 2));
                    }
                    if(diffl == 1 && diffn == 1 && ((li > lj && ni > nj) || (li < lj && ni < nj)))
                        // B/4<n+1,m+1|r^3 cos(fi)|nm>
                        sum -= 0.5 * k3 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0));

                    if(diffl == 3 && diffal == 3 && ((li > lj && ni <= nj) || (li < lj && ni >= nj))) {
                        if(diffn == 0)
                            // -B/4<n,m+3|r^3 cos(3fi)|nm>
                            sum -= 0.5 * k3 * System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                        else if(diffn == 1)
                            // -B/4<n-1,m+3|r^3 cos(3fi)|nm>
                            sum += 1.5 * k3 * System.Math.Sqrt((n + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                        else if(diffn == 2)
                            // -B/4<n-2,m+3|r^3 cos(3fi)|nm>
                            sum -= 1.5 * k3 * System.Math.Sqrt((n + 2.0) * (n + 1.0) * (n + l + 3.0));
                        else if(diffn == 3)
                            // -B/4<n-3,m+3|r^3 cos(3fi)|nm>
                            sum += 0.5 * k3 * System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0));
                    }
                    if(diffl == 3 && diffal == 1 && ((li > lj && ni <= nj) || (li < lj && ni >= nj))) {
                        if(diffn == 0)
                            // -B/4<n,m+3|r^3 cos(3fi)|nm> = -B/4<n,m+1|r^3 cos(fi)|nm>
                            sum -= 0.5 * k3 * (3.0 * n + l + 2.0) * System.Math.Sqrt(n + l + 1.0);
                        else if(diffn == 1)
                            // -B/4<n-1,m+3|r^3 cos(3fi)|nm> = -B/4<n-1,m+1|r^3 cos(fi)|nm>
                            sum += 0.5 * k3 * (3.0 * n + 2.0 * l + 4.0) * System.Math.Sqrt(n + 1.0);
                        else if(diffn == 2)
                            // -B/4<n-2,m+3|r^3 cos(3fi)|nm> = -B/4<n-2,m+1|r^3 cos(fi)|nm>
                            sum -= 0.5 * k3 * System.Math.Sqrt((n + 2.0) * (n + 1.0) * (n + l + 2));
                    }
                    if(diffl == 3 && diffal == 1 && ((li > lj && ni > nj) || (li < lj && ni < nj))) {
                        if(diffn == 1)
                        // -B/4<n+1,m+3|r^3 cos(3 fi)|nm> = -B/4<n+1,m+1|r^3 cos(fi)|nm>
                        sum += 0.5 * k3 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0));
                    }

                    if(diffl == 2 && diffal == 2 && ((li > lj && ni <= nj) || (li < lj && ni >= nj))) {
                        if(diffn == 0)
                            // 1/4<n,m+2|r^4 cos(2fi)|nm>
                            sum += 0.5 * k4 * (4.0 * n + l + 3.0) * System.Math.Sqrt((n + l + 1.0) * (n + l + 2.0));
                        else if(diffn == 1)
                            // 1/4<n-1,m+2|r^4 cos(2fi)|nm>
                            sum -= 1.5 * k4 * (2.0 * n + l + 3.0) * System.Math.Sqrt((n + 1.0) * (n + l + 2.0));
                        else if(diffn == 2)
                            // 1/4<n-2,m+2|r^4 cos(2fi)|nm>
                            sum += 0.5 * k4 * (4.0 * n + 3.0 * l + 9.0) * System.Math.Sqrt((n + 2.0) * (n + 1.0));
                        else if(diffn == 3)
                            // 1/4<n-3,m+2|r^4 cos(2fi)|nm>
                            sum -= 0.5 * k4 * System.Math.Sqrt((n + 3.0) * (n + 2.0) * (n + 1.0) * (n + l + 3.0));
                    }
                    if(diffl == 2 && diffal == 2 && diffn == 1 && ((li > lj && ni > nj) || (li < lj && ni < nj)))
                        // 1/4<n+1,m+2|r^4 cos(2fi)|nm>
                        sum -= 0.5 * k4 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0) * (n + l + 2.0) * (n + l + 3.0));
                    if(diffl == 2 && diffal == 0) {
                        if(diffn == 0)
                            // 1/4<n,m+2|r^4 cos(2fi)|nm> = 1/8<nm|r^4|nm>
                            sum += 0.5 * k4 * (n * (n - 1.0) + (n + l + 1.0) * (5.0 * n + l + 2.0));
                        else if(diffn == 1) 
                            // 1/4<n+1,m+2|r^4 cos(2fi)|nm> = 1/8<n+1,m|r^4|nm>
                            sum -= k4 * System.Math.Sqrt((n + 1.0) * (n + l + 1.0)) * (2.0 * n + l + 2.0);
                        else if(diffn == 2)
                            // 1/4<n+2,m+2|r^4 cos(2fi)|nm> = 1/8<n+2,m|r^4|nm>
                            sum += 0.5 * k4 * System.Math.Sqrt((n + l + 2.0) * (n + l + 1.0) * (n + 2.0) * (n + 1.0));                        
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
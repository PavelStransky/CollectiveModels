using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    public class QuantumCW : CW, IQuantumSystem {
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
        public QuantumCW(double a, double b, double c, double mu, double hbar, int power) : base(a, b, c, mu, power) {
            this.hbar = hbar;
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        protected QuantumCW() { }

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
        /// Vrátí matici <n|V|n> amplitudy vlastní funkce n
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="rx">Rozmìry ve smìru x</param>
        /// <param name="ry">Rozmìry ve smìru y</param>
        public Matrix[] AmplitudeMatrix(int[] n, IOutputWriter writer, DiscreteInterval intx, DiscreteInterval inty) {
            return null;
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

            for (int l = 0; l < numn; l++) {
                result[l] = new Matrix(numx, numy);

                for (int i = 0; i < numx; i++)
                    for (int j = 0; j < numy; j++)
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
            return new CWBasisIndex(basisParams);
        }

        /// <summary>
        /// Napoèítá Hamiltonovu matici v dané bázi
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="basisIndex">Parametry báze</param>
        public void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            CWBasisIndex index = basisIndex as CWBasisIndex;

            double omegax = index.OmegaX;
            double omegay = index.OmegaY;
            
            double sx = System.Math.Sqrt(this.hbar / (2.0 * omegax));
            double sy = System.Math.Sqrt(this.hbar / (2.0 * omegay));

            int length = index.Length;
            int bandWidth = index.BandWidth;

            DateTime startTime = DateTime.Now;

            if (writer != null)
                writer.Write(string.Format("Pøíprava H ({0} x {1})...", length, length));

            for (int i = 0; i < length; i++) {
                int ni = index.N[i];
                int mi = index.M[i];

                for (int j = i; j < length; j++) {
                    int nj = index.N[j];
                    int mj = index.M[j];

                    double sum = 0.0;

                    // Výbìrové pravidlo
                    if(nj == ni) {
                        if(mj == mi) {
                            double px = System.Math.Pow(sx, 2) * (2.0 * ni + 1.0);
                            double py = System.Math.Pow(sy, 2) * (2.0 * mi + 1.0);

                            // <n|H0x|n>
                            sum += this.hbar * omegax * (ni + 0.5);
                            // <m|H0y|m>
                            sum += this.hbar * omegay * (mi + 0.5);

                            if(this.Power == 4) {
                                // <n|x^8|n>
                                sum += System.Math.Pow(sx, 8) * 
                                    (105.0 + ni * 
                                    (840.0 + System.Math.Max(0, ni - 1) * 
                                    (1260.0 + System.Math.Max(0, ni - 2) * 
                                    (560.0 + System.Math.Max(0, ni - 3) * 
                                    70.0))));
                                // -4 <n|x^6|n>
                                sum -= 4.0 * System.Math.Pow(sx, 6) * 
                                    (15.0 + ni * 
                                    (90.0 + System.Math.Max(0, ni - 1) * 
                                    (90.0 + System.Math.Max(0, ni - 2) * 
                                    20.0)));
                                // 6 <n|x^4|n>
                                sum += 6.0 * System.Math.Pow(sx, 4) * 
                                    (3.0 + ni * 
                                    (12.0 + System.Math.Max(0, ni - 1) * 
                                    6.0));
                                // (-4 - Ax) <n|x^2|n>
                                sum -= (4.0 + index.A0x) * px;
                            }
                            else {
                                // <n|x^4|n>
                                sum += System.Math.Pow(sx, 4) * 
                                    (3.0 + ni * 
                                    (12.0 + System.Math.Max(0, ni - 1) * 
                                    6.0));
                                // (-2 - Ax) <n|x^2|n>
                                sum -= (2.0 + index.A0x) * px;
                            }

                            // (mu - Ay) <m|y^2|m>
                            sum += (this.Mu - index.A0y) * py;
                            // C<n|<m|x^2 y^2|m>|n>
                            sum += this.C * px * py;

                            sum += 1.0;
                        }
                        else if(mj == mi + 2) {
                            double px = System.Math.Pow(sx, 2) * (2.0 * ni + 1.0);
                            double psy = System.Math.Pow(sy, 2) * System.Math.Sqrt((mi + 1.0) * (mi + 2.0));

                            // (mu - Ay) <m|y^2|m+2>
                            sum += (this.Mu - index.A0y) * psy;
                            // C<n|<m|x^2 y^2|m+2>|n>
                            sum += this.C * px * psy;
                        }
                    }
                    else if(nj == ni + 1) {
                        if(mj == mi)
                            // <n|x|n+1> (a + b <m|y^2|m>)
                            sum += sx * System.Math.Sqrt(ni + 1.0) * (this.A + this.B * System.Math.Pow(sy, 2) * (2.0 * mi + 1.0));
                        else if(mj == mi + 2)
                            // b <n|<m|x y^2|m+2>|n+1>
                            sum += this.B * sx * System.Math.Pow(sy, 2) * System.Math.Sqrt((ni + 1.0) * (mi + 1.0) * (mi + 2.0));
                    }
                    else if(nj == ni + 2) {
                        if(mj == mi) {
                            double psx = System.Math.Pow(sx, 2) * System.Math.Sqrt((ni + 1.0) * (ni + 2.0));
                            double py = System.Math.Pow(sy, 2) * (2.0 * mi + 1.0);

                            if(this.Power == 4) {
                                // <n|x^8|n+2>
                                sum += System.Math.Pow(sx, 6) * psx * 
                                    (420.0 + ni * 
                                    (840.0 + System.Math.Max(0, ni - 1) * 
                                    (420.0 + System.Math.Max(0, ni - 2) * 
                                    56.0)));
                                // -4 <n|x^6|n+2>
                                sum -= 4.0 * System.Math.Pow(sx, 4) * psx * 
                                    (45.0 + ni * 
                                    (60.0 + System.Math.Max(0, ni - 1) * 
                                    15.0));
                                // 6 <n|x^4|n+2>
                                sum += 6.0 * System.Math.Pow(sx, 2) * psx * (6.0 + ni * 4.0);
                                // (-4 - Ax) <n|x^2|n+2>
                                sum -= (4.0 + index.A0x) * psx;
                            }
                            else {
                                // <n|x^4|n+2>
                                sum += System.Math.Pow(sx, 2) * psx * (6.0 + ni * 4.0);
                                // (-2 - Ax) <n|x^2|n+2>
                                sum -= (2.0 + index.A0x) * psx;
                            }
                            // C<n|<m|x^2 y^2|m>|n+2>
                            sum += this.C * psx * py;
                        }
                        else if(mj == mi + 2)
                            // C<n|<m|x^2 y^2|m+2>|n+2>
                            sum += this.C * System.Math.Pow(sx, 2) * System.Math.Pow(sy, 2) * System.Math.Sqrt((ni + 1.0) * (ni + 2.0) * (mi + 1.0) * (mi + 2.0));
                    }

                    else if(nj == ni + 4 && mj == mi) {
                        double psx = System.Math.Pow(sx, 4) * System.Math.Sqrt((ni + 1.0) * (ni + 2.0) * (ni + 3.0) * (ni + 4.0));

                        if(this.Power == 4) {
                            // <n|x^8|n+4>
                            sum += System.Math.Pow(sx, 4) * psx * 
                                (210.0 + ni * 
                                (168.0 + System.Math.Max(0, ni - 1) * 
                                28.0));
                            // -4 <n|x^6|n+4>
                            sum -= 4.0 * System.Math.Pow(sx, 2) * psx * (15.0 + ni * 6.0);
                            // 6 <n|x^4|n+4>
                            sum += 6.0 * psx;
                        }
                        else {
                            // <n|x^4|n+4>
                            sum += psx;
                        }
                    }

                    else if(nj == ni + 6 && mj == mi && this.Power == 4) {
                        double psx = System.Math.Pow(sx, 6) * System.Math.Sqrt((ni + 1.0) * (ni + 2.0) * (ni + 3.0) * (ni + 4.0) * (ni + 5.0) * (ni + 6.0));

                        // <n|x^8|n+6>
                        sum += System.Math.Pow(sx, 2) * psx * (28.0 + ni * 8.0);
                        // -4 <n|x^6|n+6>
                        sum -= 4.0 * psx;
                    }

                    else if(nj == ni + 8 && mj == mi && this.Power == 4) {
                        // <n|x^8|n+6>
                        sum += System.Math.Pow(sx, 8) * System.Math.Sqrt((ni + 1.0) * (ni + 2.0) * (ni + 3.0) * (ni + 4.0) * (ni + 5.0) * (ni + 6.0) * (ni + 7.0) * (ni + 8.0));
                    }

                    if(sum != 0.0) {
                        matrix[i, j] = sum;
                        matrix[j, i] = sum;
                    }
                 }
            }

            if (writer != null)
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));
        }
        #endregion

        public double LevelDensity(double e, double step) {
            if(this.Power != 2)
                throw new SystemsException("This function is implemented only for power = 2");

            BisectionPotential bp = new BisectionPotential(this, 0.0, 0.0, e);
            Bisection b = new Bisection(bp.BisectionX);

            Vector roots = new Vector(4);
            double x = 0;
            int length = 0;

            BisectionDxPotential bdxp = new BisectionDxPotential(this);
            Bisection bdx = new Bisection(bdxp.Bisection);

            Vector r = new Vector(3);
            x = bdx.Solve(-10 - System.Math.Abs(this.A), -System.Math.Sqrt(1.0 / 3.0));
            if(!double.IsNaN(x))
                r[length++] = x;

            x = bdx.Solve(-System.Math.Sqrt(1.0 / 3.0), System.Math.Sqrt(1.0 / 3.0));
            if(!double.IsNaN(x))
                r[length++] = x;

            x = bdx.Solve(System.Math.Sqrt(1.0 / 3.0), 10 + System.Math.Abs(this.A));
            if(!double.IsNaN(x))
                r[length++] = x;

            r.Length = length;
            length = 0;

            x = b.Solve(-10 - System.Math.Abs(this.A), r.FirstItem);
            if(!double.IsNaN(x))
                roots[length++] = x;

            for(int i = 1; i < r.Length; i++) {
                x = b.Solve(r[i - 1], r[i]);
                if(!double.IsNaN(x))
                    roots[length++] = x;
            }

            x = b.Solve(r.LastItem, 10 + System.Math.Abs(this.A));
            if(!double.IsNaN(x))
                roots[length++] = x;

            roots.Length = length;

            double result = 0.0;

            if(length > 1) 
                result += this.LevelDensityInterval(roots[0] + step / 100.0, roots[1] - step / 100.0, step, e);
           
            if(length > 2)
                result += this.LevelDensityInterval(roots[roots.Length - 2] + step / 100.0, roots[roots.Length - 1] - step / 100.0, step, e);

            return 2.0 / (3.0 * System.Math.PI * this.Hbar * this.Hbar) * result;
        }

        private double LevelDensityIntegrand(double x, double e) {
            double x2 = x * x;
            return System.Math.Pow(e - 1.0 - this.A * x + 2.0 * x2 - x2 * x2, 1.5) / System.Math.Sqrt(this.B * x + this.C * x2 + this.Mu);
        }

        private double LevelDensityInterval(double minx, double maxx, double step, double e) {
            double result = 0.0;

            if(maxx <= minx)
                return result;

            double x = minx;
            double y1 = this.LevelDensityIntegrand(x, e);

            for(x += step; x < maxx; x += step) {
                double y2 = this.LevelDensityIntegrand(x, e);
                result += 0.5 * step * (y2 + y1);
                y1 = y2;
            }

            result += 0.5 * (maxx - x + step) * (this.LevelDensityIntegrand(maxx, e) + y1);

            return result;
        }

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
        public QuantumCW(Core.Import import) : base(import) {
            IEParam param = new IEParam(import);

            this.hbar = (double)param.Get(0.1);

            this.eigenSystem = (EigenSystem)param.Get();
            this.eigenSystem.SetParrentQuantumSystem(this);
        }
        #endregion
    }
}
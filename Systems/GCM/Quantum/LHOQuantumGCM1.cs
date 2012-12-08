using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Systems;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Kvantový GCM s jedním minimem (pro studium ESQPT 1)
    /// </summary>
    public class LHOQuantumGCM1: GCM, IExportable, IQuantumSystem {
        // Parametr pro LHO
        private double a0;

        // Planckova konstanta
        private double hbar;                    // [Js]

        // Koeficienty
        protected double s;
        protected double omega;

        // Vektor s Cos koeficienty
        private Vector cosCoef;

        /// <summary>
        /// Planckova konstanta [Js]
        /// </summary>
        public double Hbar { get { return this.hbar; } }

        /// <summary>
        /// Úhlová frekvence LHO [J*m^-2]
        /// </summary>
        public double Omega { get { return this.omega; } }

        /// <summary>
        /// Parametr pro LHO [s^-1]
        /// </summary>
        public double A0 { get { return this.a0; } set { this.a0 = value; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCM1(double a, double b, double c, double k, double a0, double hbar, Vector cosCoef)
            : base(a, b, c, k) {
            this.a0 = a0;
            this.hbar = hbar;

            this.cosCoef = cosCoef;

            this.eigenSystem = new EigenSystem(this);
            this.RefreshConstants();
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        protected LHOQuantumGCM1() { }

        /// <summary>
        /// Pøepoèítá konstanty
        /// </summary>
        protected virtual void RefreshConstants() {
            this.omega = System.Math.Sqrt(2.0 * this.a0 / this.K);
            this.s = System.Math.Sqrt(this.K * this.omega / this.hbar);      // xi = s*x (Formanek (2.283))
        }

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
        public virtual Vector PeresInvariant(int type) {
            return null;
        }

        /// <summary>
        /// Radial part of the wave function
        /// </summary>
        /// <param name="l">Principal quantum number</param>
        /// <param name="mu">Second quantum number</param>
        /// <param name="x">Value</param>
        public double PsiR(double x, int n, int m) {
            double xi2 = this.s * x; xi2 *= xi2;
            m = System.Math.Abs(m);

            double normLog = 0.5 * (System.Math.Log(2.0) + SpecialFunctions.FactorialILog(n) - SpecialFunctions.FactorialILog(n + m)) + (m + 1) * System.Math.Log(this.s);
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
            LHOPolarIndex1 index = this.eigenSystem.BasisIndex as LHOPolarIndex1;
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
            LHOPolarIndex1 index = this.eigenSystem.BasisIndex as LHOPolarIndex1;
            int n = index.N[i];
            int m = index.M[i];

            if(this.cosCoef.LastItem < 0)
                x = -x;

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
            return new LHOPolarIndex1(basisParams, this.cosCoef.Length);
        }

        /// <summary>
        /// Napoèítá Hamiltonovu matici v dané bázi
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="basisIndex">Parametry báze</param>
        public void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            LHOPolarIndex1 index = basisIndex as LHOPolarIndex1;
            int maxE = index.MaxE;

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;
            double alpha32 = alpha * this.s;

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

                    double sum = 0.0;

                    // Výbìrové pravidlo
                    if(diffl == 0) {
                        if(diffn == 0) {
                            sum += this.Hbar * omega * (2.0 * n + l + 1.0);
                            sum += (this.A - this.A0) * (2.0 * n + l + 1.0) / alpha;
                            sum += this.C * (n * (n - 1.0) + (n + l + 1.0) * (5.0 * n + l + 2.0)) / alpha2;
                        }

                        else if(diffn == 1) {
                            sum -= (this.A - this.A0) * System.Math.Sqrt((n + 1.0) * (n + l + 1.0)) / alpha;
                            sum -= 2.0 * this.C * System.Math.Sqrt((n + 1.0) * (n + l + 1.0)) * (2.0 * n + l + 2.0) / alpha2;
                        }

                        else if(diffn == 2)
                            sum += this.C * System.Math.Sqrt((n + l + 2.0) * (n + l + 1.0) * (n + 2.0) * (n + 1.0)) / alpha2;
                    }

                    double k = (mi == mj) ? 1.0 : 0.5;
                    k *= this.B / alpha32;

                    double cos = 0.0;
                    for(int ci = 0; ci < this.cosCoef.Length; ci++) {
                        if(this.cosCoef[ci] != 0 && System.Math.Abs(mi - mj) == ci)
                            cos += this.cosCoef[ci] * SpecialFunctions.HO2DMatrixElement(ni, nj, mi, mj, 3);
                    }
                    sum += k * cos;

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
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.A, "A");
            param.Add(this.B, "B");
            param.Add(this.C, "C");
            param.Add(this.K, "K");
            param.Add(this.A0, "A0");
            param.Add(this.Hbar, "HBar");
            param.Add(this.cosCoef, "CosCoef");

            param.Add(this.eigenSystem, "EigenSystem");

            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public LHOQuantumGCM1(Core.Import import) {
            IEParam param = new IEParam(import);

            this.A = (double)param.Get(-1.0);
            this.B = (double)param.Get(1.0);
            this.C = (double)param.Get(1.0);
            this.K = (double)param.Get(1.0);
            this.A0 = (double)param.Get(1.0);
            this.hbar = (double)param.Get(0.1);
            this.cosCoef = (Vector)param.Get();

            this.eigenSystem = (EigenSystem)param.Get();
            this.eigenSystem.SetParrentQuantumSystem(this);            

            this.RefreshConstants();
        }
        #endregion
    }
}
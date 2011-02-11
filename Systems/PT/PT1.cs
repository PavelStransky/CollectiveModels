using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.DLLWrapper;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    /// <summary>
    /// System for studying quantum phase transitions
    /// </summary>
    public class PT1: IExportable, IQuantumSystem {
        // Frequency of the basis
        protected double omega0;

        // Mixing parameter
        protected double m;

        // Planck constant
        protected double hbar;                    // [Js]

        // Systém s vlastními hodnotami
        protected EigenSystem eigenSystem;

        /// <summary>
        /// Systém vlastních hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="omega0">Basis frequency</param>
        /// <param name="m">Mixing parameter</param>
        /// <param name="hbar">Planck constant</param>
        public PT1(double m, double omega0, double hbar) {
            this.omega0 = omega0;
            this.m = m;
            this.hbar = hbar;

            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvoøí instanci tøídy LHOPolarIndexIFull
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public BasisIndex CreateBasisIndex(Vector basisParams) {
            return new PTBasisIndex(basisParams);
        }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected PT1() { }

        /// <summary>
        /// Vypoèítá Hamiltonovu matici (v tomto pøípadì lze poèítat algebraicky)
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public virtual void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            int maxn = (basisIndex as PTBasisIndex).MaxN;

            double alpha2 = this.omega0 / this.hbar;
            double alpha = System.Math.Sqrt(alpha2);
            double alpha4 = alpha2 * alpha2;

            double a = 8.0 * System.Math.Abs(m) - 2.0 - 0.5 * this.omega0 * this.omega0;

            // Temporary variables
            double r = this.hbar * this.omega0;
            double s = 0.5 / alpha4;
            double t = 0.5 * a / alpha2;
            double u = 2.0 * this.m / (alpha2 * alpha);
            double v = 4.0 * this.m / alpha;

            double rsqr2 = 1.0 / System.Math.Sqrt(2.0);

            for(int i = 0; i < maxn; i++) {
                double i1 = System.Math.Sqrt(i + 1);
                double i2 = i1 * System.Math.Sqrt(i + 2);
                double i3 = i2 * System.Math.Sqrt(i + 3);
                double i4 = i3 * System.Math.Sqrt(i + 4);

                matrix[i, i] = r * (i + 0.5) + 1.0 + 3.0 * s * (i * i + i + 0.5) + t * (2 * i + 1);
                if(i + 1 < maxn)
                    matrix[i, i + 1] = i1 * rsqr2 * (3.0 * u * (i + 1) + v);
                if(i + 2 < maxn)
                    matrix[i, i + 2] = i2 * (s * (2.0 * i + 3.0) + t);
                if(i + 3 < maxn)
                    matrix[i, i + 3] = i3 * rsqr2 * u;
                if(i + 4 < maxn)
                    matrix[i, i + 4] = 0.5 * s * i4;
            }
        }

        /// <summary>
        /// Vrátí matici hustot pro vlastní funkce
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="interval">Rozmìry v jednotlivých smìrech (uspoøádané ve tvaru [minx, maxx,] numx, ...)</param>
        public object ProbabilityDensity(int[] n, IOutputWriter writer, params Vector[] interval) {
            double minx = interval[0][0];
            double maxx = interval[0][1];
            int num = (int)interval[0][2];

            int numn = n.Length;
            int length = (this.eigenSystem.BasisIndex as PTBasisIndex).Length;

            DateTime startTime = DateTime.Now;
            if(writer != null)
                writer.Write(string.Format("Calculating {0} wave functions...", numn));

            DiscreteInterval di = new DiscreteInterval(minx, maxx, num);            
            double [,]cache = new double[num, length];

            double s = System.Math.Sqrt(this.omega0 / this.hbar);

            for(int i = 0; i < num; i++) {
                double x = di.GetX(i);
                for(int j = 0; j < length; j++)
                    cache[i, j] = Psi(x, j, s);
            }

            Vector []result = new Vector[numn];

            for(int j = 0; j < numn; j++) {
                Vector ev = this.eigenSystem.GetEigenVector(n[j]);

                result[j] = new Vector(num);
                for(int k = 0; k < length; k++)
                    for(int i = 0; i < num; i++)
                        result[j][i] += ev[k] * cache[i, k];

                for(int i = 0; i < num; i++)
                    result[j][i] *= result[j][i];
            }

            if(writer != null)
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));

            return result;            
        }

        /// <summary>
        /// Vrátí hustotu vlnové funkce v daném bodì
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="x">Bod</param>
        public double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] xx) {
            Vector ev = this.eigenSystem.GetEigenVector(n);
            int length = ev.Length;
            double result = 0.0;

            double x = xx[0];
            double s = System.Math.Sqrt(this.omega0 / this.hbar);

            for(int k = 0; k < length; k++)
                result += ev[k] * Psi(x, k, s);

            return result;
        }

        /// <summary>
        /// Potential
        /// </summary>
        /// <param name="m">Mixing parameter</param>
        public static double V(double x, double m) {
            double x2 = x * x;
            return x2 * x2 + 1 + 4.0 * m * x * (x2 + 1) + (8.0 * System.Math.Abs(m) - 2.0) * x2;
        }

        /// <summary>
        /// Value of a wave function of the basis
        /// </summary>
        /// <param name="n">Principal quantum number</param>
        /// <param name="x">Value</param>
        /// <param name="s">Multiplication factor</param>
        public static double Psi(double x, int n, double s) {
            double xi = s * x; 

            double normLog = 0.5 * System.Math.Log(s / System.Math.Sqrt(System.Math.PI)) - 0.5 * (SpecialFunctions.FactorialILog(n) + n * System.Math.Log(2.0));
            double r = 0.0;
            double e = 0.0;
            SpecialFunctions.Hermite(out r, out e, xi, n);

            if(r == 0.0)
                return 0.0;

            double rLog = System.Math.Log(System.Math.Abs(r));
            double result = normLog - xi * xi / 2.0 + rLog + e;
            result = r < 0.0 ? -System.Math.Exp(result) : System.Math.Exp(result);

            return result;
        }

        /// <summary>
        /// Returns sum of logarithms of differences between E_i and other energies
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double SumLn(int i) {
            int numEV = this.eigenSystem.NumEV;
            double e = this.eigenSystem.GetEigenValues()[i];

            double result = 0.0;
            for(int j = 0; j < numEV; j++)
                if(j != i)
                    result += System.Math.Log(System.Math.Abs(this.eigenSystem.GetEigenValues()[j] - e));

            return result;
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.omega0, "Omega");
            param.Add(this.m, "M");
            param.Add(this.hbar, "HBar");
            param.Add(this.eigenSystem, "EigenSystem");

            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public PT1(Core.Import import) {
            IEParam param = new IEParam(import);

            this.omega0 = (double)param.Get(1.0);
            this.m = (double)param.Get(1.0);
            this.hbar = (double)param.Get(0.1);

            if(import.VersionNumber < 7)
                this.eigenSystem = new EigenSystem(param);
            else
                this.eigenSystem = (EigenSystem)param.Get();
            this.eigenSystem.SetParrentQuantumSystem(this);
        }
        #endregion

        public Vector PeresInvariant(int type) {
            throw new NotImpException(this, "PeresInvariant");
        }

        public Vector PeresInvariant(int type, PointVector state) {
            throw new NotImpException(this, "PeresInvariant");
        }
    }
}
using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Systems;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Kvantový GCM 
    /// </summary>
    public abstract class LHOQuantumGCM : GCM, IExportable, IQuantumSystem {
        // Parametr pro LHO
        private double a0;
 
        // Planckova konstanta
        private double hbar;                    // [Js]

        // Koeficienty
        protected double s;
        protected double omega;

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
        public LHOQuantumGCM(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k) {
            this.a0 = a0;
            this.hbar = hbar;

            this.eigenSystem = new EigenSystem(this);
            this.RefreshConstants();
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        protected LHOQuantumGCM() { }

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
        /// Vrátí velikost Hamiltonovy matice v dané bázi
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        public virtual double HamiltonianMatrixTrace(BasisIndex basisIndex) {
            return this.HamiltonianMatrix(basisIndex, null).Trace();
        }
        
        /// <summary>
        /// Napoèítá Hamiltonovu matici v dané bázi
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="basisIndex">Parametry báze</param>
        public virtual Matrix HamiltonianMatrix(BasisIndex basisIndex, IOutputWriter writer) {
            return (Matrix)this.HamiltonianSBMatrix(basisIndex, writer);
        }

        /// <summary>
        /// Napoèítá Hamiltonovu matici jako pásovou matici
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="basisIndex">Parametry báze</param>
        public virtual SymmetricBandMatrix HamiltonianSBMatrix(BasisIndex basisIndex, IOutputWriter writer) {
            throw new NotImpException(this, "HamiltonianSBMatrix");
        }

        /// <summary>
        /// Druhý invariant
        /// </summary>
        /// <param name="type">Typ Peresova operátoru</param>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        public virtual Vector PeresInvariant(int type) {
            if(type == 0) {
                return this.PeresInvariantL2();
            }

            else if(type == 1) {
                return this.PeresInvariantHPrime();
            }

            else if(type == 2) {
                return this.PeresInvariantHOscillator();
            }

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

            Matrix[] result = this.AmplitudeMatrix(n, writer, intx, inty);

            int numn = result.Length;
            int numx = result[0].LengthX;
            int numy = result[0].LengthY;

            for(int l = 0; l < numn; l++)
                for(int i = 0; i < numx; i++)
                    for(int j = 0; j < numy; j++)
                        result[l][i, j] *= result[l][i, j];

            return result;
        }

        /// <summary>
        /// Vrátí hustotu vlnové funkce v daném bodì
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="x">Bod</param>
        public double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] x) {
            Vector ev = this.eigenSystem.GetEigenVector(n);
            int length = this.eigenSystem.BasisIndex.Length;

            double result = 0;
            for(int k = 0; k < length; k++)
                result += ev[k] * this.PsiXY(x[0], x[1], k);
            
            return result;
        }

        /// <summary>
        /// Vytvoøí tøídu s parametry báze
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        public abstract BasisIndex CreateBasisIndex(Vector basisIndex);
        #endregion


        /// <summary>
        /// Koeficient pro výpoèet druhého invariantu
        /// </summary>
        /// <param name="j">Index k vlnové funkci</param>
        protected abstract double PeresInvariantCoef(int j);

        /// <summary>
        /// Peresùv operátor L2
        /// </summary>
        protected virtual Vector PeresInvariantL2() {
            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenSystem.GetEigenVector(i);
                int length = ev.Length;

                for(int j = 0; j < length; j++)
                    result[i] += ev[j] * ev[j] * this.PeresInvariantCoef(j);
            }

            return result;
        }

        /// <summary>
        /// Peresùv operátor H'
        /// </summary>
        protected virtual Vector PeresInvariantHPrime() {
            return null;
        }

        /// <summary>
        /// Peresùv operátor H v oscilátorové bázi
        /// </summary>
        protected virtual Vector PeresInvariantHOscillator() {
            return null;
        }

        #region Vlnové funkce
        /// <summary>
        /// Vlnová funkce v souøadnicích x, y
        /// </summary>
        /// <param name="x">Souøadnice x</param>
        /// <param name="y">Souøadnice y</param>
        /// <param name="n">Index vlnové funkce</param>
        protected virtual double PsiXY(double x, double y, int n) {
            double beta = System.Math.Sqrt(x * x + y * y);
            double gamma = (x > 0 ? System.Math.Atan(y / x) : System.Math.PI - System.Math.Atan(y / x));

            return this.PsiBG(beta, gamma, n);
        }

        /// <summary>
        /// Vlnová funkce v souøadnicích beta, gamma
        /// </summary>
        /// <param name="beta">Souøadnice beta</param>
        /// <param name="gamma">Souøadnice gamma</param>
        /// <param name="n">Index vlnové funkce</param>
        protected virtual double PsiBG(double beta, double gamma, int n) {
            return 0.0;
        }

        /// <summary>
        /// Radiální èást 2D vlnové funkce
        /// </summary>
        /// <param name="n">Hlavní kvantové èíslo</param>
        /// <param name="m">Úhlové kvantové èíslo</param>
        /// <param name="x">Souøadnice</param>
        public double Psi2D(double x, int n, int m) {
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
        /// Úhlová èást vlnové 2D funkce (liché)
        /// </summary>
        /// <param name="gamma">Souøadnice gamma</param>
        /// <param name="m">Úhlové kvantové èíslo</param>
        /// <returns></returns>
        public double Phi2DO(double gamma, int m) {
            return System.Math.Sin(m * gamma) * norm;
        }

        /// <summary>
        /// Úhlová èást vlnové 2D funkce (sudé)
        /// </summary>
        /// <param name="gamma">Souøadnice gamma</param>
        /// <param name="m">Úhlové kvantové èíslo</param>
        /// <returns></returns>
        public double Phi2DE(double gamma, int m) {
            if(m == 0)
                return norm / System.Math.Sqrt(2.0);
            else
                return System.Math.Cos(m * gamma) * norm;
        }

        private static double norm = 1 / System.Math.Sqrt(System.Math.PI);

        /// <summary>
        /// Radial part of the 5D wave function
        /// </summary>
        /// <param name="l">Principal quantum number</param>
        /// <param name="mu">Second quantum number</param>
        /// <param name="x">Value</param>
        public double Psi5D(double x, int l, int mu) {
            double xi2 = this.s * x; xi2 *= xi2;
            int lambda = 3 * mu;

            double normLog = (lambda + 2.5) * System.Math.Log(this.s) + 0.5 * (SpecialFunctions.FactorialILog(l) - SpecialFunctions.HalfFactorialILog(lambda + l + 2) + System.Math.Log(2.0));
            double r = 0.0;
            double e = 0.0;
            SpecialFunctions.Laguerre(out r, out e, xi2, l, lambda + 1.5);

            if(r == 0.0 || x == 0.0)
                return 0.0;

            double rLog = System.Math.Log(System.Math.Abs(r));
            double result = normLog + lambda * System.Math.Log(x) - xi2 / 2.0 + rLog + e;
            result = r < 0.0 ? -System.Math.Exp(result) : System.Math.Exp(result);

            return result;
        }

        /// <summary>
        /// Angular part of the 5D wave function
        /// </summary>
        /// <param name="g">Angle gamma</param>
        /// <param name="mu">Angular quantum number</param>
        public double Phi5D(double g, int mu) {
            double result = SpecialFunctions.Legendre(System.Math.Cos(3.0 * g), mu);
            double norm = System.Math.Sqrt((2.0 * mu + 1.0) / 4.0);
            return result * norm;
        }
        #endregion

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

            Matrix[] result = new Matrix[numn];
            for(int i = 0; i < numn; i++)
                result[i] = new Matrix(numx, numy);

            int length = this.eigenSystem.BasisIndex.Length;
            int length100 = length / 100;

            DateTime startTime = DateTime.Now;

            for(int k = 0; k < length; k++) {
                BasisCache2D cache = new BasisCache2D(intx, inty, k, this.PsiXY);

                for(int l = 0; l < numn; l++) {
                    Vector ev = this.eigenSystem.GetEigenVector(n[l]);

                    for(int i = 0; i < numx; i++)
                        for(int j = 0; j < numy; j++)
                            result[l][i, j] += ev[k] * cache[i, j];
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
        /// Matice s hodnotami vlastních èísel seøazené podle kvantových èísel
        /// </summary>
        /// <param name="n">Poøadí vlastní hodnoty</param>
        public Matrix EigenMatrix(int n) {
            int num = this.eigenSystem.BasisIndex.Length;

            int maxq1 = this.eigenSystem.BasisIndex.BasisQuantumNumberLength(0);
            int maxq2 = this.eigenSystem.BasisIndex.BasisQuantumNumberLength(1);

            Matrix result = new Matrix(maxq1, maxq2);

            for(int i = 0; i < num; i++) {
                int q1 = this.eigenSystem.BasisIndex.GetBasisQuantumNumber(0, i);
                int q2 = this.eigenSystem.BasisIndex.GetBasisQuantumNumber(1, i);

                result[q1, q2] = this.eigenSystem.GetEigenVector(n)[i];
            }

            return result;
        }

        /// <summary>
        /// Souèet složek vektorù s hranièními hodnotami kvantových èísel
        /// </summary>
        public Vector LastEVElementsSumAbs() {
            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++)
                result[i] = this.LastEVElements(i, 0).SumAbs();

            return result;
        }

        /// <summary>
        /// Vektor s posledními hodnotami od konce
        /// </summary>
        /// <param name="index">Poøadí od konce</param>
        /// <param name="n">Poøadí vlastní hodnoty</param>
        public Vector LastEVElements(int n, int index) {
            int num = this.eigenSystem.BasisIndex.Length;

            int maxq1 = this.eigenSystem.BasisIndex.BasisQuantumNumberLength(0);
            int maxq2 = this.eigenSystem.BasisIndex.BasisQuantumNumberLength(1);

            Matrix m = new Matrix(maxq1, maxq2, double.NaN);

            for(int i = 0; i < num; i++) {
                int q1 = this.eigenSystem.BasisIndex.GetBasisQuantumNumber(0, i);
                int q2 = this.eigenSystem.BasisIndex.GetBasisQuantumNumber(1, i);

                m[q1, q2] = this.eigenSystem.GetEigenVector(n)[i];
            }

            Vector result;
            if(maxq1 > maxq2) {
                result = new Vector(maxq1);

                for(int i = 0; i < maxq1; i++) {
                    int k = 0;
                    for(int j = maxq2 - 1; j >= 0; j--)
                        if(!double.IsNaN(m[i, j]) && k++ >= index) {
                            result[i] = m[i, j];
                            break;
                        }
                }
            }
            else {
                result = new Vector(maxq2);

                for(int j = 0; j < maxq2; j++) {
                    int k = 0;
                    for(int i = maxq1 - 1; i >= 0; i--)
                        if(!double.IsNaN(m[i, j]) && k++ >= index) {
                            result[j] = m[i, j];
                            break;
                        }
                }
            }

            return result;
        }

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

            param.Add(this.eigenSystem, "EigenSystem");

            param.Export(export);
		}

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public LHOQuantumGCM(Core.Import import) {
            IEParam param = new IEParam(import);

            if(import.VersionNumber >= 4) {
                this.A = (double)param.Get(-1.0);
                this.B = (double)param.Get(1.0);
                this.C = (double)param.Get(1.0);
                this.K = (double)param.Get(1.0);
                this.A0 = (double)param.Get(1.0);
                this.hbar = (double)param.Get(0.1);

                if(import.VersionNumber < 7) 
                    this.eigenSystem = new EigenSystem(param);
                else
                    this.eigenSystem = (EigenSystem)param.Get();
                this.eigenSystem.SetParrentQuantumSystem(this);

                if(import.VersionNumber < 8){
                    int p = (int)param.Get(0);
                    Vector v = new Vector(1);
                    v[0] = p;
                    this.eigenSystem.SetBasisParams(v);
                    }
            }

            this.RefreshConstants();
        }
        #endregion
    }
}
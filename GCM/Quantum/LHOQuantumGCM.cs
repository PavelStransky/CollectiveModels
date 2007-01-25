using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.GCM {
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

        // Epsilon
        protected const double epsilon = 1E-8;

        // Vlastní hodnoty
        protected Vector eigenValues;

        // Vlastní vektory
        protected Vector[] eigenVectors = new Vector[0];

        // True, pokud bylo vypoèteno
        protected bool isComputed = false;

        // True, pokud již probíhá výpoèet
        protected bool isComputing = false;

        /// <summary>
        /// Planckova konstanta [Js]
        /// </summary>
        public double Hbar { get { return this.hbar; } }

        /// <summary>
        /// Úhlová frekvence LHO [J*m^-2]
        /// </summary>
        public double Omega { get { return System.Math.Sqrt(2.0 * this.a0 / this.K); } }

        /// <summary>
        /// Parametr pro LHO [s^-1]
        /// </summary>
        public double A0 { get { return this.a0; } set { this.a0 = value; } }

        /// <summary>
        /// True, pokud jsou vypoèítané hladiny
        /// </summary>
        public bool IsComputed { get { return this.isComputed; } }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        public LHOQuantumGCM() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        public LHOQuantumGCM(double a, double b, double c, double k, double a0)
            : this(a, b, c, k, a0, 0.1) { }

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

            this.RefreshConstants();
        }

        /// <summary>
        /// Pøepoèítá konstanty
        /// </summary>
        protected virtual void RefreshConstants() {
            this.s = System.Math.Sqrt(this.K * this.Omega / this.hbar);      // xi = s*x (Formanek (2.283))
        }

        /// <summary>
        /// Vrátí velikost Hamiltonovy matice v dané bázi
        /// </summary>
        /// <param name="maxE">Nejvyšší øád bázových funkcí</param>
        public abstract int HamiltonianMatrixSize(int maxE);

        /// <summary>
        /// Napoèítá Hamiltonovu matici v dané bázi
        /// </summary>
        /// <param name="maxE">Nejvyšší øád bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        public abstract Matrix HamiltonianMatrix(int maxE, int numSteps, IOutputWriter writer);

        /// <summary>
        /// Provede výpoèet
        /// </summary>
        /// <param name="maxn">Nejvyšší øád bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="ev">True, pokud budeme poèítat i vlastní vektory</param>
        /// <param name="numev">Poèet vlastních hodnot, menší èi rovné 0 vypoèítá všechny</param>
        /// <param name="writer">Writer</param>
        public virtual void Compute(int maxn, int numSteps, bool ev, int numev, IOutputWriter writer) {
            if(this.isComputing)
                throw new GCMException(errorMessageComputing);
            this.isComputing = true;

            if(numev <= 0 || numev > this.HamiltonianMatrixSize(maxn))
                numev = this.HamiltonianMatrixSize(maxn);

            DateTime startTime = DateTime.Now;

            if(writer != null) {
                writer.WriteLine(string.Format("{0} ({1}): Výpoèet {2} vlastních hodnot{3}.",
                    this.GetType().Name,
                    startTime,
                    numev, 
                    ev ? " a vektorù" : string.Empty));
                writer.Indent(1);
            }

            Matrix h = this.HamiltonianMatrix(maxn, numSteps, writer);

            if(writer != null) {
                writer.WriteLine(string.Format("Stopa matice: {0}", h.Trace()));
                writer.WriteLine(string.Format("Nenulových {0} prvkù z celkových {1}", h.NumNonzeroItems(), h.NumItems()));
            }

            Jacobi jacobi = new Jacobi(h, writer);
            jacobi.SortAsc();

            this.eigenValues = new Vector(jacobi.EigenValue);
            this.eigenValues.Length = numev;

            if(ev) {
                this.eigenVectors = new Vector[numev];
                for(int i = 0; i < numev; i++)
                    this.eigenVectors[i] = jacobi.EigenVector[i];
            }
            else
                this.eigenVectors = new Vector[0];

            if(writer != null) {
                writer.WriteLine(string.Format("Souèet vlastních èísel: {0}", this.eigenValues.Sum()));
                writer.Indent(-1);
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));
            }

            this.isComputed = true;
            this.isComputing = false;
        }

        /// <summary>
        /// Vlastní hodnoty
        /// </summary>
        public Vector GetEigenValues() {
            return this.eigenValues;
        }

        /// <summary>
        /// Vlastní vektor
        /// </summary>
        /// <param name="i">Index vektoru</param>
        public Vector GetEigenVector(int i) {
            return this.eigenVectors[i];
        }

        /// <summary>
        /// Poèet vlastních vektorù
        /// </summary>
        public int NumEV { get { return this.eigenVectors.Length; } }

        /// <summary>
        /// Vrátí matici hustot pro vlastní funkce
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="interval">Rozmìry v jednotlivých smìrech (uspoøádané ve tvaru [minx, maxx,] numx, ...)</param>
        public abstract Matrix DensityMatrix(int n, params Vector[] interval);

        #region Implementace IExportable
        /// <summary>
        /// Pøidá další parametry pro uložení
        /// </summary>
        protected virtual void Export(IEParam param) { }

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
            param.Add(this.isComputed, "IsComputed");

            if(this.isComputed) {
                param.Add(this.eigenValues, "EigenValues");

                int numEV = this.NumEV;
                param.Add(numEV, "EigenVector Number");

                for(int i = 0; i < numEV; i++)
                    param.Add(this.eigenVectors[i]);
            }

            this.Export(param);

            param.Export(export);
		}

        /// <summary>
        /// Naètení dalších parametrù
        /// </summary>
        protected virtual void Import(IEParam param) { }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public void Import(Import import) {
            IEParam param = new IEParam(import);

            if(import.VersionNumber >= 4) {
                this.A = (double)param.Get(-1.0);
                this.B = (double)param.Get(1.0);
                this.C = (double)param.Get(1.0);
                this.K = (double)param.Get(1.0);
                this.A0 = (double)param.Get(1.0);
                this.hbar = (double)param.Get(0.1);
                this.isComputed = (bool)param.Get(false);

                if(this.isComputed) {
                    this.eigenValues = (Vector)param.Get(null);

                    int numEV = (int)param.Get(0);
                    this.eigenVectors = new Vector[numEV];

                    for(int i = 0; i < numEV; i++)
                        this.eigenVectors[i] = (Vector)param.Get();
                }

                this.Import(param);
            }

            this.RefreshConstants();
        }
        #endregion

        protected const string errorMessageNotComputed = "Energetické spektrum ještì nebylo vypoèteno.";
        protected const string errorMessageComputing = "Nad daným objektem LHOQuantumGCM již probíhá výpoèet.";
    }
}
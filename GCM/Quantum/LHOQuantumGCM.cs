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

        protected Jacobi jacobi;

        // True, pokud bylo vypoèteno
        protected bool isComputed = false;

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
        /// <param name="writer">Writer</param>
        public void Compute(int maxn, int numSteps, IOutputWriter writer) {
            DateTime startTime = DateTime.Now;
            Matrix h = this.HamiltonianMatrix(maxn, numSteps, writer);

            if(writer != null) {
                writer.WriteLine((DateTime.Now - startTime).ToString());
                writer.WriteLine(string.Format("Stopa matice: {0}", h.Trace()));
                writer.WriteLine(string.Format("Nenulových {0} prvkù z celkových {1}", h.NumNonzeroItems(), h.NumItems()));
            }

            this.jacobi = new Jacobi(h, writer);
            this.jacobi.SortAsc();

            if(writer != null) {
                writer.WriteLine(string.Format("Souèet vlastních èísel: {0}", new Vector(this.jacobi.EigenValue).Sum()));
            }

            this.isComputed = true;
        }

        /// <summary>
        /// Vlastní hodnoty
        /// </summary>
        public double[] EigenValue { get { return this.isComputed ? this.jacobi.EigenValue : null; } }

        /// <summary>
        /// Vlastní vektory
        /// </summary>
        public Vector[] EigenVector { get { return this.isComputed ? this.jacobi.EigenVector : null; } }

        /// <summary>
        /// Vrátí matici hustot pro vlastní funkce
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="interval">Rozmìry v jednotlivých smìrech (uspoøádané ve tvaru [minx, maxx,] numx, ...)</param>
        public abstract Matrix DensityMatrix(int n, params Vector[] interval);

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.A, "A");
            param.Add(this.B, "B");
            param.Add(this.C, "C");
            param.Add(this.K, "K");
            param.Add(this.A0, "A0");
            param.Add(this.Hbar, "HBar");
            param.Add(this.jacobi, "Jacobi");

            param.Export(export);
		}

		/// <summary>
		/// Naète výsledky ze souboru
		/// </summary>
        /// <param name="import">Import</param>
        public virtual void Import(Import import) {
            IEParam param = new IEParam(import);

            this.A = (double)param.Get(-1.0);
            this.B = (double)param.Get(1.0);
            this.C = (double)param.Get(1.0);
            this.K = (double)param.Get(1.0);
            this.A0 = (double)param.Get(1.0);
            this.hbar = (double)param.Get(0.1);
            this.jacobi = (Jacobi)param.Get();

            this.RefreshConstants();
        }
        #endregion

        protected const string errorMessageNotComputed = "Energetické spektrum ještì nebylo vypoèteno.";
    }
}
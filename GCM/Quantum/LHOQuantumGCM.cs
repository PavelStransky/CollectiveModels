using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantov� GCM 
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

        // Vlastn� hodnoty
        protected Vector eigenValues;

        // Vlastn� vektory
        protected Vector[] eigenVectors = new Vector[0];

        // True, pokud bylo vypo�teno
        protected bool isComputed = false;

        // True, pokud ji� prob�h� v�po�et
        protected bool isComputing = false;

        /// <summary>
        /// Planckova konstanta [Js]
        /// </summary>
        public double Hbar { get { return this.hbar; } }

        /// <summary>
        /// �hlov� frekvence LHO [J*m^-2]
        /// </summary>
        public double Omega { get { return System.Math.Sqrt(2.0 * this.a0 / this.K); } }

        /// <summary>
        /// Parametr pro LHO [s^-1]
        /// </summary>
        public double A0 { get { return this.a0; } set { this.a0 = value; } }

        /// <summary>
        /// True, pokud jsou vypo��tan� hladiny
        /// </summary>
        public bool IsComputed { get { return this.isComputed; } }

        /// <summary>
        /// Pr�zdn� konstruktor
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
        /// P�epo��t� konstanty
        /// </summary>
        protected virtual void RefreshConstants() {
            this.s = System.Math.Sqrt(this.K * this.Omega / this.hbar);      // xi = s*x (Formanek (2.283))
        }

        /// <summary>
        /// Vr�t� velikost Hamiltonovy matice v dan� b�zi
        /// </summary>
        /// <param name="maxE">Nejvy��� ��d b�zov�ch funkc�</param>
        public abstract int HamiltonianMatrixSize(int maxE);

        /// <summary>
        /// Napo��t� Hamiltonovu matici v dan� b�zi
        /// </summary>
        /// <param name="maxE">Nejvy��� ��d b�zov�ch funkc�</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="writer">Writer</param>
        public abstract Matrix HamiltonianMatrix(int maxE, int numSteps, IOutputWriter writer);

        /// <summary>
        /// Provede v�po�et
        /// </summary>
        /// <param name="maxn">Nejvy��� ��d b�zov�ch funkc�</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="ev">True, pokud budeme po��tat i vlastn� vektory</param>
        /// <param name="numev">Po�et vlastn�ch hodnot, men�� �i rovn� 0 vypo��t� v�echny</param>
        /// <param name="writer">Writer</param>
        public virtual void Compute(int maxn, int numSteps, bool ev, int numev, IOutputWriter writer) {
            if(this.isComputing)
                throw new GCMException(errorMessageComputing);
            this.isComputing = true;

            if(numev <= 0 || numev > this.HamiltonianMatrixSize(maxn))
                numev = this.HamiltonianMatrixSize(maxn);

            DateTime startTime = DateTime.Now;

            if(writer != null) {
                writer.WriteLine(string.Format("{0} ({1}): V�po�et {2} vlastn�ch hodnot{3}.",
                    this.GetType().Name,
                    startTime,
                    numev, 
                    ev ? " a vektor�" : string.Empty));
                writer.Indent(1);
            }

            Matrix h = this.HamiltonianMatrix(maxn, numSteps, writer);

            if(writer != null) {
                writer.WriteLine(string.Format("Stopa matice: {0}", h.Trace()));
                writer.WriteLine(string.Format("Nenulov�ch {0} prvk� z celkov�ch {1}", h.NumNonzeroItems(), h.NumItems()));
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
                writer.WriteLine(string.Format("Sou�et vlastn�ch ��sel: {0}", this.eigenValues.Sum()));
                writer.Indent(-1);
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));
            }

            this.isComputed = true;
            this.isComputing = false;
        }

        /// <summary>
        /// Vlastn� hodnoty
        /// </summary>
        public Vector GetEigenValues() {
            return this.eigenValues;
        }

        /// <summary>
        /// Vlastn� vektor
        /// </summary>
        /// <param name="i">Index vektoru</param>
        public Vector GetEigenVector(int i) {
            return this.eigenVectors[i];
        }

        /// <summary>
        /// Po�et vlastn�ch vektor�
        /// </summary>
        public int NumEV { get { return this.eigenVectors.Length; } }

        /// <summary>
        /// Vr�t� matici hustot pro vlastn� funkce
        /// </summary>
        /// <param name="n">Index vlastn� funkce</param>
        /// <param name="interval">Rozm�ry v jednotliv�ch sm�rech (uspo��dan� ve tvaru [minx, maxx,] numx, ...)</param>
        public abstract Matrix DensityMatrix(int n, params Vector[] interval);

        #region Implementace IExportable
        /// <summary>
        /// P�id� dal�� parametry pro ulo�en�
        /// </summary>
        protected virtual void Export(IEParam param) { }

        /// <summary>
        /// Ulo�� v�sledky do souboru
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
        /// Na�ten� dal��ch parametr�
        /// </summary>
        protected virtual void Import(IEParam param) { }

        /// <summary>
        /// Na�te v�sledky ze souboru
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

        protected const string errorMessageNotComputed = "Energetick� spektrum je�t� nebylo vypo�teno.";
        protected const string errorMessageComputing = "Nad dan�m objektem LHOQuantumGCM ji� prob�h� v�po�et.";
    }
}
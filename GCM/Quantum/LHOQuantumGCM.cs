using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantový GCM 
    /// </summary>
    public abstract class LHOQuantumGCM : GCM, IExportable, IQuantumSystem {
        /// <summary>
        /// Metoda výpoètu
        /// </summary>
        public enum ComputeMethod { Jacobi, LAPACKBand }
        
        // Parametr pro LHO
        private double a0;
 
        // Planckova konstanta
        private double hbar;                    // [Js]

        // Koeficienty
        protected double s;
        protected double omega;

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
        public double Omega { get { return this.omega; } }

        /// <summary>
        /// Parametr pro LHO [s^-1]
        /// </summary>
        public double A0 { get { return this.a0; } set { this.a0 = value; } }

        /// <summary>
        /// True, pokud jsou vypoèítané hladiny
        /// </summary>
        public bool IsComputed { get { return this.isComputed; } }

        /// <summary>
        /// První kvantové èíslo v poøadí od 0 s krokem 1
        /// </summary>
        protected abstract int GetBasisQuantumNumber1(int i);

        /// <summary>
        /// Druhé kvantové èíslo v poøadí od 0 s krokem 1
        /// </summary>
        protected abstract int GetBasisQuantumNumber2(int i);

        /// <summary>
        /// Poèet stavù báze
        /// </summary>
        protected abstract int GetBasisLength();

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

        /// <summary>
        /// Vrátí velikost Hamiltonovy matice v dané bázi
        /// </summary>
        /// <param name="maxE">Nejvyšší øád bázových funkcí</param>
        public virtual int HamiltonianMatrixSize(int maxE) {
            return this.HamiltonianMatrix(maxE, 0, null).Length;
        }

        /// <summary>
        /// Stopa Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Nejvyšší øád bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        public virtual double HamiltonianMatrixTrace(int maxE, int numSteps, IOutputWriter writer) {
            return this.HamiltonianMatrix(maxE, numSteps, writer).Trace();
        }

        /// <summary>
        /// Napoèítá Hamiltonovu matici v dané bázi
        /// </summary>
        /// <param name="maxE">Nejvyšší øád bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        public virtual Matrix HamiltonianMatrix(int maxE, int numSteps, IOutputWriter writer) {
            return (Matrix)this.HamiltonianSBMatrix(maxE, numSteps, writer);
        }

        /// <summary>
        /// Napoèítá Hamiltonovu matici jako pásovou matici
        /// </summary>
        /// <param name="maxE">Nejvyšší øád bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        protected virtual SymmetricBandMatrix HamiltonianSBMatrix(int maxE, int numSteps, IOutputWriter writer) {
            throw new GCMException(string.Format(Messages.EMNotImplemented, "SymmetricBandMatrix", this.GetType().Name));
        }

        /// <summary>
        /// Provede výpoèet (diagonalizaci)
        /// </summary>
        /// <param name="maxE">Nejvyšší energie bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="ev">True, pokud budeme poèítat i vlastní vektory</param>
        /// <param name="numev">Poèet vlastních hodnot, menší èi rovné 0 vypoèítá všechny</param>
        /// <param name="writer">Writer</param>
        /// <param name="method">Metoda výpoètu</param>
        public void Compute(int maxE, int numSteps, bool ev, int numev, IOutputWriter writer, ComputeMethod method) {
            if(this.isComputing)
                throw new GCMException(Messages.EMComputing);

            this.isComputing = true;

            try {
                if(numev <= 0 || numev > this.HamiltonianMatrixSize(maxE))
                    numev = this.HamiltonianMatrixSize(maxE);

                DateTime startTime = DateTime.Now;

                if(writer != null) {
                    writer.WriteLine(string.Format("{0} ({1}): Výpoèet {2} vlastních hodnot{3}.",
                        this.GetType().Name,
                        startTime,
                        numev,
                        ev ? " a vektorù" : string.Empty));
                    writer.Indent(1);
                }

                if(method == ComputeMethod.Jacobi) {
                    Matrix h = this.HamiltonianMatrix(maxE, numSteps, writer);

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
                }

                else if(method == ComputeMethod.LAPACKBand) {
                    SymmetricBandMatrix m = this.HamiltonianSBMatrix(maxE, numSteps, writer);

                    if(writer != null) {
                        writer.WriteLine(string.Format("Stopa matice: {0}", m.Trace()));
                        writer.Write("Diagonalizace dsbevx...");
                    }

                    GC.Collect();

                    DateTime startTime1 = DateTime.Now;

                    Vector[] eigenSystem = LAPackDLL.dsbevx(m, ev, 0, numev);
                    m.Dispose();

                    GC.Collect();

                    if(writer != null)
                        writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime1));

                    this.eigenValues = eigenSystem[0];
                    this.eigenValues.Length = numev;

                    if(ev) {
                        this.eigenVectors = new Vector[numev];
                        for(int i = 0; i < numev; i++)
                            this.eigenVectors[i] = eigenSystem[i + 1];
                    }
                    else
                        this.eigenVectors = new Vector[0];
                }

                if(writer != null) {
                    writer.WriteLine(string.Format("Souèet vlastních èísel: {0}", this.eigenValues.Sum()));
                    writer.Indent(-1);
                    writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));
                }

                this.isComputed = true;
            }
            finally {
                this.isComputing = false;
            }
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
        /// Vlnová funkce v souøadnice x, y
        /// </summary>
        /// <param name="x">Souøadnice x</param>
        /// <param name="y">Souøadnice y</param>
        /// <param name="n">Index vlnové funkce</param>
        protected abstract double PsiXY(double x, double y, int n);

        /// <summary>
        /// Vrátí matici <n|V|n> amplitudy vlastní funkce n
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="rx">Rozmìry ve smìru x</param>
        /// <param name="ry">Rozmìry ve smìru y</param>
        public virtual Matrix AmplitudeMatrix(int n, DiscreteInterval intx, DiscreteInterval inty) {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed);

            Vector ev = this.eigenVectors[n];

            int numx = intx.Num;
            int numy = inty.Num;

            Matrix result = new Matrix(numx, numy);

            int length = this.GetBasisLength();

            for(int k = 0; k < length; k++) {
                BasisCache2D cache = new BasisCache2D(intx, inty, k, this.PsiXY);

                for(int i = 0; i < numx; i++)
                    for(int j = 0; j < numy; j++)
                        result[i, j] += ev[i] * cache[i, j];
            }

            return result;
        }

        /// <summary>
        /// Vrátí matici hustot pro vlastní funkce
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="interval">Rozmìry v jednotlivých smìrech (uspoøádané ve tvaru [minx, maxx,] numx, ...)</param>
        public virtual Matrix DensityMatrix(int n, params Vector[] interval) {
            DiscreteInterval intx = new DiscreteInterval(interval[0]);
            DiscreteInterval inty = new DiscreteInterval(interval[1]);

            Matrix result = this.AmplitudeMatrix(n, intx, inty);

            int numx = result.LengthX;
            int numy = result.LengthY;

            for(int i = 0; i < numx; i++)
                for(int j = 0; j < numy; j++)
                    result[i, j] *= result[i, j];

            return result;
        }

        /// <summary>
        /// Matice s hodnotami vlastních èísel seøazené podle kvantových èísel
        /// </summary>
        /// <param name="n">Poøadí vlastní hodnoty</param>
        public Matrix EigenMatrix(int n) {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed); 
            
            int num = this.GetBasisLength();

            int maxq1 = this.GetBasisQuantumNumber1(-1);
            int maxq2 = this.GetBasisQuantumNumber2(-1);

            Matrix result = new Matrix(maxq1, maxq2);

            for(int i = 0; i < num; i++) {
                int q1 = this.GetBasisQuantumNumber1(i);
                int q2 = this.GetBasisQuantumNumber2(i);

                result[q1, q2] = this.eigenVectors[n][i];
            }

            return result;
        }

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
        public LHOQuantumGCM(Core.Import import) {
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
    }
}
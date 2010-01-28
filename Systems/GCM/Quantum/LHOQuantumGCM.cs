using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Systems;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    public enum PeresInvariantTypes {
        L2 = 0, 
        HPrime = 1, 
        HOscillator = 2
    }

    /// <summary>
    /// Kvantov� GCM 
    /// </summary>
    public abstract class LHOQuantumGCM : GCM, IExportable, IQuantumSystem {
        /// <summary>
        /// Metoda v�po�tu
        /// </summary>
        public enum ComputeMethod { Jacobi, LAPACKBand }
        
        // Parametr pro LHO
        private double a0;
 
        // Planckova konstanta
        private double hbar;                    // [Js]

        // Koeficienty
        protected double s;
        protected double omega;

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
        public double Omega { get { return this.omega; } }

        /// <summary>
        /// Parametr pro LHO [s^-1]
        /// </summary>
        public double A0 { get { return this.a0; } set { this.a0 = value; } }

        /// <summary>
        /// True, pokud jsou vypo��tan� hladiny
        /// </summary>
        public bool IsComputed { get { return this.isComputed; } }

        /// <summary>
        /// Prvn� kvantov� ��slo v po�ad� od 0 s krokem 1
        /// </summary>
        protected abstract int GetBasisQuantumNumber1(int i);

        /// <summary>
        /// Druh� kvantov� ��slo v po�ad� od 0 s krokem 1
        /// </summary>
        protected abstract int GetBasisQuantumNumber2(int i);

        /// <summary>
        /// Po�et stav� b�ze
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
        /// P�epo��t� konstanty
        /// </summary>
        protected virtual void RefreshConstants() {
            this.omega = System.Math.Sqrt(2.0 * this.a0 / this.K);
            this.s = System.Math.Sqrt(this.K * this.omega / this.hbar);      // xi = s*x (Formanek (2.283))
        }

        /// <summary>
        /// Vr�t� velikost Hamiltonovy matice v dan� b�zi
        /// </summary>
        /// <param name="maxE">Nejvy��� ��d b�zov�ch funkc�</param>
        public virtual int HamiltonianMatrixSize(int maxE) {
            return this.HamiltonianMatrix(maxE, 0, null).Length;
        }

        /// <summary>
        /// Stopa Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Nejvy��� ��d b�zov�ch funkc�</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="writer">Writer</param>
        public virtual double HamiltonianMatrixTrace(int maxE, int numSteps, IOutputWriter writer) {
            return this.HamiltonianMatrix(maxE, numSteps, writer).Trace();
        }

        /// <summary>
        /// Napo��t� Hamiltonovu matici v dan� b�zi
        /// </summary>
        /// <param name="maxE">Nejvy��� ��d b�zov�ch funkc�</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="writer">Writer</param>
        public virtual Matrix HamiltonianMatrix(int maxE, int numSteps, IOutputWriter writer) {
            return (Matrix)this.HamiltonianSBMatrix(maxE, numSteps, writer);
        }

        /// <summary>
        /// Napo��t� Hamiltonovu matici jako p�sovou matici
        /// </summary>
        /// <param name="maxE">Nejvy��� ��d b�zov�ch funkc�</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="writer">Writer</param>
        protected virtual SymmetricBandMatrix HamiltonianSBMatrix(int maxE, int numSteps, IOutputWriter writer) {
            throw new GCMException(string.Format(Messages.EMNotImplemented, "SymmetricBandMatrix", this.GetType().Name));
        }

        /// <summary>
        /// Provede v�po�et (diagonalizaci)
        /// </summary>
        /// <param name="maxE">Nejvy��� energie b�zov�ch funkc�</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="ev">True, pokud budeme po��tat i vlastn� vektory</param>
        /// <param name="numev">Po�et vlastn�ch hodnot, men�� �i rovn� 0 vypo��t� v�echny</param>
        /// <param name="writer">Writer</param>
        /// <param name="method">Metoda v�po�tu</param>
        public void Compute(int maxE, int numSteps, bool ev, int numev, IOutputWriter writer, ComputeMethod method) {
            if(this.isComputing)
                throw new GCMException(Messages.EMComputing);

            this.isComputing = true;

            try {
                if(numev <= 0 || numev > this.HamiltonianMatrixSize(maxE))
                    numev = this.HamiltonianMatrixSize(maxE);

                DateTime startTime = DateTime.Now;

                if(writer != null) {
                    writer.WriteLine(string.Format("{0} ({1}): V�po�et {2} vlastn�ch hodnot{3}.",
                        this.GetType().Name,
                        startTime,
                        numev,
                        ev ? " a vektor�" : string.Empty));
                    writer.Indent(1);
                }

                if(method == ComputeMethod.Jacobi) {
                    Matrix h = this.HamiltonianMatrix(maxE, numSteps, writer);

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
                    writer.WriteLine(string.Format("Sou�et vlastn�ch ��sel: {0}", this.eigenValues.Sum()));
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
        /// Druh� invariant
        /// </summary>
        /// <param name="type">Typ Peresova oper�toru</param>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        public virtual Vector GetPeresInvariant(PeresInvariantTypes type) {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed);

            if(type == PeresInvariantTypes.L2) {
                return this.GetPeresInvariantL2();
            }

            else if(type == PeresInvariantTypes.HPrime) {
                return this.GetPeresInvariantHPrime();
            }

            else if(type == PeresInvariantTypes.HOscillator) {
                return this.GetPeresInvariantHOscillator();
            }

            return null;
        }

        /// <summary>
        /// Koeficient pro v�po�et druh�ho invariantu
        /// </summary>
        /// <param name="j">Index k vlnov� funkci</param>
        protected abstract double PeresInvariantCoef(int j);

        /// <summary>
        /// Peres�v oper�tor L2
        /// </summary>
        protected virtual Vector GetPeresInvariantL2() {
            int count = this.eigenVectors.Length;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenVectors[i];
                int length = ev.Length;

                for(int j = 0; j < length; j++)
                    result[i] += ev[j] * ev[j] * this.PeresInvariantCoef(j);
            }

            return result;
        }

        /// <summary>
        /// Peres�v oper�tor H'
        /// </summary>
        protected virtual Vector GetPeresInvariantHPrime() {
            return null;
        }

        /// <summary>
        /// Peres�v oper�tor H v oscil�torov� b�zi
        /// </summary>
        protected virtual Vector GetPeresInvariantHOscillator() {
            return null;
        }

        /// <summary>
        /// Entropie syst�mu S = -Sum(ev[j]^2 ln(ev[j]^2))
        /// </summary>
        /// <remarks>M.S. Santhanam et al., arXiv:chao-dyn/9704002v1</remarks>
        public virtual Vector GetEntropy() {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed);

            int count = this.eigenVectors.Length;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenVectors[i];
                int length = ev.Length;

                for(int j = 0; j < length; j++)
                    if(ev[j] != 0.0)
                        result[i] -= ev[j] * ev[j] * System.Math.Log(ev[j] * ev[j]);
            }

            return result;
        }

        #region Vlnov� funkce
        /// <summary>
        /// Vlnov� funkce v sou�adnic�ch x, y
        /// </summary>
        /// <param name="x">Sou�adnice x</param>
        /// <param name="y">Sou�adnice y</param>
        /// <param name="n">Index vlnov� funkce</param>
        protected virtual double PsiXY(double x, double y, int n) {
            double beta = System.Math.Sqrt(x * x + y * y);
            double gamma = (x > 0 ? System.Math.Atan(y / x) : System.Math.PI - System.Math.Atan(y / x));

            return this.PsiBG(beta, gamma, n);
        }

        /// <summary>
        /// Vlnov� funkce v sou�adnic�ch beta, gamma
        /// </summary>
        /// <param name="beta">Sou�adnice beta</param>
        /// <param name="gamma">Sou�adnice gamma</param>
        /// <param name="n">Index vlnov� funkce</param>
        protected virtual double PsiBG(double beta, double gamma, int n) {
            return 0.0;
        }

        /// <summary>
        /// Radi�ln� ��st 2D vlnov� funkce
        /// </summary>
        /// <param name="n">Hlavn� kvantov� ��slo</param>
        /// <param name="m">�hlov� kvantov� ��slo</param>
        /// <param name="x">Sou�adnice</param>
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
        /// �hlov� ��st vlnov� 2D funkce (lich�)
        /// </summary>
        /// <param name="gamma">Sou�adnice gamma</param>
        /// <param name="m">�hlov� kvantov� ��slo</param>
        /// <returns></returns>
        public double Phi2DO(double gamma, int m) {
            return System.Math.Sin(m * gamma) * norm;
        }

        /// <summary>
        /// �hlov� ��st vlnov� 2D funkce (sud�)
        /// </summary>
        /// <param name="gamma">Sou�adnice gamma</param>
        /// <param name="m">�hlov� kvantov� ��slo</param>
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
        /// Vr�t� matici <n|V|n> amplitudy vlastn� funkce n
        /// </summary>
        /// <param name="n">Index vlastn� funkce</param>
        /// <param name="rx">Rozm�ry ve sm�ru x</param>
        /// <param name="ry">Rozm�ry ve sm�ru y</param>
        public virtual Matrix[] AmplitudeMatrix(int[] n, IOutputWriter writer, DiscreteInterval intx, DiscreteInterval inty) {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed);

            int numx = intx.Num;
            int numy = inty.Num;

            int numn = n.Length;

            Matrix[] result = new Matrix[numn];
            for(int i = 0; i < numn; i++)
                result[i] = new Matrix(numx, numy);

            int length = this.GetBasisLength();
            int length100 = length / 100;

            DateTime startTime = DateTime.Now;

            for(int k = 0; k < length; k++) {
                BasisCache2D cache = new BasisCache2D(intx, inty, k, this.PsiXY);

                for(int l = 0; l < numn; l++) {
                    Vector ev = this.eigenVectors[n[l]];

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
        /// Vr�t� matici hustot pro vlastn� funkce
        /// </summary>
        /// <param name="n">Index vlastn� funkce</param>
        /// <param name="interval">Rozm�ry v jednotliv�ch sm�rech (uspo��dan� ve tvaru [minx, maxx,] numx, ...)</param>
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
        /// Vr�t� hustotu vlnov� funkce v dan�m bod�
        /// </summary>
        /// <param name="n">Index vlastn� funkce</param>
        /// <param name="x">Bod</param>
        public double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] x) {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed); 
            
            Vector ev = this.eigenVectors[n];
            int length = this.GetBasisLength();

            double result = 0;
            for(int k = 0; k < length; k++)
                result += ev[k] * this.PsiXY(x[0], x[1], k);

            return result;
        }

        /// <summary>
        /// Matice s hodnotami vlastn�ch ��sel se�azen� podle kvantov�ch ��sel
        /// </summary>
        /// <param name="n">Po�ad� vlastn� hodnoty</param>
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

        /// <summary>
        /// Sou�et slo�ek vektor� s hrani�n�mi hodnotami kvantov�ch ��sel
        /// </summary>
        public Vector LastEVElementsSumAbs() {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed);

            int count = this.eigenVectors.Length;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++)
                result[i] = this.LastEVElements(i, 0).SumAbs();

            return result;
        }

        /// <summary>
        /// Vektor s posledn�mi hodnotami od konce
        /// </summary>
        /// <param name="index">Po�ad� od konce</param>
        /// <param name="n">Po�ad� vlastn� hodnoty</param>
        public Vector LastEVElements(int n, int index) {
            if(!this.isComputed)
                throw new GCMException(Messages.EMNotComputed);

            int num = this.GetBasisLength();

            int maxq1 = this.GetBasisQuantumNumber1(-1);
            int maxq2 = this.GetBasisQuantumNumber2(-1);

            Matrix m = new Matrix(maxq1, maxq2, double.NaN);

            for(int i = 0; i < num; i++) {
                int q1 = this.GetBasisQuantumNumber1(i);
                int q2 = this.GetBasisQuantumNumber2(i);

                m[q1, q2] = this.eigenVectors[n][i];
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
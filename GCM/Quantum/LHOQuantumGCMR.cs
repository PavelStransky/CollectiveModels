using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantov� GCM v b�zi 2D line�rn�ho harmonick�ho oscil�toru
    /// </summary>
    public class LHOQuantumGCMR : LHOQuantumGCM {
        // Indexy b�ze
        protected LHOPolarIndex index;

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        public LHOQuantumGCMR() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        public LHOQuantumGCMR(double a, double b, double c, double k, double a0)
            : base(a, b, c, k, a0) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMR(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        /// <summary>
        /// Vytvo�� instanci t��dy LHOPolarIndex
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie</param>
        protected virtual void CreateIndex(int maxE) {
            if(this.index == null || this.index.MaxE != maxE)
                this.index = new LHOPolarIndex(maxE);
        }

        /// <summary>
        /// Velikost Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie</param>
        public override int HamiltonianMatrixSize(int maxE) {
            this.CreateIndex(maxE);
            return this.index.Length;
        }

        /// <summary>
        /// Stopa Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Nejvy��� energie v n�sobc�ch hbar * Omega</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="writer">Writer</param>
        public override double HamiltonianMatrixTrace(int maxE, int numSteps, IOutputWriter writer) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Napo��t� Hamiltonovu matici v dan� b�zi
        /// </summary>
        /// <param name="maxE">Nejvy��� energie v n�sobc�ch hbar * Omega</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="writer">Writer</param>
        public override Matrix HamiltonianMatrix(int maxE, int numSteps, IOutputWriter writer) {
            this.CreateIndex(maxE);

            if(numSteps == 0)
                numSteps = 10 * this.index.MaxM + 1;

            if(writer != null) {
                writer.WriteLine(string.Format("Maxim�ln� (n, m) = ({0}, {1})", this.index.MaxN, this.index.MaxM));
                writer.WriteLine(string.Format("Velikost b�ze: {0}", this.index.Length));
                writer.WriteLine(string.Format("P�ipravuji cache ({0})...", numSteps));
            }

            double omega = this.Omega;
            double range = this.GetRange(epsilon);

            // Cache psi hodnot (Bazove vlnove funkce)
            BasisCache psiCache = new BasisCache(new DiscreteInterval(0.0, range, numSteps), this.index.Length, this.Psi);

            int[] psiCacheLowerLimits = psiCache.GetLowerLimits(epsilon);
            int[] psiCacheUpperLimits = psiCache.GetUpperLimits(epsilon);

            double step = psiCache.Step;

            // Cache hodnot potencialu
            double[] vCache1 = new double[numSteps];
            double[] vCache2 = new double[numSteps];
            for(int sb = 0; sb < numSteps; sb++) {
                double beta = psiCache.GetX(sb);
                double beta2 = beta * beta;
                vCache1[sb] = beta2 * beta * ((this.A - this.A0) + this.C * beta2);
                vCache2[sb] = 0.5 * this.B * beta2 * beta2;
            }

            int length = this.index.Length;
            Matrix m = new Matrix(length);

            if(writer != null)
                writer.WriteLine(string.Format("P��prava H ({0} x {1})", length, length));

            for(int i = 0; i < length; i++) {
                int ni = this.index.N[i];
                int mi = this.index.M[i];

                for(int j = i; j < length; j++) {
                    int nj = this.index.N[j];
                    int mj = this.index.M[j];

                    // V�b�rov� pravidlo
                    if(mi != mj && System.Math.Abs(mi - mj) != 3)
                        continue;

                    double sum = 0;

                    double[] vCache = vCache2;
                    if(mi == mj)
                        vCache = vCache1;

                    for(int sb = 0; sb < numSteps; sb++)
                        sum += psiCache[i, sb] * vCache[sb] * psiCache[j, sb];

                    sum *= step;

                    if(mi == mj && ni == nj)
                        sum += this.Hbar * omega * (1.0 + ni + ni + System.Math.Abs(mi));

                    m[i, j] = sum;
                    m[j, i] = sum;
                }

                // V�pis te�ky na konzoli
                if(writer != null) {
                    if(i != 0 && this.index.M[i - 1] != this.index.M[i])
                        writer.Write(".");
                }
            }

            return m;
        }

        /// <summary>
        /// Vr�t� matici <n|V|n> vlastn� funkce n
        /// </summary>
        /// <param name="n">Index vlastn� funkce</param>
        /// <param name="rx">Rozm�ry ve sm�ru x</param>
        /// <param name="ry">Rozm�ry ve sm�ru y</param>
        private Complex[,] EigenMatrix(int n, DiscreteInterval intx, DiscreteInterval inty) {
            Vector ev = this.eigenVectors[n];

            Complex[,] result = new Complex[intx.Num, inty.Num];
            for(int sx = 0; sx < intx.Num; sx++)
                for(int sy = 0; sy < inty.Num; sy++)
                    result[sx, sy] = new Complex();

            DiscreteInterval intr = new DiscreteInterval(0,
                System.Math.Max(System.Math.Max(System.Math.Abs(intx.Min), System.Math.Abs(intx.Max)),
                    System.Math.Max(System.Math.Abs(inty.Min), System.Math.Abs(inty.Max))), 
                intx.Num + inty.Num);

            int length = this.index.Length;

            BasisCache cache = new BasisCache(intr, length, this.Psi);

            for(int i = 0; i < length; i++) {
                int ni = this.index.N[i];
                int mi = this.index.M[i];

                for(int sx = 0; sx < intx.Num; sx++) {
                    double x = intx.GetX(sx);
                    for(int sy = 0; sy < inty.Num; sy++) {
                        double y = inty.GetX(sy);
                        double beta = System.Math.Sqrt(x * x + y * y);
                        double gamma = (x > 0 ? System.Math.Atan(y / x) : System.Math.PI - System.Math.Atan(y / x));
                        result[sx, sy] += cache.GetValue(i, beta) * ev[i] * Complex.Exp(Complex.I * mi * gamma);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Zkontroluje, zda je range �pln� a p��padn� dopln�
        /// </summary>
        /// <param name="range">Vstupn� rozm�ry</param>
        /// <returns>V�stupn� rozm�ry</returns>
        private DiscreteInterval ParseRange(Vector range) {
            if((object)range == null || range.Length == 0)
                return new DiscreteInterval(this.GetRange(epsilon), 10 * this.index.MaxM + 1);

            if(range.Length == 1)
                return new DiscreteInterval(this.GetRange(epsilon), (int)range[0]);

            if(range.Length == 2)
                return new DiscreteInterval(range[0], (int)range[1]);

            return new DiscreteInterval(range);
        }

        /// <summary>
        /// Vrac� parametr range podle dosahu nejvy��� pou�it� vlastn� funkce
        /// </summary>
        /// <param name="epsilon">Epsilon</param>
        /// <param name="maxn">Maxim�ln� rank vlastn� funkce</param>
        protected double GetRange(double epsilon) {
            // range je klasicky dosah oscilatoru, pridame urcitou rezervu
            double range = System.Math.Sqrt(this.Hbar * this.Omega * this.index.MaxE / this.A0);
            range *= 5.0;

            // dx musi byt nekolikrat mensi, nez vzdalenost mezi sousednimi nody
            double dx = range / (50.0 * this.index.MaxM);

            while(System.Math.Abs(this.Psi(this.index.MaxN, this.index.MaxM, range)) < epsilon)
                range -= dx;

            //jedno dx, abysme se dostali tam, co to bylo male a druhe jako rezerva
            return range + 2 * dx;
        }
        
        /// <summary>
        /// Vr�t� matici hustot pro vlastn� funkce
        /// </summary>
        /// <param name="n">Index vlastn� funkce</param>
        /// <param name="interval">Rozm�ry v jednotliv�ch sm�rech (uspo��dan� ve tvaru [minx, maxx,] numx, ...)</param>
        public override Matrix DensityMatrix(int n, params Vector[] interval) {
            if(!this.isComputed)
                throw new GCMException(errorMessageNotComputed);

            DiscreteInterval intx = this.ParseRange(interval.Length > 0 ? interval[0] : null);
            DiscreteInterval inty = this.ParseRange(interval.Length > 1 ? interval[1] : null);

            Complex[,] m = this.EigenMatrix(n, intx, inty);
            Matrix result = new Matrix(m.GetLength(0), m.GetLength(1));

            for(int sx = 0; sx < intx.Num; sx++)
                for(int sy = 0; sy < inty.Num; sy++)
                    result[sx, sy] = m[sx, sy].SquaredNorm;

            return result;
        }
               
        /// <summary>
        /// Radi�ln� ��st vlnov� funkce
        /// </summary>
        /// <param name="n">Hlavn� kvantov� ��slo</param>
        /// <param name="m">Spin</param>
        /// <param name="x">Sou�adnice</param>
        protected double Psi(int n, int m, double x) {
            double xi2 = this.s * x; xi2 *= xi2;
            m = System.Math.Abs(m);

            double normLog = 0.5 * (System.Math.Log(2.0) + SpecialFunctions.FactorialILog(n) - SpecialFunctions.FactorialILog(n + m)) + (m + 1) * System.Math.Log(this.s);
            double l = 0.0;
            double e = 0.0;
            SpecialFunctions.Laguerre(out l, out e, n, m, xi2);

            if(l == 0.0 || x == 0.0)
                return 0.0;

            double lLog = System.Math.Log(System.Math.Abs(l));
            double result = normLog + m * System.Math.Log(x) - xi2 / 2.0 + lLog + e;
            result = l < 0.0 ? -System.Math.Exp(result) : System.Math.Exp(result);

            return result;
        }

        /// <summary>
        /// Radi�ln� ��st vlnov� funkce
        /// </summary>
        /// <param name="i">Index (kvantov� ��sla zjist�me podle uchovan� cache index�)</param>
        /// <param name="x">Sou�adnice</param>
        protected double Psi(int i, double x) {
            return this.Psi(this.index.N[i], this.index.M[i], x);
        }

        protected override void Export(IEParam param) {
            if(this.isComputed)
                param.Add(this.index.MaxE, "Maximum Energy of Basis Functions");
        }

        protected override void Import(IEParam param) {
            if(this.isComputed)
                this.CreateIndex((int)param.Get(10));
        }
    }
}
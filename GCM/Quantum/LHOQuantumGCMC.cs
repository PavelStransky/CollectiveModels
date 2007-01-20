using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantov� GCM v b�zi 2D line�rn�ho harmonick�ho oscil�toru
    /// </summary>
    public class LHOQuantumGCMC : LHOQuantumGCM {
        private double n;

        private HermitPolynom hermit;

        /// <summary>
        /// Maxim�ln� hlavn� kvantov� ��slo
        /// </summary>
        public int MaxN { get { return this.IsComputed ? this.hermit.MaxN : 0; } }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        public LHOQuantumGCMC() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        public LHOQuantumGCMC(double a, double b, double c, double k, double a0)
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
        public LHOQuantumGCMC(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) {
        }

        /// <summary>
        /// P�epo��t� konstanty s
        /// </summary>        
        protected override void RefreshConstants() {
            base.RefreshConstants();
            this.n = System.Math.Sqrt(System.Math.Sqrt(this.K * this.Omega / (System.Math.PI * this.Hbar)));
                                                                // sqrt(sqrt(M*omega/(pi*hbar))) (Formanek (2.286))
        }

        /// <summary>
        /// Napo��t� Hamiltonovu matici v b�zi LHO 1Dx1D oscil�toru
        /// </summary>
        /// <param name="maxn">Nejvy��� ��d b�zov�ch funkc�</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="writer">Writer</param>        
        public override Matrix HamiltonianMatrix(int maxn, int numSteps, IOutputWriter writer) {
            if(numSteps == 0)
                numSteps = 10 * maxn + 1;

            if(writer != null)
                writer.WriteLine(string.Format("P�ipravuji cache ({0} x {1})...", numSteps, numSteps));

            // Hermit�v polynom
            this.hermit = new HermitPolynom(maxn);
            double omega = this.Omega;
            double range = this.GetRange(epsilon);

            // Cache psi hodnot (Bazove vlnove funkce)
            BasisCache psiCache = new BasisCache(new DiscreteInterval(range, numSteps), maxn, this.Psi);
            int[] psiCacheLowerLimits = psiCache.GetLowerLimits(epsilon);
            int[] psiCacheUpperLimits = psiCache.GetUpperLimits(epsilon);

            double step = psiCache.Step;

            // Cache hodnot potencialu
            double[,] vCache = new double[numSteps, numSteps];
            for(int sx = 0; sx < numSteps; sx++) {
                double x = psiCache.GetX(sx);
                for(int sy = 0; sy < numSteps; sy++) {
                    double y = psiCache.GetX(sy);
                    // Origin�ln� potenci�l - potenci�l b�ze
                    vCache[sx, sy] = this.V(x, y) - this.A0 * (x * x + y * y);
                }
            }

            int max2 = maxn * maxn;
            Matrix m = new Matrix(max2, max2);

            if(writer != null)
                writer.WriteLine(string.Format("P��prava H ({0} x {1})", max2, max2));

            DateTime startTime = DateTime.Now;

            for(int i = 0; i < max2; i++) {
                for(int j = i; j < max2; j++) {
                    int ix = i / maxn;
                    int iy = i % maxn;
                    int jx = j / maxn;
                    int jy = j % maxn;

                    double sum = 0;

                    int minsx = System.Math.Max(psiCacheLowerLimits[ix], psiCacheLowerLimits[jx]);
                    int maxsx = System.Math.Min(psiCacheUpperLimits[ix], psiCacheUpperLimits[jx]);

                    int minsy = System.Math.Max(psiCacheLowerLimits[iy], psiCacheLowerLimits[jy]);
                    int maxsy = System.Math.Min(psiCacheUpperLimits[iy], psiCacheUpperLimits[jy]);

                    for(int sy = minsy; sy < maxsy; sy++)
                        for(int sx = minsx; sx < maxsx; sx++)
                            sum += psiCache[ix, sx] * psiCache[iy, sy] * vCache[sx, sy] *
                                psiCache[jx, sx] * psiCache[jy, sy];

                    sum *= step * step;

                    if(ix == jx && iy == jy)
                        sum += this.Hbar * omega * (1.0 + ix + iy);

                    m[i, j] = sum;
                    m[j, i] = sum;
                }

                // V�pis te�ky na konzoli
                if(writer != null) {
                    if(i % maxn == 0) {
                        if(i != 0)
                            writer.WriteLine((DateTime.Now - startTime).ToString());

                        writer.Write(i / maxn);
                        startTime = DateTime.Now;
                    }

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
        private Matrix EigenMatrix(int n, DiscreteInterval intx, DiscreteInterval inty) {
            Vector ev = jacobi.EigenVector[n];
            Matrix result = new Matrix(intx.Num, inty.Num);

            int sqrlength = ev.Length;
            int length = (int)System.Math.Round(System.Math.Sqrt(sqrlength));

            BasisCache cachex = new BasisCache(intx, this.MaxN, this.Psi);
            BasisCache cachey = new BasisCache(inty, this.MaxN, this.Psi);

            for(int i = 0; i < sqrlength; i++) {
                int ix = i / length;
                int iy = i % length;

                for(int sx = 0; sx < intx.Num; sx++)
                    for(int sy = 0; sy < inty.Num; sy++)
                        result[sx, sy] += cachex[ix, sx] * cachey[iy, sy] * ev[i];
            }

            return result;
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

            Matrix result = this.EigenMatrix(n, intx, inty);

            for(int sx = 0; sx < intx.Num; sx++)
                for(int sy = 0; sy < inty.Num; sy++)
                    result[sx, sy] = result[sx, sy] * result[sx, sy];

            // Zakreslen� ekvipotenci�ln� kontury
            if(interval.Length > 2) {
                double maxAbs = System.Math.Abs(result.MaxAbs());

                PointVector[] pv = this.EquipotentialContours(this.EigenValue[n]);
                for(int i = 0; i < pv.Length; i++) {
                    int pvlength = pv[i].Length;
                    for(int j = 0; j < pvlength; j++) {
                        int sx = intx.GetIndex(pv[i][j].X);
                        int sy = inty.GetIndex(pv[i][j].Y);

                        if(sx >= 0 && sx < intx.Num && sy >= 0 && sy < inty.Num)
                            result[sx, sy] = -maxAbs;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Diference |H|psi> - E|psi>|^2 pro zadanou vlastn� funkci
        /// </summary>
        /// <param name="n">Index vlastn� funkce</param>
        /// <param name="interval">Rozm�ry v jednotliv�ch sm�rech (uspo��dan� ve tvaru [minx, maxx,] numx, ...)</param>
        public Matrix NumericalDiff(int n, params Vector[] interval) {
            if(!this.isComputed)
                throw new GCMException(errorMessageNotComputed);

            DiscreteInterval intx = this.ParseRange(interval.Length > 0 ? interval[0] : null);
            DiscreteInterval inty = this.ParseRange(interval.Length > 1 ? interval[1] : null);

            Matrix em = this.EigenMatrix(n, intx, inty);
            Matrix result = new Matrix(intx.Num, inty.Num);

            for(int sx = 1; sx < intx.Num - 1; sx++) {
                double x = intx.GetX(sx);
                for(int sy = 1; sy < inty.Num - 1; sy++) {
                    double y = inty.GetX(sy);

                    double laplace = (em[sx + 1, sy] + em[sx - 1, sy] + em[sx, sy - 1] + em[sx, sy + 1]
                        - 4.0 * em[sx, sy]) / (intx.Step * inty.Step);

                    //laplace = 1 / (dx * dx) * (
                    //    -(M[i + 2, j] + M[i - 2, j] + M[i, j + 2] + M[i, j - 2]) / 12 +
                    //    4 * (M[i + 1, j] + M[i - 1, j] + M[i, j + 1] + M[i, j - 1]) / 3 - 5 * M[i, j]
                    //    ); //O(dx^4)

                    result[sx, sy] = -this.Hbar * this.Hbar / (2 * this.K) * laplace + em[sx, sy] * this.V(x, y) - em[sx, sy] * this.jacobi.EigenValue[n];
                    result[sx, sy] *= result[sx, sy];
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
                return new DiscreteInterval(this.GetRange(epsilon), 10 * this.MaxN + 1);

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
        private double GetRange(double epsilon) {
            // range je klasicky dosah oscilatoru, pridame urcitou rezervu
            double range = System.Math.Sqrt(this.Hbar * this.Omega * (this.MaxN + 0.5) / this.A0);
            range *= 5.0;

            // dx musi byt nekolikrat mensi, nez vzdalenost mezi sousednimi nody
            double dx = range / (50.0 * this.MaxN);

            while(System.Math.Abs(this.Psi(this.MaxN, range)) < epsilon)
                range -= dx;

            //jedno dx, abysme se dostali tam, co to bylo male a druhe jako rezerva
            return range + 2 * dx;
        }

        /// <summary>
        /// Funkce Psi_n (Form�nek 2.286)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Psi(int n, double x) {
            double xi = this.s * x;
            return this.n / System.Math.Sqrt(SpecialFunctions.Factorial(n) * System.Math.Pow(2, n)) * hermit.GetValue(n, xi) * System.Math.Exp(-xi * xi / 2);
        }

        /// <summary>
        /// Provede import
        /// </summary>
        /// <param name="import">Import funkc�</param>
        public override void Import(Import import) {
            base.Import(import);

            if(this.jacobi == null) {
                this.hermit = null;
                this.isComputed = false;
            }
            else {
                this.hermit = new HermitPolynom((int)System.Math.Sqrt(this.jacobi.EigenValue.Length));
                this.isComputed = true;
            }
        }
    }
}
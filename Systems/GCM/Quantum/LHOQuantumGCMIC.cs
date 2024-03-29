using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    /// <summary>
    /// Kvantov� GCM v b�zi 2D line�rn�ho harmonick�ho oscil�toru
    /// </summary>
    public class LHOQuantumGCMIC : LHOQuantumGCMI {
        /// <summary>
        /// Konstruktor pro IE
        /// </summary>
        /// <param name="import"></param>
        public LHOQuantumGCMIC(Core.Import import) : base(import) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMIC(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) {
        }

        protected override int MaximalNumNodes { get { return (this.eigenSystem.BasisIndex as LHOCartesianIndex).MaxN; } }
        protected override double MaximalRange { get { return System.Math.Sqrt(this.Hbar * this.Omega * ((this.eigenSystem.BasisIndex as LHOCartesianIndex).MaxN + 0.5) / this.A0); } }
        protected override double PsiRange(double range) {
            return this.Psi(range, (this.eigenSystem.BasisIndex as LHOCartesianIndex).MaxN);
        }

        /// <summary>
        /// Vytvo�� instanci t��dy s parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public override BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LHOCartesianIndex(basisParams);
        }

        /// <summary>
        /// Napo��t� Hamiltonovu matici v b�zi LHO 1Dx1D oscil�toru
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        public override void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            LHOCartesianIndex index = basisIndex as LHOCartesianIndex;
            int maxn = index.MaxN;
            int numSteps = index.NumSteps;

            DateTime startTime = DateTime.Now;

            if(writer != null) {
                writer.WriteLine("Hamiltonova matice");
                writer.Indent(1);
                writer.WriteLine(string.Format("P�ipravuji cache ({0} x {1})...", numSteps, numSteps));
            }

            // Hermit�v polynom
            double range = this.GetRange();

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

            if(writer != null)
                writer.WriteLine(string.Format("P��prava H ({0} x {1})", max2, max2));

            DateTime startTime1 = DateTime.Now;

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
                        sum += this.Hbar * this.omega * (1.0 + ix + iy);

                    matrix[i, j] = sum;
                    matrix[j, i] = sum;
                }

                // V�pis te�ky na konzoli
                if(writer != null) {
                    if(i % maxn == 0) {
                        if(i != 0)
                            writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime1));

                        writer.Write(i / maxn);
                        startTime = DateTime.Now;
                    }

                    writer.Write(".");
                }
            }

            if(writer != null){
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime1));
                writer.Indent(-1);
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));
            }
        }

        /// <summary>
        /// Vr�t� matici <n|V|n> vlastn� funkce n
        /// </summary>
        /// <param name="n">Index vlastn� funkce</param>
        /// <param name="rx">Rozm�ry ve sm�ru x</param>
        /// <param name="ry">Rozm�ry ve sm�ru y</param>
        public override Matrix[] AmplitudeMatrix(int[] n, IOutputWriter writer, DiscreteInterval intx, DiscreteInterval inty) {
            LHOCartesianIndex index = this.eigenSystem.BasisIndex as LHOCartesianIndex;

            int numn = n.Length;
            int numx = intx.Num;
            int numy = inty.Num;

            Matrix[] result = new Matrix[numn];

            for(int i = 0; i < numn; i++)
                result[i] = new Matrix(intx.Num, inty.Num);

            int sqrlength = index.Length;
            int length = (int)System.Math.Round(System.Math.Sqrt(sqrlength));

            BasisCache cachex = new BasisCache(intx, index.MaxN, this.Psi);
            BasisCache cachey = new BasisCache(inty, index.MaxN, this.Psi);

            for(int i = 0; i < sqrlength; i++) {
                int ix = i / length;
                int iy = i % length;

                for(int j = 0; j < numn; j++) {
                    Vector ev = this.eigenSystem.GetEigenVector(n[j]);

                    for(int sx = 0; sx < intx.Num; sx++)
                        for(int sy = 0; sy < inty.Num; sy++)
                            result[j][sx, sy] += cachex[ix, sx] * cachey[iy, sy] * ev[i];
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
            if(!this.eigenSystem.IsComputed)
                throw new SystemsException(Messages.EMNotComputed);

            DiscreteInterval intx = this.ParseRange(interval.Length > 0 ? interval[0] : null);
            DiscreteInterval inty = this.ParseRange(interval.Length > 1 ? interval[1] : null);

            int[] nn = new int[1];
            nn[0] = n;

            Matrix em = this.AmplitudeMatrix(nn, null, intx, inty)[0];
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

                    result[sx, sy] = -this.Hbar * this.Hbar / (2 * this.K) * laplace + em[sx, sy] * this.V(x, y) - em[sx, sy] * ((Vector)this.eigenSystem.GetEigenValues())[n];
                    result[sx, sy] *= result[sx, sy];
                }
            }

            return result;
        }

        /// <summary>
        /// Funkce Psi_n (Form�nek 2.286)
        /// </summary>
        /// <param name="s">Koeficient normalizace</param>
        /// <param name="n">Hlavn� kvantov� ��slo</param>
        /// <param name="x">Hodnota polynomu</param>
        /// <returns></returns>
        public double Psi(double x, int n) {
            double xi = this.s * x;
            return n / System.Math.Sqrt(System.Math.PI / this.s * SpecialFunctions.Factorial(n) * System.Math.Pow(2, n)) * SpecialFunctions.Hermite(xi, n) * System.Math.Exp(-xi * xi / 2);
        }

        /// <summary>
        /// Funkce Psi ve dvou rozm�rech
        /// </summary>
        protected override double PsiXY(double x, double y, int n) {
            return this.Psi(x, n) * this.Psi(y, n);
        }

        protected override double PeresInvariantCoef(int j) {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}

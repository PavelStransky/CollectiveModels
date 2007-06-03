using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantový GCM v bázi 2D lineárního harmonického oscilátoru
    /// </summary>
    public class LHOQuantumGCMC : LHOQuantumGCM {
        private double n;

        private HermitPolynom hermit;

        /// <summary>
        /// Maximální hlavní kvantové èíslo
        /// </summary>
        public int MaxN { get { return this.IsComputed ? this.hermit.MaxN : 0; } }

        /// <summary>
        /// Prázdný konstruktor
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
        /// Pøepoèítá konstanty s
        /// </summary>        
        protected override void RefreshConstants() {
            base.RefreshConstants();
            this.n = System.Math.Sqrt(System.Math.Sqrt(this.K * this.Omega / (System.Math.PI * this.Hbar)));
                                                                // sqrt(sqrt(M*omega/(pi*hbar))) (Formanek (2.286))
        }

        /// <summary>
        /// Velikost Hamiltonovy matice
        /// </summary>
        /// <param name="maxn">Nejvyšší øád bázových funkcí</param>
        public override int HamiltonianMatrixSize(int maxn) {
            return maxn * maxn;
        }

        /// <summary>
        /// Stopa Hamiltonovy matice
        /// </summary>
        /// <param name="maxE">Nejvyšší øád bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        public override double HamiltonianMatrixTrace(int maxE, int numSteps, IOutputWriter writer) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Napoèítá Hamiltonovu matici v bázi LHO 1Dx1D oscilátoru
        /// </summary>
        /// <param name="maxn">Nejvyšší øád bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>        
        public override Matrix HamiltonianMatrix(int maxn, int numSteps, IOutputWriter writer) {
            if(numSteps == 0)
                numSteps = 10 * maxn + 1;

            DateTime startTime = DateTime.Now;

            if(writer != null) {
                writer.WriteLine("Hamiltonova matice");
                writer.Indent(1);
                writer.WriteLine(string.Format("Pøipravuji cache ({0} x {1})...", numSteps, numSteps));
            }

            // Hermitùv polynom
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
                    // Originální potenciál - potenciál báze
                    vCache[sx, sy] = this.V(x, y) - this.A0 * (x * x + y * y);
                }
            }

            int max2 = maxn * maxn;
            Matrix m = new Matrix(max2, max2);

            if(writer != null)
                writer.WriteLine(string.Format("Pøíprava H ({0} x {1})", max2, max2));

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
                        sum += this.Hbar * omega * (1.0 + ix + iy);

                    m[i, j] = sum;
                    m[j, i] = sum;
                }

                // Výpis teèky na konzoli
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

            return m;
        }

        /// <summary>
        /// Vrátí matici <n|V|n> vlastní funkce n
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="rx">Rozmìry ve smìru x</param>
        /// <param name="ry">Rozmìry ve smìru y</param>
        private Matrix EigenMatrix(int n, DiscreteInterval intx, DiscreteInterval inty) {
            Vector ev = this.eigenVectors[n];
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
        /// Vrátí matici hustot pro vlastní funkce
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="interval">Rozmìry v jednotlivých smìrech (uspoøádané ve tvaru [minx, maxx,] numx, ...)</param>
        public override Matrix DensityMatrix(int n, params Vector[] interval) {
            if(!this.isComputed)
                throw new GCMException(errorMessageNotComputed);

            DiscreteInterval intx = this.ParseRange(interval.Length > 0 ? interval[0] : null);
            DiscreteInterval inty = this.ParseRange(interval.Length > 1 ? interval[1] : null);

            Matrix result = this.EigenMatrix(n, intx, inty);

            for(int sx = 0; sx < intx.Num; sx++)
                for(int sy = 0; sy < inty.Num; sy++)
                    result[sx, sy] = result[sx, sy] * result[sx, sy];

            return result;
        }

        /// <summary>
        /// Diference |H|psi> - E|psi>|^2 pro zadanou vlastní funkci
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="interval">Rozmìry v jednotlivých smìrech (uspoøádané ve tvaru [minx, maxx,] numx, ...)</param>
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

                    result[sx, sy] = -this.Hbar * this.Hbar / (2 * this.K) * laplace + em[sx, sy] * this.V(x, y) - em[sx, sy] * this.eigenValues[n];
                    result[sx, sy] *= result[sx, sy];
                }
            }

            return result;
        }

        /// <summary>
        /// Zkontroluje, zda je range úplný a pøípadnì doplní
        /// </summary>
        /// <param name="range">Vstupní rozmìry</param>
        /// <returns>Výstupní rozmìry</returns>
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
        /// Vrací parametr range podle dosahu nejvyšší použité vlastní funkce
        /// </summary>
        /// <param name="epsilon">Epsilon</param>
        /// <param name="maxn">Maximální rank vlastní funkce</param>
        private double GetRange(double epsilon) {
            // range je klasicky dosah oscilatoru, pridame urcitou rezervu
            double range = System.Math.Sqrt(this.Hbar * this.Omega * (this.MaxN + 0.5) / this.A0);
            range *= 5.0;

            // dx musi byt nekolikrat mensi, nez vzdalenost mezi sousednimi nody
            double dx = range / (50.0 * this.MaxN);

            while(System.Math.Abs(this.Psi(range, this.MaxN)) < epsilon)
                range -= dx;

            //jedno dx, abysme se dostali tam, co to bylo male a druhe jako rezerva
            return range + 2 * dx;
        }

        /// <summary>
        /// Funkce Psi_n (Formánek 2.286)
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Psi(double x, int n) {
            double xi = this.s * x;
            return this.n / System.Math.Sqrt(SpecialFunctions.Factorial(n) * System.Math.Pow(2, n)) * this.hermit.GetValue(n, xi) * System.Math.Exp(-xi * xi / 2);
        }

        protected override void Export(IEParam param) {
            if(this.isComputed)
                param.Add(this.hermit.MaxN, "Order of Hermit Polynom");
        }

        protected override void Import(IEParam param) {
            if(this.isComputed)
                this.hermit = new HermitPolynom((int)param.Get(10));
        }
    }
}

using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.DLLWrapper;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantový GCM v bázi 2D lineárního harmonického oscilátoru
    /// pomocí knihovny LAPACK
    /// </summary>
    public class LHOQuantumGCMRL : LHOQuantumGCMR {
        private double[] eigenValue;

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        public LHOQuantumGCMRL() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        public LHOQuantumGCMRL(double a, double b, double c, double k, double a0)
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
        public LHOQuantumGCMRL(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        /// <summary>
        /// Vypoèítá Hamiltonovu matici
        /// </summary>
        /// <param name="maxE">Nejvyšší energie bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        public override Matrix HamiltonianMatrix(int maxE, int numSteps, IOutputWriter writer) {
            return (Matrix)this.HamiltonianSBMatrix(maxE, numSteps, writer);
        }

        /// <summary>
        /// Vypoèítá Hamiltonovu matici do tvaru pásové matice
        /// </summary>
        /// <param name="maxE">Nejvyšší energie bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        private SymmetricBandMatrix HamiltonianSBMatrix(int maxE, int numSteps, IOutputWriter writer) {
            this.CreateIndex(maxE);

            if(numSteps == 0)
                numSteps = 10 * this.index.MaxM + 1;

            if(writer != null) {
                writer.WriteLine(string.Format("Maximální (n, m) = ({0}, {1})", this.index.MaxN, this.index.MaxM));
                writer.WriteLine(string.Format("Velikost báze: {0}", this.index.Length));
                writer.WriteLine(string.Format("Pøipravuji cache ({0})...", numSteps));
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
            SymmetricBandMatrix m = new SymmetricBandMatrix(length, maxE - 2);

            if(writer != null)
                writer.WriteLine(string.Format("Pøíprava pásové matice H ({0} x {1})", length, maxE - 2));

            for(int i = 0; i < length; i++) {
                int ni = this.index.N[i];
                int mi = this.index.M[i];

                for(int j = i; j < length; j++) {
                    int nj = this.index.N[j];
                    int mj = this.index.M[j];

                    // Výbìrové pravidlo
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

                    // Již je symetrické
                    m[i, j] = sum;
                }

                // Výpis teèky na konzoli
                if(writer != null) {
                    if(i != 0 && this.index.M[i - 1] != this.index.M[i])
                        writer.Write(".");
                }
            }

            return m;
        }

        /// <summary>
        /// Provede výpoèet
        /// </summary>
        /// <param name="maxE">Nejvyšší energie bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        public override void Compute(int maxE, int numSteps, IOutputWriter writer) {
            DateTime startTime = DateTime.Now;

            SymmetricBandMatrix m = this.HamiltonianSBMatrix(maxE, numSteps, writer);

            if(writer != null) {
                writer.WriteLine((DateTime.Now - startTime).ToString());
            }

            startTime = DateTime.Now;

            this.eigenValue = LAPackDLL.dsbevx(m, 0, 1000);

            if(writer != null) {
                writer.WriteLine((DateTime.Now - startTime).ToString());
                writer.WriteLine(string.Format("Souèet vlastních èísel: {0}", new Vector(this.eigenValue).Sum()));
            }

            this.isComputed = true;

            m.Dispose();
        }

        /// <summary>
        /// Vlastní hodnoty
        /// </summary>
        public override double[] EigenValue { get { return this.eigenValue; } }

        /// <summary>
        /// Vlastní vektory (NEFUNGUJE !!!)
        /// </summary>
        public override Vector[] EigenVector { get { return new Vector[this.eigenValue.Length]; } }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public override void Import(Import import) {
            base.Import(import);

            if(this.jacobi == null) {
                this.index = null;
                this.isComputed = false;
            }
            else {
                int length = this.jacobi.EigenValue.Length;
                for(int i = (int)System.Math.Sqrt(length) / 2; i < length; i++) {
                    this.index = new LHOPolarIndex(i);
                    if(this.index.Length == length)
                        break;
                }
                this.isComputed = true;
            }

            this.RefreshConstants();
        }
    }
}
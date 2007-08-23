using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.DLLWrapper;
using PavelStransky.Core;

namespace PavelStransky.GCM {
    /// <summary>
    /// Kvantový GCM v bázi 2D lineárního harmonického oscilátoru
    /// pomocí knihovny LAPACK a jen pro sudé stavy
    /// </summary>
    public class LHOQuantumGCMRLE: LHOQuantumGCMRL {
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        public LHOQuantumGCMRLE() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        public LHOQuantumGCMRLE(double a, double b, double c, double k, double a0)
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
        public LHOQuantumGCMRLE(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        /// <summary>
        /// Vytvoøí instanci tøídy LHOPolarIndex
        /// </summary>
        /// <param name="maxE">Maximální energie</param>
        protected override void CreateIndex(int maxE) {
            if(this.index == null || this.index.MaxE != maxE)
                this.index = new LHOPolarIndex(maxE, false, 2);
        }

        /// <summary>
        /// Vypoèítá Hamiltonovu matici do tvaru pásové matice
        /// </summary>
        /// <param name="maxE">Nejvyšší energie bázových funkcí</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="writer">Writer</param>
        /// <param name="trace">Calculates only trace of the matrix</param>
        protected override SymmetricBandMatrix HamiltonianSBMatrix(int maxE, int numSteps, IOutputWriter writer, bool trace) {
            this.CreateIndex(maxE);

            if(numSteps == 0)
                numSteps = 10 * this.index.MaxM + 1;

            DateTime startTime = DateTime.Now;

            if(writer != null) {
                writer.WriteLine("Hamiltonova matice");
                writer.Indent(1);
                writer.WriteLine(string.Format("Pøipravuji cache potenciálu ({0})...", numSteps));
            }

            double omega = this.Omega;
            double range = this.GetRange(epsilon);

            // Cache psi hodnot (Bazove vlnove funkce)
            DiscreteInterval interval = new DiscreteInterval(0.0, range, numSteps);

            double step = interval.Step;

            // Cache hodnot potencialu
            double[] vCache1 = new double[numSteps];
            double[] vCache2 = new double[numSteps];
            for(int sb = 0; sb < numSteps; sb++) {
                double beta = interval.GetX(sb);
                double beta2 = beta * beta;
                vCache1[sb] = beta2 * beta * ((this.A - this.A0) + this.C * beta2);
                vCache2[sb] = 0.5 * this.B * beta2 * beta2;
            }

            int length = this.index.Length;
            int bandWidth = maxE - 2;
            SymmetricBandMatrix m = new SymmetricBandMatrix(length, bandWidth);

            int blockSize = bandWidth + 1;
            int blockNum = length / blockSize + 1;

            if(writer != null) {
                writer.WriteLine(string.Format("Pøíprava pásové matice H ({0} x {1})", length, maxE - 2));
                writer.WriteLine(string.Format("Poèet blokù: {0}, velikost bloku: {1}", blockNum, blockSize));
            }

            DateTime startTime1 = DateTime.Now;

            BasisCache cache2 = new BasisCache(interval, 0, System.Math.Min(blockSize, this.index.Length), this.Psi);
            BasisCache cache1 = cache2;

            for(int k = 0; k < blockNum; k++) {
                int i0 = k * blockSize;
                int i1 = System.Math.Min((k + 1) * blockSize, this.index.Length);
                int i2 = System.Math.Min((k + 2) * blockSize, this.index.Length);

                if(writer != null) {
                    writer.Write(k);
                    writer.Write(" D");
                }

                // Diagonální blok
                for(int i = i0; i < i1; i++) {
                    int ni = this.index.N[i];
                    int mi = this.index.M[i];

                    for(int j = i; j < i1; j++) {
                        int nj = this.index.N[j];
                        int mj = this.index.M[j];

                        // Výbìrové pravidlo
                        if(mi != mj && System.Math.Abs(mi - mj) != 3)
                            continue;

                        if(trace && i != j)
                            continue;

                        double sum = 0;

                        double[] vCache = vCache2;
                        if(mi == mj)
                            vCache = vCache1;

                        for(int sb = 0; sb < numSteps; sb++)
                            sum += cache1[i, sb] * vCache[sb] * cache1[j, sb];

                        sum *= step;

                        if(mi == mj && ni == nj)
                            sum += this.Hbar * omega * (1.0 + ni + ni + System.Math.Abs(mi));

                        if(mi == 0 && mj == 0)
                            sum *= 2.0;
                        else if(mi == 0 || mj == 0)
                            sum *= 1.6;

                        // Již je symetrické
                        m[i, j] = sum;
                    }
                }

                if(writer != null)
                    writer.Write('C');

                cache2 = new BasisCache(interval, i1, i2, this.Psi);

                if(!trace) {
                    if(writer != null)
                        writer.Write("N ");

                    // Nediagonální blok
                    for(int i = i0; i < i1; i++) {
                        int ni = this.index.N[i];
                        int mi = this.index.M[i];

                        for(int j = i1; j < i2; j++) {
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
                                sum += cache1[i, sb] * vCache[sb] * cache2[j, sb];

                            sum *= step;

                            if(mi == 0 && mj == 0)
                                sum *= 2.0;

                            else if(mi == 0 || mj == 0)
                                sum *= 1.6;

                            // Již je symetrické
                            m[i, j] = sum;
                        }
                    }

                    cache1 = cache2;
                }

                if(writer != null) {
                    writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime1));
                    startTime1 = DateTime.Now;
                }
            }

            if(writer != null) {
                writer.Indent(-1);
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));
            }

            return m;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Systems {
    /// <summary>
    /// Poèítá radiální maticové elementy potøebné pro výpoèet Hamiltonovy a kvadrupólové matice
    /// (metoda výpoètu popsána v èlánku)
    /// </summary>
    public class RadialMatrixElement {
        // Radiální maticové elementy mezi stavy s radiálními kvantovými èísly i1, i2
        // a beta ^ i3, pokud 0 < i3 < 5
        private double[, ,] radan;

        // Kinetické èleny
        private double[,] radank;

        /// <summary>
        /// Konstruktor (provede výpoèet)
        /// </summary>
        /// <param name="minBeta">Minimální exponenciála beta</param>
        /// <param name="maxBeta">Maximální exponenciála beta</param>
        public RadialMatrixElement(int minBeta, int maxBeta) {
            GammaLog gammaLog = new GammaLog();

            this.radan = new double[MaxDim, MaxDim, maxBeta + 1];
            this.radank = new double[MaxDim, MaxDim];

            // i1, i2 jsou radiální indexy ketu na levé, resp. pravé stranì
            // Matice je symetrická, staèí tudíž poèítat pouze polovinu
            for(int irho = minBeta; irho <= maxBeta; irho++)
                for(int i1 = 0; i1 < MaxDim; i1++)
                    for(int i2 = 0; i2 <= i1; i2++) {
                        // Pomocné promìnné
                        int ihlp1 = (int)(System.Math.Sqrt(i1) + System.Math.Sqrt(i1 + 1));
                        int ihlp2 = i1 + 1 - (ihlp1 % 2 == 0 ? (ihlp1 * ihlp1) / 4 : (ihlp1 * ihlp1 - 1) / 4);

                        int ihlp1s = (int)(System.Math.Sqrt(i2) + System.Math.Sqrt(i2 + 1));
                        int ihlp2s = i2 + 1 - (ihlp1s % 2 == 0 ? (ihlp1s * ihlp1s) / 4 : (ihlp1s * ihlp1s - 1) / 4);

                        // Poèet fononù
                        int nue = ihlp1 - 1;
                        int nues = ihlp1s - 1;

                        // Lambda
                        int lambda = (int)(2 * (ihlp2 - 1) + ((ihlp1 - 1) % 2));
                        int lambdas = (int)(2 * (ihlp2s - 1) + ((ihlp1s - 1) % 2));

                        // Poèet párù fononù vázaných na nulu na pravé (levé) stranì
                        int notp = (nue - lambda) / 2;
                        int notps = (int)(nues - lambdas) / 2;

                        // Výbìrová pravidla (podle èlánku)
                        int idn = System.Math.Abs(nue - nues);
                        int idl = System.Math.Abs(lambda - lambdas);

                        if(idl > irho) continue;
                        if(idn > irho) continue;
                        if(idn % 2 != idl % 2) continue;
                        if((irho % 2 == 0) && (idn % 2 != 0)) continue;
                        if((irho % 2 != 0) && (idn % 2 == 0)) continue;
                        if(irho == 4 && idl != 0) continue;
                        if(irho == 5 && idl == 5) continue;

                        // Meze sèítání, dvì možnosti
                        if(System.Math.Abs(lambda - lambdas) <= irho) {
                            // Horní a dolní mez sèítání
                            int imin = 0;
                            int imax = System.Math.Min(notp, notps);

                            if((irho + lambdas - lambda) % 2 == 0)
                                imin = System.Math.Max(
                                    System.Math.Max(notp - (irho + lambdas - lambda) / 2, notps - (irho + lambda - lambdas) / 2),
                                    0);

                            double prefactor = (gammaLog[2 * (notp + 1)] +
                                gammaLog[2 * (notps + 1)] -
                                gammaLog[2 * (notp + lambda) + 5] -
                                gammaLog[2 * (notps + lambdas) + 5]) / 2.0;

                            // Pomocná promìnná
                            int def1 = (irho + lambdas - lambda) / 2;
                            int def2 = (irho + lambda - lambdas) / 2;

                            for(int isigma = imin; isigma <= imax; isigma++) {
                                int sign1 = this.SignGamma(isigma + def1 - notp + 1);
                                int sign2 = this.SignGamma(isigma + def2 - notps + 1);

                                double sum = gammaLog[2 * isigma + (irho + lambda + lambdas + 5)] -
                                    gammaLog[2 * (isigma + 1)] -
                                    gammaLog[2 * (-isigma + notp + 1)] -
                                    gammaLog[2 * (-isigma + notps + 1)] -
                                    gammaLog[2 * (isigma - notp + 1 + def1)] -
                                    gammaLog[2 * (isigma - notps + 1 + def2)] +
                                    gammaLog[2 * (def1 + 1)] +
                                    gammaLog[2 * (def2 + 1)] +
                                    prefactor;

                                this.radan[i1, i2, irho] += sign1 * sign2 * System.Math.Exp(sum) * ((notp + notps) % 2 == 0 ? 1 : -1);
                                // Console.WriteLine("radan({0}, {1}, {2}) = {3}", i1, i2, irho, this.radan[i1, i2, irho]);
                            }
                        }
                        else {
                            if(lambda > lambdas) {
                                int l = lambda; lambda = lambdas; lambdas = l;
                                int n = notp; notp = notps; notps = n;
                                n = nue; nue = (int)nues; nues = n;
                            }

                            // Horní a dolní mez sèítání
                            int imin = 0;
                            int imax = System.Math.Min(notp, notps);

                            if((irho + lambdas - lambda) % 2 == 0)
                                imin = System.Math.Max(notp - (irho + lambdas - lambda) / 2, 0);

                            double prefactor = (gammaLog[2 * (notp + 1)] +
                                gammaLog[2 * (notps + 1)] -
                                gammaLog[2 * (notp + lambda) + 5] -
                                gammaLog[2 * (notps + lambdas) + 5]) / 2.0;

                            // Pomocná promìnná
                            int def1 = (irho + lambdas - lambda) / 2;
                            int def2 = (irho + lambda - lambdas) / 2;

                            for(int isigma = imin; isigma <= imax; isigma++) {
                                int sign1 = this.SignGamma(notps - def2 - isigma);
                                int sign2 = this.SignGamma(isigma - def1 - notp + 1);

                                double sum = gammaLog[2 * isigma + (irho + lambda + lambdas + 5)] -
                                    gammaLog[2 * (isigma + 1)] -
                                    gammaLog[2 * (-isigma + notp + 1)] -
                                    gammaLog[2 * (-isigma + notps + 1)] -
                                    gammaLog[2 * (isigma - notp + 1 + def1)] +
                                    gammaLog[2 * (-isigma + notps - def2)] +
                                    gammaLog[2 * (def1 + 1)] -
                                    gammaLog[2 * (-def2)] +
                                    prefactor;

                                this.radan[i1, i2, irho] += sign1 * sign2 * System.Math.Exp(sum) * ((notp + isigma) % 2 == 0 ? 1 : -1);
                                // Console.WriteLine("radan({0}, {1}, {2}) = {3}", i1, i2, irho, this.radan[i1, i2, irho]);
                            }
                        }

                        // Výpoèet radiálních maticových elementù pro kinetickou energii
                        // Kinetická energie do 1. øádu
                        if(irho == 2 && idl == 0) {
                            this.radank[i1, i2] = 0.5 * this.radan[i1, i2, irho] * ((idn / 2) % 2 == 0 ? 1 : -1);
                            this.radank[i2, i1] = this.radank[i1, i2];
                        }

                        /*
                                               // Kinetická energie do druhého øádu
                                                if(irho == 3) {
                                                    if(idn == 3) 
                                                        this.radan[i1, i2, 8] = -3.0 * this.radan[i1, i2, 3];
                                                    else if(idn == 1) 
                                                        this.radan[i1, i2, 8] = this.radan[i1, i2, 8];

                                                    this.radan[i2, i1, 7] = this.radan[i1, i2, 7];
                                                }
                        */
                        this.radan[i2, i1, irho] = radan[i1, i2, irho];
                    }
        }

        /// <summary>
        /// Znaménko gamma funkce
        /// </summary>
        /// <param name="x">Parametr funkce</param>
        /// <returns>-1 pro záporné znaménko, +1 pro kladné znaménko</returns>
        private int SignGamma(double x) {
            if(x >= 0)
                return 1;
            else
                return (int)System.Math.Round(System.Math.Abs(x) - 0.5, 0) % 2 == 0 ? -1 : 1;
        }

        /// <summary>
        /// Indexer (Pro kinetický èlen je irho == -1)
        /// </summary>
        public double this[int i1, int i2, int irho] { get { return irho >= 0 ? this.radan[i1, i2, irho] : this.radank[i1, i2]; } }

        public const int MaxDim = 1300;
    }
}

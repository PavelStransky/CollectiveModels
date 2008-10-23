using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Všecha rozdìlení náhodných matic (GOE, GUE, GSE)
    /// </summary>
    public class RMTDistribution {
        private static Random random = new Random();

        /// <summary>
        /// Hodnota z GOE rozdìlení
        /// </summary>
        public static double GetGOE() {
            double x, y;

            do {
                x = random.NextDouble() * maxx;
                y = random.NextDouble() * maxy;
            } while(SpecialFunctions.GOE(x) < y);

            return x;
        }

        /// <summary>
        /// Hodnota z GUE rozdìlení
        /// </summary>
        public static double GetGUE() {
            double x, y;

            do {
                x = random.NextDouble() * maxx;
                y = random.NextDouble() * maxy;
            } while(SpecialFunctions.GUE(x) < y);

            return x;
        }

        /// <summary>
        /// Hodnota z GOE rozdìlení
        /// </summary>
        public static double GetGSE() {
            double x, y;

            do {
                x = random.NextDouble() * maxx;
                y = random.NextDouble() * maxy;
            } while(SpecialFunctions.GSE(x) < y);

            return x;
        }

        /// <summary>
        /// Hodnota z Poissonova rozdìlení
        /// </summary>
        public static double GetPoisson() {
            double x, y;

            do {
                x = random.NextDouble() * maxx;
                y = random.NextDouble() * maxy;
            } while(SpecialFunctions.Poisson(x) < y);

            return x;
        }

        /// <summary>
        /// Hodnota Brodyho rozdìlení
        /// </summary>
        /// <param name="b">Brodyho parametr</param>
        public static double GetBrody(double b) {
            double x, y;

            do {
                x = random.NextDouble() * maxx;
                y = random.NextDouble() * maxy;
            } while(SpecialFunctions.Brody(x, b) < y);

            return x;
        }

        private const double maxx = 10.0;
        private const double maxy = 1.5;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// V�echa rozd�len� n�hodn�ch matic (GOE, GUE, GSE)
    /// </summary>
    public class RMTDistribution {
        private static Random random = new Random();

        /// <summary>
        /// Hodnota z GOE rozd�len�
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
        /// Hodnota z GUE rozd�len�
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
        /// Hodnota z GOE rozd�len�
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
        /// Hodnota z Poissonova rozd�len�
        /// </summary>
        public static double GetPoisson() {
            double x, y;

            do {
                x = random.NextDouble() * maxx;
                y = random.NextDouble() * maxy;
            } while(SpecialFunctions.Poisson(x) < y);

            return x;
        }

        private const double maxx = 10.0;
        private const double maxy = 1.5;
    }
}

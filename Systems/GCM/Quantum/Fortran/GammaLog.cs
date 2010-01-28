using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Systems {
    /// <summary>
    /// Pomocná tøída, uchovává logaritmy gamma funkce
    /// </summary>
    class GammaLog {
        double[] data;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public GammaLog() {
            this.data = new double[300];

            this.data[102] = 0;
            this.data[101] = System.Math.Log(1.772454);

            for(int i = 104; i <= 200; i += 2)
                this.data[i] = System.Math.Log(i - 102) + this.data[i - 2] - System.Math.Log(2.0);

            for(int i = 103; i <= 199; i += 2)
                this.data[i] = System.Math.Log(i - 102) + this.data[i - 2] - System.Math.Log(2.0);

            for(int i = 99; i >= 1; i -= 2)
                this.data[i] = this.data[i + 2] + System.Math.Log(2.0) - System.Math.Log(System.Math.Abs(i - 100));
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="id">2 * x</param>
        /// <returns>Hodnota ln(Gamma(2 * x))</returns>
        public double this[int id] { get { return this.data[id + 100]; } }
    }
}

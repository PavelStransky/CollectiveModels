using System;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// Pøedvypoèítá hodnoty bázové funkce na zadané møížce
    /// </summary>
    public class BasisCache2D {
        /// <summary>
        /// Bázová funkce 2D
        /// </summary>
        /// <param name="n">Øád bázové funkce</param>
        /// <param name="x">Promìnná x</param>
        /// <param name="y">Promìnná y</param>
        public delegate double BasisFunction2D(double x, double y, int n);

        private double[,] cache;
        private BasisFunction2D function;
        private int n;

        private DiscreteInterval intervalx, intervaly;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="intervalx">Meze parametrù x</param>
        /// <param name="intervaly">Meze parametrù y</param>
        /// <param name="n">Øád bázové funkce</param>
        /// <param name="function">Bázová funkce</param>
        public BasisCache2D(DiscreteInterval intervalx, DiscreteInterval intervaly, int n, BasisFunction2D function)
            {
            this.intervalx = intervalx;
            this.intervaly = intervaly;

            this.n = n;
            this.function = function;

            int numx = this.intervalx.Num;
            int numy = this.intervaly.Num;

            this.cache = new double[numx, numy];

            // Naplnìní cache
            for(int i = 0; i < numx; i++)
                for(int j = 0; j < numy; j++)
                    this.cache[i, j] = function(this.intervalx.GetX(i), this.intervaly.GetX(j), n);
        }

        /// <summary>
        /// Konstruktor (shodné intervaly x, y)
        /// </summary>
        /// <param name="interval">Meze parametrù</param>
        /// <param name="n">Øád bázové funkce</param>
        /// <param name="function">Bázová funkce</param>
        public BasisCache2D(DiscreteInterval interval, int n, BasisFunction2D function)
            : this(interval, interval, n, function) { }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="i">Index cache ve smìru x</param>
        /// <param name="j">Index cache ve smìru y</param>
        public double this[int i, int j] {
            get {
                return this.cache[i, j];
            }
        }

        /// <summary>
        /// Maximální index cache x
        /// </summary>
        public int MaxIndexX { get { return this.cache.GetLength(0) - 1; } }

        /// <summary>
        /// Maximální index cache y
        /// </summary>
        public int MaxIndexY { get { return this.cache.GetLength(1) - 1; } }

        /// <summary>
        /// Cachovaný øád bázové funkce
        /// </summary>
        public int N { get { return this.n; } }

        /// <summary>
        /// Vrátí hodnotu v daném bodì x
        /// </summary>
        /// <param name="x">Hodnota x</param>
        /// <param name="y">Hodnota y</param>
        public double GetValue(double x, double y) {
            int i = this.intervalx.GetIndex(x);
            int j = this.intervaly.GetIndex(y);

            if(i < 0 || i > this.MaxIndexX || j < 0 || j >= this.MaxIndexY)
                return this.function(x, y, this.n);

            return this.cache[i, j];
        }
    }
}
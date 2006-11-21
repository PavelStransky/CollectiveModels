using System;

namespace PavelStransky.GCM {
    /// <summary>
    /// Bázová funkce
    /// </summary>
    /// <param name="n">Øád bázové funkce</param>
    /// <param name="x">Promìnná</param>
    public delegate double BasisFunction(int n, double x);

    /// <summary>
    /// Pøedvypoèítá hodnoty bázové funkce na zadané møížce
    /// </summary>
    public class BasisCache {
        private double minx, stepx;
        private double[,] cache;
        private int maxn;
        private BasisFunction function;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="minx">Minimální hodnota</param>
        /// <param name="stepx">Krok</param>
        /// <param name="numSteps">Poèet krokù</param>
        /// <param name="maxn">Maximální øád bázové funkce</param>
        /// <param name="function">Bázová funkce</param>
        public BasisCache(double minx, double stepx, int numSteps, int maxn, BasisFunction function) {
            this.minx = minx;
            this.stepx = stepx;
            this.maxn = maxn;
            this.function = function;

            this.cache = new double[maxn, numSteps];

            for(int n = 0; n < maxn; n++) {
                double x = minx;
                for(int i = 0; i < numSteps; i++) {
                    this.cache[n, i] = function(n, x);
                    x += stepx;
                }
            }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="n">Øád bázové funkce</param>
        /// <param name="i">Index cache</param>
        public double this[int n, int i] {
            get {
                return this.cache[n, i];
            }
        }

        /// <summary>
        /// Vrátí x k zadanému indexu
        /// </summary>
        /// <param name="i">Index cache</param>
        public double GetX(int i) {
            return this.minx + this.stepx * i;
        }

        /// <summary>
        /// Vrátí index cache k zadanému x
        /// </summary>
        /// <param name="x">X</param>
        public int GetIndex(double x) {
            return (int)System.Math.Round((x - this.minx) / this.stepx);
        }

        /// <summary>
        /// Maximální index cache
        /// </summary>
        public int MaxIndex { get { return this.cache.GetLength(1); } }

        /// <summary>
        /// Maximální cachovaný øád bázové funkce
        /// </summary>
        public int MaxN { get { return this.maxn; } }

        /// <summary>
        /// Vrátí hodnotu v daném bodì x
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetValue(int n, double x) {
            int i = this.GetIndex(x);
            
            if(i < 0 || i > this.MaxIndex || n < 0 || n > this.maxn)
                return this.function(n, x);

            return cache[n, i];
        }
    }
}
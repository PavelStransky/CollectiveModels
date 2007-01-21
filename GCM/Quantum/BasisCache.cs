using System;

using PavelStransky.Math;

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
    public class BasisCache : DiscreteInterval {
        private double[,] cache;
        private BasisFunction function;
        private int maxn;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="interval">Meze parametrù</param>
        /// <param name="maxn">Maximální øád bázové funkce</param>
        /// <param name="function">Bázová funkce</param>
        public BasisCache(DiscreteInterval interval, int maxn, BasisFunction function)
            : base(interval) {
            this.maxn = maxn;
            this.function = function;

            this.cache = new double[maxn, this.Num];

            for(int n = 0; n < maxn; n++)
                for(int i = 0; i < this.Num; i++)
                    this.cache[n, i] = function(n, this.GetX(i));
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
        /// Maximální index cache
        /// </summary>
        public int MaxIndex { get { return this.cache.GetLength(1) - 1; } }

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

            return this.cache[n, i];
        }

        /// <summary>
        /// Vrací první indexy cache, které jsou vìtší než epsilon
        /// </summary>
        /// <param name="epsilon">Epsilon</param>
        public int[] GetLowerLimits(double epsilon) {
            int[] result = new int[this.maxn];

            int maxIndex = this.MaxIndex;

            for(int i = 0; i < this.maxn; i++) {
                int limit = 0;
                while(limit <= maxIndex && System.Math.Abs(this.cache[i, limit]) < epsilon)
                    limit++;
                result[i] = System.Math.Max(0, limit - 1);
            }

            return result;
        }

        /// <summary>
        /// Vrací poslední indexy cache, které jsou vìtší než epsilon
        /// </summary>
        /// <param name="epsilon">Epsilon</param>
        public int[] GetUpperLimits(double epsilon) {
            int[] result = new int[this.maxn];

            int maxIndex = this.MaxIndex;

            for(int i = 0; i < this.maxn; i++) {
                int limit = maxIndex;
                while(limit >= 0 && System.Math.Abs(this.cache[i, limit]) < epsilon)
                    limit--;
                result[i] = System.Math.Min(maxIndex, limit + 1);
            }

            return result;
        }
    }
}
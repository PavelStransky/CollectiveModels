using System;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// P�edvypo��t� hodnoty b�zov� funkce na zadan� m��ce
    /// </summary>
    public class BasisCache : DiscreteInterval {
        /// <summary>
        /// B�zov� funkce
        /// </summary>
        /// <param name="n">��d b�zov� funkce</param>
        /// <param name="x">Prom�nn�</param>
        public delegate double BasisFunction(double x, int n);

        private double[,] cache;
        private BasisFunction function;
        private int minn, maxn;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="interval">Meze parametr�</param>
        /// <param name="maxn">Maxim�ln� ��d b�zov� funkce</param>
        /// <param name="function">B�zov� funkce</param>
        public BasisCache(DiscreteInterval interval, int maxn, BasisFunction function)
            : this(interval, 0, maxn, function) { }

        /// <summary>
        /// Konstruktor (cache pouze v intervalu minn, maxn)
        /// </summary>
        /// <param name="interval">Meze parametr�</param>
        /// <param name="maxn">Minim�ln� ��d b�zov� funkce</param>
        /// <param name="maxn">Maxim�ln� ��d b�zov� funkce</param>
        /// <param name="function">B�zov� funkce</param>
        public BasisCache(DiscreteInterval interval, int minn, int maxn, BasisFunction function) :base(interval) {
            this.minn = minn;
            this.maxn = maxn;
            this.function = function;

            this.cache = new double[(maxn - minn), this.Num];

            for(int n = minn; n < maxn; n++)
                for(int i = 0; i < this.Num; i++)
                    this.cache[n - minn, i] = function(this.GetX(i), n);

        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="n">��d b�zov� funkce</param>
        /// <param name="i">Index cache</param>
        public double this[int n, int i] {
            get {
                return this.cache[n - this.minn, i];
            }
        }

        /// <summary>
        /// Maxim�ln� index cache
        /// </summary>
        public int MaxIndex { get { return this.cache.GetLength(1) - 1; } }

        /// <summary>
        /// Minim�ln� cachovan� ��d b�zov� funkce
        /// </summary>
        public int MinN { get { return this.minn; } }

        /// <summary>
        /// Maxim�ln� cachovan� ��d b�zov� funkce
        /// </summary>
        public int MaxN { get { return this.maxn; } }

        /// <summary>
        /// Vr�t� hodnotu v dan�m bod� x
        /// </summary>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public double GetValue(int n, double x) {
            int i = this.GetIndex(x);

            if(i < 0 || i > this.MaxIndex || n < this.minn || n >= this.maxn)
                return this.function(x, n);

            return this.cache[n - this.minn, i];
        }

        /// <summary>
        /// Vrac� prvn� indexy cache, kter� jsou v�t�� ne� epsilon
        /// </summary>
        /// <param name="epsilon">Epsilon</param>
        public int[] GetLowerLimits(double epsilon) {
            int[] result = new int[this.maxn - this.minn];

            int maxIndex = this.MaxIndex;

            for(int i = 0; i < this.maxn - this.minn; i++) {
                int limit = 0;
                while(limit <= maxIndex && System.Math.Abs(this.cache[i, limit]) < epsilon)
                    limit++;
                result[i] = System.Math.Max(0, limit - 1);
            }

            return result;
        }

        /// <summary>
        /// Vrac� posledn� indexy cache, kter� jsou v�t�� ne� epsilon
        /// </summary>
        /// <param name="epsilon">Epsilon</param>
        public int[] GetUpperLimits(double epsilon) {
            int[] result = new int[this.maxn - this.minn];

            int maxIndex = this.MaxIndex;

            for(int i = 0; i < this.maxn - this.minn; i++) {
                int limit = maxIndex;
                while(limit >= 0 && System.Math.Abs(this.cache[i, limit]) < epsilon)
                    limit--;
                result[i] = System.Math.Min(maxIndex, limit + 1);
            }

            return result;
        }
    }
}
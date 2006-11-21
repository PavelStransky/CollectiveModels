using System;

namespace PavelStransky.GCM {
    /// <summary>
    /// B�zov� funkce
    /// </summary>
    /// <param name="n">��d b�zov� funkce</param>
    /// <param name="x">Prom�nn�</param>
    public delegate double BasisFunction(int n, double x);

    /// <summary>
    /// P�edvypo��t� hodnoty b�zov� funkce na zadan� m��ce
    /// </summary>
    public class BasisCache {
        private double minx, stepx;
        private double[,] cache;
        private int maxn;
        private BasisFunction function;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="minx">Minim�ln� hodnota</param>
        /// <param name="stepx">Krok</param>
        /// <param name="numSteps">Po�et krok�</param>
        /// <param name="maxn">Maxim�ln� ��d b�zov� funkce</param>
        /// <param name="function">B�zov� funkce</param>
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
        /// <param name="n">��d b�zov� funkce</param>
        /// <param name="i">Index cache</param>
        public double this[int n, int i] {
            get {
                return this.cache[n, i];
            }
        }

        /// <summary>
        /// Vr�t� x k zadan�mu indexu
        /// </summary>
        /// <param name="i">Index cache</param>
        public double GetX(int i) {
            return this.minx + this.stepx * i;
        }

        /// <summary>
        /// Vr�t� index cache k zadan�mu x
        /// </summary>
        /// <param name="x">X</param>
        public int GetIndex(double x) {
            return (int)System.Math.Round((x - this.minx) / this.stepx);
        }

        /// <summary>
        /// Maxim�ln� index cache
        /// </summary>
        public int MaxIndex { get { return this.cache.GetLength(1); } }

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
            
            if(i < 0 || i > this.MaxIndex || n < 0 || n > this.maxn)
                return this.function(n, x);

            return cache[n, i];
        }
    }
}
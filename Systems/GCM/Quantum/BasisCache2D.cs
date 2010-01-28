using System;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// P�edvypo��t� hodnoty b�zov� funkce na zadan� m��ce
    /// </summary>
    public class BasisCache2D {
        /// <summary>
        /// B�zov� funkce 2D
        /// </summary>
        /// <param name="n">��d b�zov� funkce</param>
        /// <param name="x">Prom�nn� x</param>
        /// <param name="y">Prom�nn� y</param>
        public delegate double BasisFunction2D(double x, double y, int n);

        private double[,] cache;
        private BasisFunction2D function;
        private int n;

        private DiscreteInterval intervalx, intervaly;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="intervalx">Meze parametr� x</param>
        /// <param name="intervaly">Meze parametr� y</param>
        /// <param name="n">��d b�zov� funkce</param>
        /// <param name="function">B�zov� funkce</param>
        public BasisCache2D(DiscreteInterval intervalx, DiscreteInterval intervaly, int n, BasisFunction2D function)
            {
            this.intervalx = intervalx;
            this.intervaly = intervaly;

            this.n = n;
            this.function = function;

            int numx = this.intervalx.Num;
            int numy = this.intervaly.Num;

            this.cache = new double[numx, numy];

            // Napln�n� cache
            for(int i = 0; i < numx; i++)
                for(int j = 0; j < numy; j++)
                    this.cache[i, j] = function(this.intervalx.GetX(i), this.intervaly.GetX(j), n);
        }

        /// <summary>
        /// Konstruktor (shodn� intervaly x, y)
        /// </summary>
        /// <param name="interval">Meze parametr�</param>
        /// <param name="n">��d b�zov� funkce</param>
        /// <param name="function">B�zov� funkce</param>
        public BasisCache2D(DiscreteInterval interval, int n, BasisFunction2D function)
            : this(interval, interval, n, function) { }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="i">Index cache ve sm�ru x</param>
        /// <param name="j">Index cache ve sm�ru y</param>
        public double this[int i, int j] {
            get {
                return this.cache[i, j];
            }
        }

        /// <summary>
        /// Maxim�ln� index cache x
        /// </summary>
        public int MaxIndexX { get { return this.cache.GetLength(0) - 1; } }

        /// <summary>
        /// Maxim�ln� index cache y
        /// </summary>
        public int MaxIndexY { get { return this.cache.GetLength(1) - 1; } }

        /// <summary>
        /// Cachovan� ��d b�zov� funkce
        /// </summary>
        public int N { get { return this.n; } }

        /// <summary>
        /// Vr�t� hodnotu v dan�m bod� x
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
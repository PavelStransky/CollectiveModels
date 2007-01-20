using System;

using PavelStransky.Math;

namespace PavelStransky.Expression {
    /// <summary>
    /// T��da, kter� uchov�v� minim�ln� a maxim�ln� hodnotu skupiny grafu
    /// </summary>
    public class MinMaxCache {
        private double minX, maxX, minY, maxY;
        private int maxLength;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="minmax">Vektor se 4 slo�kami (minX, maxX, minY, maxY)</param>
        /// <param name="maxLength">Maxim�ln� d�lka vektoru k vykreslen� ve skupin�</param>
        public MinMaxCache(Vector minmax, int maxLength) {
            this.minX = minmax[0];
            this.maxX = minmax[1];
            this.minY = minmax[2];
            this.maxY = minmax[3];

            this.maxLength = maxLength;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="minmax">Vektor se 4 slo�kami (minX, maxX, minY, maxY)</param>
        public MinMaxCache(Vector minmax) : this(minmax, 0) { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="minX">Minim�ln� x</param>
        /// <param name="maxX">Maxim�ln� x</param>
        /// <param name="minY">Minim�ln� y</param>
        /// <param name="maxY">Maxim�ln� y</param>
        /// <param name="maxLength">Maxim�ln� d�lka vektoru k vykreslen� ve skupin�</param>
        public MinMaxCache(double minX, double maxX, double minY, double maxY, int maxLength) {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;

            this.maxLength = maxLength;
        }

        public double MinX { get { return this.minX; } set { this.minX = value; } }
        public double MaxX { get { return this.maxX; } set { this.maxX = value; } }
        public double MinY { get { return this.minY; } set { this.minY = value; } }
        public double MaxY { get { return this.maxY; } set { this.maxY = value; } }

        public double Width { get { return this.maxX - this.minX; } }
        public double Height { get { return this.maxY - this.minY; } }

        public int MaxLength { get { return this.maxLength; } }
    }
}

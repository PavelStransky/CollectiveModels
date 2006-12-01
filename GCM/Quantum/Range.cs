using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    public class Range {
        private double min, max, step;
        private int num;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="range">Rozmìry (uspoøádané ve tvaru min, max, num)</param>
        public Range(Vector range) {
            this.min = range[0]; this.max = range[1];
            this.num = (int)range[2];
            this.step = (this.max - this.min) / (this.num - 1);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="min">Minimální x</param>
        /// <param name="max">Maximální x</param>
        /// <param name="num">Poèet hodnot</param>
        public Range(double min, double max, int num) {
            this.min = min;
            this.max = max;
            this.num = num;
        }

        /// <summary>
        /// Minimální x
        /// </summary>
        public double Min { get { return this.min; } }

        /// <summary>
        /// Maximální x
        /// </summary>
        public double Max { get { return this.max; } }

        /// <summary>
        /// Poèet hodnot 
        /// </summary>
        public int Num { get { return this.num; } }

        /// <summary>
        /// Vzdálenost mezi hodnotami
        /// </summary>
        public double Step { get { return this.step; } }

        /// <summary>
        /// Podle zadaného indexu vrací x
        /// </summary>
        /// <param name="i">Index x</param>
        public double GetX(int i) {
            return this.step * i + this.min;
        }

        /// <summary>
        /// Vrátí index k zadanému x
        /// </summary>
        /// <param name="x">X</param>
        public int GetIndex(double x) {
            return (int)System.Math.Round((x - this.min) / this.step);
        }
    }
}
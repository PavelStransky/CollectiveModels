using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    public class DiscreteInterval {
        private double min, max, step;
        private int num;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="interval">Rozmìry (uspoøádané ve tvaru min, max, num)</param>
        public DiscreteInterval(Vector interval) {
            this.min = interval[0]; this.max = interval[1];
            this.num = (int)interval[2];
            this.CalculateStep();
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="interval">Interval</param>
        public DiscreteInterval(DiscreteInterval interval) {
            this.min = interval.min;
            this.max = interval.max;
            this.num = interval.num;
            this.step = interval.step;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="min">Minimální x</param>
        /// <param name="max">Maximální x</param>
        /// <param name="num">Poèet hodnot</param>
        public DiscreteInterval(double min, double max, int num) {
            this.min = min;
            this.max = max;
            this.num = num;
            this.CalculateStep();
        }

        /// <summary>
        /// Konstruktor symetrické oblasti
        /// </summary>
        /// <param name="minmax">Minimální a maximální hodnota</param>
        /// <param name="num">Poèet hodnot</param>
        public DiscreteInterval(double minmax, int num) {
            this.max = System.Math.Abs(minmax);
            this.min = -this.max;
            this.num = num;
            this.CalculateStep();
        }

        /// <summary>
        /// Dopoèítá promìnnou step
        /// </summary>
        private void CalculateStep() {
            if (this.max == this.min)
                this.step = 0;
            else
                this.step = (this.max - this.min) / (this.num - 1);
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
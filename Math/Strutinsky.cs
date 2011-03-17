using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    public class Strutinsky {
        private Vector energy;
        private Vector gamma;   // Range for each energy
        private int degree;     // Degree of the Laguerre polynomial

        private Vector cache;

        public Strutinsky(Vector energy, Vector gamma, int degree) {
            this.energy = (Vector)energy.Sort();
            this.gamma = gamma;
            this.degree = degree;

            this.BuildCache();
        }

        /// <summary>
        /// Static constructor (preparing cache)
        /// </summary>
        private void BuildCache() {
            int length = (int)(max / precision) + 1;
            this.cache = new Vector(length);

            double oldy = this.Function(0.0);

            for(int i = 1; i < length; i++) {
                double x = precision * i;
                double y = this.Function(x);
                this.cache[i] = this.cache[i - 1] + precision * (y + oldy) / 2.0;
                oldy = y;
            }
        }

        private class ChPBisectionFunction {
            private int A;
            private Vector energy;
            private Vector gamma;
            private Vector cache;

            public ChPBisectionFunction(Vector energy, Vector gamma, Vector cache, int A) {
                this.energy = energy;
                this.gamma = gamma;
                this.cache = cache;
                this.A = A;
            }

            public double BisectionFunction(double lambda) {
                int length = this.energy.Length;
                double result = 0.0;
                double max = this.cache.LastItem;

                for(int i = 0; i < length; i++) {
                    double x = (lambda - this.energy[i]) / this.gamma[i];
                    int xi = (int)(x / precision);
                    if(x < -max)
                        continue;
                    else if(x < 0.0)
                        result += max - this.cache[-xi];
                    else if(x < max)
                        result += max + this.cache[xi];
                    else
                        result += max + max;
                }

                return result - this.A;
            }
        }

        public double ChemicalPotential(int A) {
            ChPBisectionFunction bf = new ChPBisectionFunction(this.energy, this.gamma, this.cache, A);
            Bisection b = new Bisection(bf.BisectionFunction);
            return b.Solve(this.energy.Min(), this.energy.Max(), precision);
        }

        private double Function(double x) {
            x *= x;
            if(x > 100.0)
                return 0.0;

            return SpecialFunctions.Laguerre(x, this.degree, 0.5) * System.Math.Exp(-x);
        }

        private const double precision = 1E-5;
        private const double max = 8.0;
    }
}

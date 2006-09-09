using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Tøída, poèátající adaptivní RK metodou; ovìøuje energii
    /// </summary>
    public class RungeKuttaEnergy: RungeKutta {
        protected ScalarFunction energy;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="equation">Rovnice</param>
        /// <param name="energy">Energie</param>
        /// <param name="precision">Pøesnost výpoètu</param>
        public RungeKuttaEnergy(VectorFunction equation, ScalarFunction energy, double precision)
            : base(equation, precision) {
            this.energy = energy;
        }

        public override Vector Step(Vector x, ref double step, out double newStep) {
            double e = this.energy(x);
            Vector result;
            newStep = step;

            do {
                result = base.Step(x, ref step, out step);
                double newe = this.energy(x + result);

                double diff = System.Math.Abs(newe - e);

                if(diff > this.precision) {
                    step /= 2.0;
                    continue;
                }
                else if(1.5 * diff > this.precision) {
                    break;
                }
                else {
                    newStep = 2.0 * step;
                    break;
                }
            } while(true);

            return result;
        }

        private const double defaultPrecisionE = 1E-12;
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// T��da, po��taj�c� adaptivn� RK metodou; ov��uje energii
    /// </summary>
    public class RungeKuttaEnergy: RungeKutta {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        /// <param name="precision">P�esnost v�po�tu</param>
        public RungeKuttaEnergy(IDynamicalSystem dynamicalSystem, double precision)
            : base(dynamicalSystem, precision) {
        }

        public override Vector Step(Vector x, ref double step, out double newStep) {
            double e = this.dynamicalSystem.E(x);
            Vector result;
            newStep = step;

            do {
                result = base.Step(x, ref step, out step);
                double newe = this.dynamicalSystem.E(x + result);

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
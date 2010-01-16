using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Tøída, poèátající adaptivní RK metodou; ovìøuje energii
    /// </summary>
    public class RungeKuttaEnergy: RungeKutta {
        private double precision;
        private IDynamicalSystem dynamicalSystem;

        /// <summary>
        /// Pøesnost výpoètu
        /// </summary>
        public override double Precision { get { return this.precision; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="precision">Pøesnost výpoètu</param>
        public RungeKuttaEnergy(IDynamicalSystem dynamicalSystem, double precision)
            : base(dynamicalSystem) {
            this.precision = precision;
            this.dynamicalSystem = dynamicalSystem;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="precision">Pøesnost výpoètu</param>
        public RungeKuttaEnergy(IDynamicalSystem dynamicalSystem)
            : this(dynamicalSystem, defaultPrecisionE) {
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
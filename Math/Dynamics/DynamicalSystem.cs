using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Základní tøída pro výpoèet dynamiky
    /// </summary>
    public class DynamicalSystem {
        protected IDynamicalSystem dynamicalSystem;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        public DynamicalSystem(IDynamicalSystem dynamicalSystem) {
            this.dynamicalSystem = dynamicalSystem;
        }

        /// <summary>
        /// Vytvoøí správný typ RungeKutta tøídy
        /// </summary>
        /// <param name="precision">Pøesnost výpoètu</param>
        /// <param name="rkMethod">Metoda k výpoètu RK</param>
        /// <returns></returns>
        public RungeKutta CreateRungeKutta(double precision, RungeKuttaMethods rkMethod) {
            switch(rkMethod) {
                case RungeKuttaMethods.Normal: return new RungeKutta(new VectorFunction(this.dynamicalSystem.Equation), precision);
                case RungeKuttaMethods.Energy: return new RungeKuttaEnergy(new VectorFunction(this.dynamicalSystem.Equation), new ScalarFunction(this.dynamicalSystem.E), precision);
                case RungeKuttaMethods.Adapted: return new RungeKuttaAdaptive(new VectorFunction(this.dynamicalSystem.Equation), precision);
            }
            return null;
        }
    }
}

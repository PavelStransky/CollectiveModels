using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Z�kladn� t��da pro v�po�et dynamiky
    /// </summary>
    public class DynamicalSystem {
        protected IDynamicalSystem dynamicalSystem;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        public DynamicalSystem(IDynamicalSystem dynamicalSystem) {
            this.dynamicalSystem = dynamicalSystem;
        }

        /// <summary>
        /// Vytvo�� spr�vn� typ RungeKutta t��dy
        /// </summary>
        /// <param name="precision">P�esnost v�po�tu</param>
        /// <param name="rkMethod">Metoda k v�po�tu RK</param>
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

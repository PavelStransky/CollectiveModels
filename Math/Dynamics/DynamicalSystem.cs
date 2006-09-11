using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Z�kladn� t��da pro v�po�et dynamiky
    /// </summary>
    public class DynamicalSystem {
        protected IDynamicalSystem dynamicalSystem;
        protected RungeKutta rungeKutta;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        public DynamicalSystem(IDynamicalSystem dynamicalSystem) {
            this.dynamicalSystem = dynamicalSystem;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="rkMethod">Metoda k v�po�tu RK</param>
        public DynamicalSystem(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod)
            : this(dynamicalSystem) {
            this.rungeKutta = this.CreateRungeKutta(precision, rkMethod);
        }

        /// <summary>
        /// Vytvo�� spr�vn� typ RungeKutta t��dy
        /// </summary>
        /// <param name="precision">P�esnost v�po�tu</param>
        /// <param name="rkMethod">Metoda k v�po�tu RK</param>
        /// <returns></returns>
        public RungeKutta CreateRungeKutta(double precision, RungeKuttaMethods rkMethod) {
            switch(rkMethod) {
                case RungeKuttaMethods.Normal: return new RungeKutta(this.dynamicalSystem, precision);
                case RungeKuttaMethods.Energy: return new RungeKuttaEnergy(this.dynamicalSystem, precision);
                case RungeKuttaMethods.Adapted: return new RungeKuttaAdaptive(this.dynamicalSystem, precision);
            }
            return null;
        }
    }
}

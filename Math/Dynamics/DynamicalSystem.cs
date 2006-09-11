using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Základní tøída pro výpoèet dynamiky
    /// </summary>
    public class DynamicalSystem {
        protected IDynamicalSystem dynamicalSystem;
        protected RungeKutta rungeKutta;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        public DynamicalSystem(IDynamicalSystem dynamicalSystem) {
            this.dynamicalSystem = dynamicalSystem;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="precision">Pøesnost výsledku</param>
        /// <param name="rkMethod">Metoda k výpoètu RK</param>
        public DynamicalSystem(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod)
            : this(dynamicalSystem) {
            this.rungeKutta = this.CreateRungeKutta(precision, rkMethod);
        }

        /// <summary>
        /// Vytvoøí správný typ RungeKutta tøídy
        /// </summary>
        /// <param name="precision">Pøesnost výpoètu</param>
        /// <param name="rkMethod">Metoda k výpoètu RK</param>
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

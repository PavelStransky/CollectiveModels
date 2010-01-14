using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Metody RK
    /// </summary>
    public enum RungeKuttaMethods { Normal, Energy, Adapted }

    public class RungeKutta {
        // Rovnice (jen pro klasickou RK, jinak se musí použít dynamický systém)
        protected VectorFunction equation;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="precision">Pøesnost výpoètu</param>
        public RungeKutta(IDynamicalSystem dynamicalSystem)
            : this(new VectorFunction(dynamicalSystem.Equation)) {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="equation">Pravá strana rovnic</param>
        public RungeKutta(VectorFunction equation) {
            this.equation = equation;
        }

        /// <summary>
        /// Inicializuje výpoèet
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        public virtual void Init(Vector initialX) { }

        /// <summary>
        /// Jeden krok výpoètu
        /// </summary>
        /// <param name="x">Vektor v èase x</param>
        /// <param name="step">Krok</param>
        /// <param name="newStep">Nový krok</param>
        /// <returns>Vypoèítaný pøírùstek</returns>
        public virtual Vector Step(Vector x, ref double step, out double newStep) {
            //Kód neoptimalizovaný na rychlost
            //Vector rightSide1 = this.equation(x);
            //Vector rightSide2 = this.equation(x + 0.5 * step * rightSide1);
            //Vector rightSide3 = this.equation(x + 0.5 * step * rightSide2);
            //Vector rightSide4 = this.equation(x + step * rightSide3);

            //newStep = step;
            //return (rightSide1 / 6.0 + rightSide2 / 3.0 + rightSide3 / 3.0 + rightSide4 / 6.0) * step;

            Vector rightSide1 = this.equation(x);
            Vector rightSide2 = this.equation(Vector.Summarize(x, 0.5 * step, rightSide1));
            Vector rightSide3 = this.equation(Vector.Summarize(x, 0.5 * step, rightSide2));
            Vector rightSide4 = this.equation(Vector.Summarize(x, step, rightSide3));

            newStep = step;

            double s3 = step / 3.0;
            return Vector.Summarize(0.5 * s3, rightSide1, s3, rightSide2, s3, rightSide3, 0.5 * s3, rightSide4);
        }

        /// <summary>
        /// Vytvoøí správný typ RungeKutta tøídy
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="rkMethod">Metoda k výpoètu RK</param>
        /// <param name="precision">Pøesnost metody</param>
        public static RungeKutta CreateRungeKutta(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod) {
            switch(rkMethod) {
                case RungeKuttaMethods.Normal: return new RungeKutta(dynamicalSystem);
                case RungeKuttaMethods.Energy: return new RungeKuttaEnergy(dynamicalSystem, precision);
                case RungeKuttaMethods.Adapted: return new RungeKuttaAdaptive(dynamicalSystem, precision);
            }

            return null;
        }

        /// <summary>
        /// Vytvoøí správný typ RungeKutta tøídy
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="rkMethod">Metoda k výpoètu RK</param>
        public static RungeKutta CreateRungeKutta(IDynamicalSystem dynamicalSystem, RungeKuttaMethods rkMethod) {
            switch(rkMethod) {
                case RungeKuttaMethods.Normal: return new RungeKutta(dynamicalSystem);
                case RungeKuttaMethods.Energy: return new RungeKuttaEnergy(dynamicalSystem);
                case RungeKuttaMethods.Adapted: return new RungeKuttaAdaptive(dynamicalSystem);
            }

            return null;
        }

        /// <summary>
        /// Vytvoøí správný typ RungeKutta tøídy
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="rkMethod">Metoda k výpoètu RK</param>
        /// <param name="precision">Pøesnost metody</param>
        public static RungeKutta CreateRungeKutta(VectorFunction function, double precision, RungeKuttaMethods rkMethod) {
            switch(rkMethod) {
                case RungeKuttaMethods.Normal: return new RungeKutta(function);
                case RungeKuttaMethods.Adapted: return new RungeKuttaAdaptive(function, precision);
            }

            return null;
        }

        private const double defaultPrecision = 1E-3;
        private const int initialNumPoints = 10000;
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Calculates the zeros of the Spherical Bessel J function
    /// </summary>
    public class BesselJZero {
        private double order;
        private double precision;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="order">Order of the Bessel function</param>
        /// <param name="precision">Precision of the calculation</param>
        public BesselJZero(double order, double precision) {
            this.order = order;
            this.precision = precision;
        }

        /// <summary>
        /// Function for the bisection procedure
        /// </summary>
        /// <param name="x">Argument of the Bessel fucntion</param>
        private double BesselFunction(double x) {
            return SpecialFunctions.SphericalBesselFunction(x, order)[0];
        }

        /// <summary>
        /// Calculates a given number of zeros
        /// </summary>
        /// <param name="num">Number of zeros</param>
        public Vector Solve(int num) {
            double factor = 0.9 * System.Math.PI;

            Vector result = new Vector(num);
            Bisection b = new Bisection(this.BesselFunction);

            double x = factor;
            double y = this.BesselFunction(x);

            int numZero = 0;
            while(numZero < num) {
                double xn = x + factor;
                double yn = this.BesselFunction(xn);

                if((y > 0 && yn <= 0) || (y < 0 && yn >= 0)) {
                    result[numZero] = b.Solve(x, xn, precision);
                    numZero++;
                }

                x = xn; y = yn;
            }

            return result;
        }
    }
}

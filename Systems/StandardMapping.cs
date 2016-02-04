using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Standard mapping system
    /// r(n+1) = r(n) - k * sin(theta(n))
    /// theta(n+1) = r(n+1) + theta(n)
    /// </summary>
    /// <remarks>83Karney_PD8_360 Long-time correlations in the stochastic regime</remarks>
    public class StandardMapping {
        private double k;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="k">Stochasticity parameter</param>
        public StandardMapping(double k) {
            this.k = k;
        }

        /// <summary>
        /// Computes the values of the mapping
        /// </summary>
        /// <param name="r">Initial value of r</param>
        /// <param name="theta">Initial value of theta</param>
        /// <param name="t">Time lapse of the calculation</param>
        public PointVector Compute(double r, double theta, int t) {
            PointVector result = new PointVector(t);
            for(int i = 0; i < t; i++) {
                theta = this.Modulo(theta, System.Math.PI);
                r = this.Modulo(r, System.Math.PI);

                result[i].X = r;
                result[i].Y = theta;

                r -= this.k * System.Math.Sin(theta);
                theta += r;
            }
            return result;
        }

        /// <summary>
        /// Calculates the modulo value
        /// </summary>
        /// <param name="x">Value</param>
        /// <param name="mod">Mantisa</param>
        public double Modulo(double x, double mod) {
            while(x > 2 * mod)
                x -= 2.0 * mod;
            while(x < 0)
                x += 2.0 * mod;
            return x;
        }
    }
}

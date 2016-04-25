using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.DLLWrapper;

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

    /// <summary>
    /// Standard mapping system
    /// y(n+1) = y(n) + k/(2 pi) * sin(2 pi x(n))
    /// x(n+1) = y(n+1) + x(n)
    /// </summary>
    /// <remarks>G. Contopoulos, JPA 32, 5213 (1999)</remarks>
    public class StandardMapping1 {
        private double k;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="k">Stochasticity parameter</param>
        public StandardMapping1(double k) {
            this.k = k;
        }

        /// <summary>
        /// Computes the values of the mapping
        /// </summary>
        /// <param name="x">Initial value of x</param>
        /// <param name="y">Initial value of y</param>
        /// <param name="t">Time lapse of the calculation</param>
        /// <param name="modulo">True if we restrict the result in interval (0; 1)</param>
        public PointVector Compute(double x, double y, int t, bool modulo) {
            PointVector result = new PointVector(t);
            for(int i = 0; i < t; i++) {
                if(modulo) {
                    x = this.Modulo(x, 1);
                    y = this.Modulo(y, 1);
                }

                result[i].X = x;
                result[i].Y = y;

                y += this.k * System.Math.Sin(2.0 * System.Math.PI * x) / (2.0 * System.Math.PI);
                x += y;
            }
            return result;
        }

        /// <summary>
        /// Jacobi matrix
        /// </summary>
        /// <param name="x">Point x</param>
        /// <param name="y">Point y</param>
        public Matrix Jacobi(double x, double y) {
            Matrix result = new Matrix(2);

            result[0, 0] = 1.0 + this.k * System.Math.Cos(2.0 * System.Math.PI * x);
            result[1, 0] = this.k * System.Math.Cos(2.0 * System.Math.PI * x);
            result[0, 1] = 1.0;
            result[1, 1] = 1.0;

            return result;
        }

        /// <summary>
        /// Calculates the modulo value
        /// </summary>
        /// <param name="x">Value</param>
        /// <param name="mod">Mantisa</param>
        public double Modulo(double x, double mod) {
            while(x > 1)
                x -= 1;
            while(x < 0)
                x += 1;
            return x;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Øeší rovnici f(x) == 0 na zadaném intervalu metodou pùlení intervalù
    /// </summary>
    public class Bisection {
        private RealFunction function;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="function">Funkce k øešení rovnice f(x) == 0</param>
        public Bisection(RealFunction function) {
            this.function = function;
        }

        /// <summary>
        /// Øešení rovnice
        /// </summary>
        /// <param name="x1">Poèáteèní mez intervalu</param>
        /// <param name="x2">Koneèná mez intervalu</param>
        /// <param name="precision">Pøesnost øešení</param>
        /// <returns>Nalezený koøen</returns>
        public double Solve(double x1, double x2, double precision) {
            double f1 = this.function(x1);
            double f2 = this.function(x2);

            if(double.IsNaN(f1) || double.IsNaN(f2))
                return double.NaN;

            if((f1 > 0 && f2 > 0) || (f1 < 0 && f2 < 0))
                return double.NaN;

            while(System.Math.Abs(x1 - x2) > precision) {
                double x = (x1 + x2) * 0.5;
                double f = this.function(x);

                if(f == 0)
                    return x;

                if((f > 0 && f1 > 0) || (f < 0 && f1 < 0)) {
                    x1 = x;
                    f1 = f;
                }
                else {
                    x2 = x;
                    f2 = f;
                }
            }

            return (x1 + x2) * 0.5; 
        }

        /// <summary>
        /// Øešení rovnice
        /// </summary>
        /// <param name="x1">Poèáteèní mez intervalu</param>
        /// <param name="x2">Koneèná mez intervalu</param>
        /// <returns>Nalezený koøen</returns>
        public double Solve(double x1, double x2) {
            return this.Solve(x1, x2, defaultPrecision);
        }

        private const double defaultPrecision = 1E-12;
    }
}

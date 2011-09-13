using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Øeší rovnici f(x) == 0, pøípadnì hledá minimum èi maximum na zadaném intervalu metodou pùlení intervalù
    /// </summary>
    public class Bisection {
        private RealFunction function;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="function">Funkce k øešení rovnice f(x) == 0 nebo k hledání minima</param>
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

            if(precision <= 0.0)
                precision = defaultPrecision;

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

        /// <summary>
        /// Hledání minima
        /// </summary>
        /// <param name="x1">Poèáteèní mez intervalu</param>
        /// <param name="x2">Koneèná mez intervalu</param>
        /// <param name="precision">Pøesnost øešení</param>
        /// <returns>Nalezené minimum</returns>
        public double Minimum(double x1, double x2, double precision) {
            double f1 = this.function(x1);
            double f2 = this.function(x2);

            if(double.IsNaN(f1) || double.IsNaN(f2))
                return double.NaN;

            if(f2 > f1) {       // Prohození bodù (aby mìly sestupnou tendenci)
                double a = f2; f2 = f1; f1 = a;
                a = x2; x2 = x1; x1 = a;
            }

            double x3 = x2 + gold * (x2 - x1);
            double f3 = this.function(x3);

            double u = 0.0;
            double fu = 0.0;

            // Procedura hledání tøí dobøe uspoøádaných bodù
            while(f2 > f3) {
                double r = (x2 - x1) * (f2 - f3);
                double q = (x2 - x3) * (f2 - f1);
                
                u = x2 - ((x2 - x3) * q - (x2 - x1) * r);

                if(q == r)
                    u /= tiny;
                else
                    u /= 2.0 * (q - r);

                // Limit
                double ulim = x2 + limit * (x3 - x2);

                if((x2 - u) * (u - x3) > 0) {
                    fu = this.function(u);

                    if(fu < f3) {
                        x1 = x2; x2 = u;
                        f1 = f2; f2 = fu;
                        break;
                    }
                    else if(fu > f2) {
                        x3 = u;
                        f3 = fu;
                        break;
                    }

                    u = x3 + gold * (x3 - x2);
                    fu = this.function(u);
                }

                else if((x3 - u) * (u - ulim) > 0.0) {
                    fu = this.function(u);

                    if(fu < f3) {
                        x2 = x3; x3 = u; u = x3 + gold * (x3 - x2);
                        f2 = f3; f3 = fu; fu = this.function(u);
                    }
                }

                else if((u - ulim) * (ulim - x3) >= 0.0) {
                    u = ulim;
                    fu = this.function(u);
                }

                else {
                    u = x3 + gold * (x3 - x2);
                    fu = this.function(u);
                }

                x1 = x2; x2 = x3; x3 = u;
                f1 = f2; f2 = f3; f3 = u;
            }

            double x0 = x1;

            double rg = gold-1;
            double cg = 1-rg;

            if(System.Math.Abs(x3 - x2) > System.Math.Abs(x2 - x1)) {
                x1 = x2;
                x2 = x2 + cg * (x3 - x2);
            }
            else {
                x1 = x2 - cg * (x2 - x1);
            }

            f1 = this.function(x1);
            f2 = this.function(x2);

            while(System.Math.Abs(x3 - x1) > precision * (System.Math.Abs(x1) + System.Math.Abs(x2))) {
                if(f2 < f1) {
                    x0 = x1; x1 = x2; x2 = rg * x1 + cg * x3;
                    f1 = f2; f2 = this.function(x2);
                }
                else {
                    x3 = x2; x2 = x1; x1 = rg * x2 + cg * x0;
                    f2 = f1; f1 = this.function(x1);
                }
            }

            if(f1 < f2)
                return x1;
            else
                return x2;
        }
        
        private const double defaultPrecision = 1E-12;

        private static double gold = (1 + System.Math.Sqrt(5.0)) / 2.0;

        private const double tiny = 1.0E-10;
        private const double limit = 100;
    }
}

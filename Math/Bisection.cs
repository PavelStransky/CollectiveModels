using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// �e�� rovnici f(x) == 0, p��padn� hled� minimum �i maximum na zadan�m intervalu metodou p�len� interval�
    /// </summary>
    public class Bisection {
        private RealFunction function;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="function">Funkce k �e�en� rovnice f(x) == 0 nebo k hled�n� minima</param>
        public Bisection(RealFunction function) {
            this.function = function;
        }

        /// <summary>
        /// �e�en� rovnice
        /// </summary>
        /// <param name="x1">Po��te�n� mez intervalu</param>
        /// <param name="x2">Kone�n� mez intervalu</param>
        /// <param name="precision">P�esnost �e�en�</param>
        /// <returns>Nalezen� ko�en</returns>
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
        /// �e�en� rovnice
        /// </summary>
        /// <param name="x1">Po��te�n� mez intervalu</param>
        /// <param name="x2">Kone�n� mez intervalu</param>
        /// <returns>Nalezen� ko�en</returns>
        public double Solve(double x1, double x2) {
            return this.Solve(x1, x2, defaultPrecision);
        }

        /// <summary>
        /// Hled�n� minima
        /// </summary>
        /// <param name="x1">Po��te�n� mez intervalu</param>
        /// <param name="x2">Kone�n� mez intervalu</param>
        public double Minimum(double x1, double x2) {
            return this.Minimum(x1, x2, defaultPrecision);
        }

        public double Minimum(double x1, double x2, double precision) {
            return this.Minimum(x1, 0.5 * (x1 + x2), x2, defaultPrecision);
        }

        /// <summary>
        /// Hled�n� minima
        /// </summary>
        /// <param name="a">Po��te�n� mez intervalu</param>
        /// <param name="b">St�ed intervalu</param>
        /// <param name="c">Kone�n� mez intervalu</param>
        /// <param name="precision">P�esnost �e�en�</param>
        /// <returns>Nalezen� minimum</returns>
        public double Minimum(double a, double b, double c, double precision) {
            double x = (c - b > b - a) ? b + resgold * (c - b) : b - resgold * (b - a);

            if(System.Math.Abs(c - a) < precision * (System.Math.Abs(b) + System.Math.Abs(x)))
                return 0.5 * (c + a);

            if(this.function(x) < this.function(b)) {
                if(c - b > b - a)
                    return this.Minimum(b, x, c, precision);
                else
                    return this.Minimum(a, x, b, precision);
            }
            else {
                if(c - b > b - a)
                    return this.Minimum(a, b, x, precision);
                else
                    return this.Minimum(x, b, c, precision);
            }
       }
        
        private const double defaultPrecision = 1E-12;

        private static double gold = (1 + System.Math.Sqrt(5.0)) / 2.0;
        private static double resgold = 2 - gold;

        private const double tiny = 1.0E-10;
        private const double limit = 100;
    }
}
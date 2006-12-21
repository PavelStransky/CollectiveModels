using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Tøída poèítající speciální funkce
    /// </summary>
    public class SpecialFunctions {
        /// <summary>
        /// Statický konstruktor
        /// </summary>
        static SpecialFunctions() {
            // Nastavení koeficientù pro výpoèet logaritmu gamma funkce
            gammaLogKoef = new double[6];
            gammaLogKoef[0] = 76.18009172947146;
            gammaLogKoef[1] = -86.50532032941677;
            gammaLogKoef[2] = 24.01409824083091;
            gammaLogKoef[3] = -1.231739572450155;
            gammaLogKoef[4] = 0.1208650973866179E-2;
            gammaLogKoef[5] = -0.5395239384953e-5;
        }

        /// <summary>
        /// Logaritmus gamma funkce (Gamma(z) = int_0^inf (t^(z-1)Exp(-t)dt)
        /// </summary>
        /// <param name="x">Vstupní hodnota</param>
        /// <remarks>Numerical Recipies in C, Chapter 6.1</remarks>
        public static double GammaLog(double x) {
            double a = x;
            double b = x;
            double t = x + 5.5;
            t -= (x + 0.5) * System.Math.Log(t);

            double s = 1.000000000190015;
            for(int i = 0; i < 6; i++)
                s += gammaLogKoef[i] / ++b;

            return -t + System.Math.Log(2.5066282746310005 * s / a);
        }

        /// <summary>
        /// Poèítá faktoriál pomocí gamma funkce
        /// </summary>
        /// <param name="x">Vstupní hodnota</param>
        /// <remarks>Numerical Recipies in C, Chapter 6.1</remarks>
        public static double Factorial(double x) {
            return System.Math.Exp(GammaLog(x + 1.0));
        }

        /// <summary>
        /// Poèítá binomický koeficient (n k)
        /// </summary>
        /// <param name="x">Vstupní hodnota</param>
        /// <remarks>Numerical Recipies in C, Chapter 6.1</remarks>
        public static double BinomialCoeficient(int n, int k) {
            return System.Math.Floor(0.5 + System.Math.Exp(LogFactorial(n) - LogFactorial(k) - LogFactorial(n - k)));
        }

        /// <summary>
        /// Poèítá binomický koeficient (n k)
        /// </summary>
        /// <param name="x">Vstupní hodnota</param>
        /// <remarks>Numerical Recipies in C, Chapter 6.1</remarks>
        public static double BinomialCoeficient(double n, double k) {
            return System.Math.Exp(LogFactorial(n) - LogFactorial(k) - LogFactorial(n - k));
        }

        /// <summary>
        /// Vrací ln(x!)
        /// </summary>
        /// <param name="x">Vstupní hodnota</param>
        /// <remarks>Numerical Recipies in C, Chapter 6.1</remarks>
        public static double LogFactorial(double x) {
            return GammaLog(x + 1.0);
        }

        /// <summary>
        /// Vrací beta funkci B(z, w) = Gamma(z)Gamma(w)/Gamma(z+w)
        /// </summary>
        /// <param name="x">Vstupní hodnota</param>
        /// <remarks>Numerical Recipies in C, Chapter 6.1</remarks>
        public static double Beta(double z, double w) {
            return System.Math.Exp(GammaLog(z) + GammaLog(w) - GammaLog(z + w));
        }

        /// <summary>
        /// Poèítá nekompletní gamma funkci P(a, x) = 1 / Gamma(a) int_0^x (t^(a-1)Exp(-t)dt)
        /// </summary>
        /// <remarks>Numerical Recipies in C, Chapter 6.2</remarks>
        public static double IncompleteGammaP(double a, double x) {
            if(x < a + 1.0)
                return GSeries(a, x);
            else
                return 1 - GFraction(a, x);
        }

        /// <summary>
        /// Poèítá nekompletní gamma funkci P(a, x) pomocí øady
        /// </summary>
        /// <remarks>Numerical Recipies in C, Chapter 6.2</remarks>
        private static double GSeries(double a, double x) {
            double gla = GammaLog(a);

            double ap = a;
            double sum = 1.0 / a;
            double del = sum;

            for(int i = 0; i < maxIteration; i++) {
                ap++;
                del *= x / ap;
                sum += del;

                if(System.Math.Abs(del) < epsilon * System.Math.Abs(sum))
                    return sum * System.Math.Exp(-x + a * System.Math.Log(x) - gla);
            }

            throw new Exception(string.Format(errorMessageIterationOverrun, "GSeries"));
        }

        /// <summary>
        /// Poèítá nekompletní gamma funkci P(a, x) pomocí øetìzového zlomku
        /// </summary>
        /// <remarks>Numerical Recipies in C, Chapter 6.2</remarks>
        private static double GFraction(double a, double x) {
            double gla = GammaLog(a);

            double b = x + 1.0 - a;
            double c = 1.0 / double.Epsilon;
            double d = 1.0 / b;
            double h = d;

            for(int i = 0; i < maxIteration; i++) {
                double an = -i * (i - a);
                b += 2.0;

                d = b + an * d;
                if(System.Math.Abs(d) < double.Epsilon)
                    d = double.Epsilon;

                c = b + an / c;
                if(System.Math.Abs(c) < double.Epsilon)
                    c = double.Epsilon;

                d = 1.0 / d;
                double del = d * c;
                h *= del;
                if(System.Math.Abs(del - 1.0) < epsilon)
                    return System.Math.Exp(-x + a * System.Math.Log(x) - gla) * h;
            }

            throw new Exception(string.Format(errorMessageIterationOverrun, "GFraction"));
        }

        /// <summary>
        /// Poèítá error funkci 2/Sqrt(pi) int_0^pi (Exp(-t^2) dt)
        /// </summary>
        /// <remarks>Numerical Recipies in C, Chapter 6.2</remarks>
        public static double Erf(double x) {
            return x < 0.0 ? -IncompleteGammaP(0.5, x * x) : IncompleteGammaP(0.5, x * x);
        }

        private static double[] gammaLogKoef;
        private const int maxIteration = 1000;
        private const double epsilon = 1E-10;

        private const string errorMessageIterationOverrun = "Pøekroèen poèet iterací ve funkci {0}.";
    }
}

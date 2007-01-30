using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Tøída poèítající speciální funkce
    /// </summary>
    public class SpecialFunctions {
        private static double[] factorial;
        private static double[] factorialLog;

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

            // Výpoèet bufferu factoriálù
            factorial = new double[maxFactorial];
            factorial[0] = 1;

            for(int i = 1; i < maxFactorial; i++)
                factorial[i] = factorial[i - 1] * i;

            factorialLog = new double[maxFactorialLog];
            factorialLog[0] = 0;

            for(int i = 1; i < maxFactorialLog; i++)
                factorialLog[i] = factorialLog[i - 1] + System.Math.Log(i);
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
        /// Vrátí faktoriál z pøedvypoèítaných hodnot z bufferu
        /// </summary>
        /// <param name="i">Vstupní hodnota</param>
        public static double FactorialI(int i) {
            if(i < 0)
                return double.NaN;
            else if(i >= maxFactorial)
                return double.PositiveInfinity;
            else
                return factorial[i];
        }

        /// <summary>
        /// Vrátí logaritmus faktoriálu z pøedvypoèítaných hodnot z bufferu
        /// </summary>
        /// <param name="i">Vstupní hodnota</param>
        public static double FactorialILog(int i) {
            if(i < 0)
                return double.NaN;
            else if(i >= maxFactorialLog) {
                double result = factorialLog[maxFactorialLog - 1];

                for(int j = maxFactorialLog; j <= i; i++)
                    result += System.Math.Log(j);

                return result;
            }
            else
                return factorialLog[i];
        }

        /// <summary>
        /// Poèítá binomický koeficient (n k)
        /// </summary>
        /// <param name="n">Vstupní hodnota n</param>
        /// <param name="k">Vstupní hodnota k</param>
        /// <remarks>Numerical Recipies in C, Chapter 6.1</remarks>
        public static double BinomialCoeficient(int n, int k) {
            return System.Math.Floor(0.5 + System.Math.Exp(LogFactorial(n) - LogFactorial(k) - LogFactorial(n - k)));
        }

        /// <summary>
        /// Výpoèet binomického koeficientu (n k) pøímo
        /// </summary>
        /// <param name="n">Vstupní hodnota n</param>
        /// <param name="k">Vstupní hodnota k</param>
        public static double BinomialCoeficientI(int n, int k) {
            double result = 1;
            int nk = n - k;

            if(k > nk) {
                while(n > k && nk > 1)
                    result *= (double)(n--) / (double)(nk--);

                while(n > k)
                    result *= n--;

                while(nk > 1)
                    result /= nk--;
            }
            else {
                while(n > nk && k > 1)
                    result *= (double)(n--) / (double)(k--);

                while(n > nk)
                    result *= n--;

                while(k > 1)
                    result /= k--;
            }

            return result;
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

        /// <summary>
        /// Laguerrùv polynom poèítaný rekuretním vzorcem
        /// </summary>
        /// <param name="n">Øád polynomu</param>
        /// <param name="m">Stupeò polynomu</param>
        /// <param name="x">Hodnota</param>
        /// <remarks>
        /// http://en.wikipedia.org/wiki/Laguerre_polynomials
        /// http://mathworld.wolfram.com/LaguerrePolynomial.html
        /// </remarks>
        public static double Laguerre(int n, int m, double x) {
            double lm2 = 1;
            double lm1 = m + 1.0 - x;
            double lm = lm1;

            if(n == 0)
                return lm2;
            else if(n == 1)
                return lm1;

            for(int i = 2; i <= n; i++) {
                lm = ((2 * i - 1 + m - x) * lm1 - (i - 1 + m) * lm2) / i;
                lm2 = lm1;
                lm1 = lm;
            }

            return lm;
        }

        /// <summary>
        /// Vrací logaritmus Laguerrova polynomu
        /// </summary>
        /// <param name="result">Výsledek (základ)</param>
        /// <param name="exp">Exponent - výsledné èíslo získáme jako result * Math.Exp(exp)</param>
        /// <param name="n">Øád polynomu</param>
        /// <param name="m">Stupeò polynomu</param>
        /// <param name="x">Hodnota</param>
        public static void Laguerre(out double result, out double exp, int n, int m, double x) {
            double lm2 = 1;
            double lm1 = m + 1.0 - x;

            result = lm1;
            exp = 0.0;

            if(n == 0) {
                result = lm2;
                return;
            }
            else if(n == 1)
                return;

            for(int i = 2; i <= n; i++) {
                result = ((2 * i - 1 + m - x) * lm1 - (i - 1 + m) * lm2) / i;
                double abs = System.Math.Abs(result);
                // Pokud je abs malé, nemùžeme dìlat logaritmus - nenormujeme
                if(abs == 0.0)
                    abs = 1.0;
                
                exp += System.Math.Log(abs);

                result /= abs;
                lm2 = lm1 / abs;
                lm1 = result;
            }

            return;
        }

        private static double[] gammaLogKoef;
        private const int maxIteration = 1000;
        private const double epsilon = 1E-10;
        private const int maxFactorial = 171;
        private const int maxFactorialLog = 1000;

        private const string errorMessageIterationOverrun = "Pøekroèen poèet iterací ve funkci {0}.";
    }
}

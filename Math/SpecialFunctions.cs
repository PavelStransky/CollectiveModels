using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// T��da po��taj�c� speci�ln� funkce
    /// </summary>
    public class SpecialFunctions {
        private static double[] factorial;
        private static double[] factorialLog;
        private static double[] halfFactorialLog;

        private static LongNumber[] factorialL;
        private static LongFraction[] halfFactorialL;

        /// <summary>
        /// Statick� konstruktor
        /// </summary>
        static SpecialFunctions() {
            // Nastaven� koeficient� pro v�po�et logaritmu gamma funkce
            gammaLogKoef = new double[6];
            gammaLogKoef[0] = 76.18009172947146;
            gammaLogKoef[1] = -86.50532032941677;
            gammaLogKoef[2] = 24.01409824083091;
            gammaLogKoef[3] = -1.231739572450155;
            gammaLogKoef[4] = 0.1208650973866179E-2;
            gammaLogKoef[5] = -0.5395239384953E-5;

            // V�po�et bufferu factori�l�
            factorial = new double[maxFactorial];
            factorial[0] = 1;
            for(int i = 1; i < maxFactorial; i++)
                factorial[i] = factorial[i - 1] * i;

            factorialL = new LongNumber[maxFactorialL];
            factorialL[0] = new LongNumber(1);
            for(int i = 1; i < maxFactorialL; i++)
                factorialL[i] = factorialL[i - 1] * i;

            halfFactorialL = new LongFraction[maxHalfFactorialL];
            halfFactorialL[0] = new LongNumber(1);
            for(int i = 1; i < maxHalfFactorialL; i++)
                halfFactorialL[i] = halfFactorialL[i - 1] * new LongFraction(2 * i - 1, 2);

            factorialLog = new double[maxFactorialLog];
            factorialLog[0] = 0;
            for(int i = 1; i < maxFactorialLog; i++)
                factorialLog[i] = factorialLog[i - 1] + System.Math.Log(i);

            halfFactorialLog = new double[maxHalfFactorialLog];
            halfFactorialLog[0] = System.Math.Log(System.Math.Sqrt(System.Math.PI));
            for(int i = 1; i < maxHalfFactorialLog; i++)
                halfFactorialLog[i] = halfFactorialLog[i - 1] + System.Math.Log(i - 0.5);
        }

        /// <summary>
        /// Logaritmus gamma funkce (Gamma(z) = int_0^inf (t^(z-1)Exp(-t)dt)
        /// </summary>
        /// <param name="x">Vstupn� hodnota</param>
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
        /// Po��t� faktori�l pomoc� gamma funkce
        /// </summary>
        /// <param name="x">Vstupn� hodnota</param>
        /// <remarks>Numerical Recipies in C, Chapter 6.1</remarks>
        public static double Factorial(double x) {
            return System.Math.Exp(GammaLog(x + 1.0));
        }

        /// <summary>
        /// Vr�t� faktori�l z p�edvypo��tan�ch hodnot z bufferu
        /// </summary>
        /// <param name="i">Vstupn� hodnota</param>
        public static double FactorialI(int i) {
            if(i < 0)
                return double.NaN;
            else if(i >= maxFactorial)
                return double.PositiveInfinity;
            else
                return factorial[i];
        }

        /// <summary>
        /// Vr�t� faktori�l z p�edvypo��tan�ch hodnot z bufferu
        /// </summary>
        /// <param name="i">Vstupn� hodnota</param>
        public static LongNumber FactorialL(int i) {
            if(i < 0)
                return new LongNumber(0);

            else if(i >= maxFactorialL) {
                LongNumber result = factorialL[maxFactorialL - 1];

                for(int j = maxFactorialL; j <= i; j++)
                    result *= j;

                return result;
            }

            else
                return factorialL[i];
        }

        /// <summary>
        /// Vr�t� faktori�l polovi�n�ho ��sla z p�edvypo��tan�ch hodnot z bufferu
        /// </summary>
        /// <param name="i">Vstupn� hodnota</param>
        public static LongFraction HalfFactorialL(int i) {
            if(i < 0)
                return new LongFraction(0);

            else if(i >= maxHalfFactorialL) {
                LongFraction result = factorialL[maxHalfFactorialL - 1];

                for(int j = maxHalfFactorialL; j <= i; j++)
                    result *= new LongFraction(2 * j - 1, 2);

                return result;
            }

            else
                return halfFactorialL[i];
        }

        /// <summary>
        /// Vr�t� logaritmus faktori�lu z p�edvypo��tan�ch hodnot z bufferu
        /// </summary>
        /// <param name="i">Vstupn� hodnota</param>
        public static double FactorialILog(int i) {
            if(i < 0)
                return double.NaN;

            else if(i >= maxFactorialLog) {
                double result = factorialLog[maxFactorialLog - 1];

                for(int j = maxFactorialLog; j <= i; j++)
                    result += System.Math.Log(j);

                return result;
            }

            else
                return factorialLog[i];
        }

        /// <summary>
        /// Vr�t� logaritmus faktori�lu polo��seln� hodnoty z p�edvypo��tan�ch hodnot z bufferu
        /// </summary>
        /// <param name="i">Vstupn� hodnota</param>
        public static double HalfFactorialILog(int i) {
            if(i < 0)
                return double.NaN;
            else if(i >= maxHalfFactorialLog) {
                double result = halfFactorialLog[maxHalfFactorialLog - 1];

                for(int j = maxHalfFactorialLog; j <= i; j++)
                    result += System.Math.Log(j);

                return result;
            }
            else
                return halfFactorialLog[i];
        }

        /// <summary>
        /// Po��t� binomick� koeficient (n k)
        /// </summary>
        /// <param name="n">Vstupn� hodnota n</param>
        /// <param name="k">Vstupn� hodnota k</param>
        /// <remarks>Numerical Recipies in C, Chapter 6.1</remarks>
        public static double BinomialCoeficient(int n, int k) {
            return System.Math.Floor(0.5 + System.Math.Exp(LogFactorial(n) - LogFactorial(k) - LogFactorial(n - k)));
        }

        /// <summary>
        /// V�po�et binomick�ho koeficientu (n k) p��mo
        /// </summary>
        /// <param name="n">Vstupn� hodnota n</param>
        /// <param name="k">Vstupn� hodnota k</param>
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
        /// Po��t� binomick� koeficient (n k)
        /// </summary>
        /// <param name="x">Vstupn� hodnota</param>
        /// <remarks>Numerical Recipies in C, Chapter 6.1</remarks>
        public static double BinomialCoeficient(double n, double k) {
            return System.Math.Exp(LogFactorial(n) - LogFactorial(k) - LogFactorial(n - k));
        }

        /// <summary>
        /// Vrac� ln(x!)
        /// </summary>
        /// <param name="x">Vstupn� hodnota</param>
        /// <remarks>Numerical Recipies in C, Chapter 6.1</remarks>
        public static double LogFactorial(double x) {
            return GammaLog(x + 1.0);
        }

        /// <summary>
        /// Vrac� beta funkci B(z, w) = Gamma(z)Gamma(w)/Gamma(z+w)
        /// </summary>
        /// <param name="x">Vstupn� hodnota</param>
        /// <remarks>Numerical Recipies in C, Chapter 6.1</remarks>
        public static double Beta(double z, double w) {
            return System.Math.Exp(GammaLog(z) + GammaLog(w) - GammaLog(z + w));
        }

        /// <summary>
        /// Po��t� nekompletn� gamma funkci P(a, x) = 1 / Gamma(a) int_0^x (t^(a-1)Exp(-t)dt)
        /// </summary>
        /// <remarks>Numerical Recipies in C, Chapter 6.2</remarks>
        public static double IncompleteGammaP(double a, double x) {
            if(x < a + 1.0)
                return GSeries(a, x);
            else
                return 1 - GFraction(a, x);
        }

        /// <summary>
        /// Po��t� nekompletn� gamma funkci P(a, x) pomoc� �ady
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
        /// Po��t� nekompletn� gamma funkci P(a, x) pomoc� �et�zov�ho zlomku
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
        /// Po��t� error funkci 2/Sqrt(pi) int_0^pi (Exp(-t^2) dt)
        /// </summary>
        /// <remarks>Numerical Recipies in C, Chapter 6.2</remarks>
        public static double Erf(double x) {
            return x < 0.0 ? -IncompleteGammaP(0.5, x * x) : IncompleteGammaP(0.5, x * x);
        }

        /// <summary>
        /// Laguerr�v polynom po��tan� rekuretn�m vzorcem
        /// </summary>
        /// <param name="n">��d polynomu</param>
        /// <param name="m">Stupe� polynomu</param>
        /// <param name="x">Hodnota</param>
        /// <remarks>
        /// http://en.wikipedia.org/wiki/Laguerre_polynomials
        /// http://mathworld.wolfram.com/LaguerrePolynomial.html
        /// </remarks>
        public static double Laguerre(double x, int n, int m) {
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
        /// Vrac� logaritmus Laguerrova polynomu
        /// </summary>
        /// <param name="result">V�sledek (z�klad)</param>
        /// <param name="exp">Exponent - v�sledn� ��slo z�sk�me jako result * Math.Exp(exp)</param>
        /// <param name="n">��d polynomu</param>
        /// <param name="m">Stupe� polynomu</param>
        /// <param name="x">Hodnota</param>
        public static void Laguerre(out double result, out double exp, double x, int n, double m) {
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
                // Pokud je abs mal�, nem��eme d�lat logaritmus - nenormujeme
                if(abs == 0.0)
                    abs = 1.0;
                
                exp += System.Math.Log(abs);

                result /= abs;
                lm2 = lm1 / abs;
                lm1 = result;
            }

            return;
        }

        /// <summary>
        /// Legendre polynomial
        /// </summary>
        /// <param name="n">Order of the polynomial</param>
        /// <param name="x">Value</param>
        public static double Legendre(double x, int n) {
            double lm2 = 1;
            double lm1 = x;

            double result = lm1;

            if(n == 0)
                result = lm2;

            else {
                for(int i = 2; i <= n; i++) {
                    result = ((2 * i - 1) * x * lm1 - (i - 1) * lm2) / i;

                    lm2 = lm1;
                    lm1 = result;
                }
            }

            return result;
        }

        /// <summary>
        /// Hermite polynomial (physicists' notation)
        /// </summary>
        /// <param name="n">Order of the polynomial</param>
        /// <param name="x">Value</param>
        public static double Hermite(double x, int n) {
            double hm2 = 1.0;
            double hm1 = 2.0 * x;

            double result = hm1;

            if(n == 0)
                result = hm2;

            else {
                for(int i = 2; i <= n; i++) {
                    result = 2.0 * x * hm1 - 2 * (i - 1) * hm2;

                    hm2 = hm1;
                    hm1 = result;
                }
            }

            return result;
        }

        /// <summary>
        /// Vrac� logaritmus Hermiteova polynomu
        /// </summary>
        /// <param name="result">V�sledek (z�klad)</param>
        /// <param name="exp">Exponent - v�sledn� ��slo z�sk�me jako result * Math.Exp(exp)</param>
        /// <param name="n">��d polynomu</param>
        /// <param name="x">Hodnota</param>
        public static void Hermite(out double result, out double exp, double x, int n) {
            double hm2 = 1;
            double hm1 = 2.0 * x;

            result = hm1;
            exp = 0.0;

            if(n == 0) {
                result = hm2;
                return;
            }
            else if(n == 1)
                return;

            for(int i = 2; i <= n; i++) {
                result = 2.0 * x * hm1 - 2 * (i - 1) * hm2;
                double abs = System.Math.Abs(result);
                // Pokud je abs mal�, nem��eme d�lat logaritmus - nenormujeme
                if(abs == 0.0)
                    abs = 1.0;

                exp += System.Math.Log(abs);

                result /= abs;
                hm2 = hm1 / abs;
                hm1 = result;
            }

            return;
        }
        
        /// <summary>
        /// Hodnota Wignerova rozd�len� se st�edn� hodnotou 1
        /// </summary>
        /// <param name="x">x</param>
        public static double Wigner(double x) {
            return System.Math.PI / 2.0 * x * System.Math.Exp(-System.Math.PI / 4.0 * x * x);
        }

        /// <summary>
        /// Hodnota Poissonova rozd�len� se st�edn� hodnotou 1
        /// </summary>
        /// <param name="x">x</param>
        /// <remarks>M. L. Mehta, RMT 1967, p. 10, expression (1.1)</remarks>
        public static double Poisson(double x) {
            return System.Math.Exp(-x);
        }

        private static double[] gammaLogKoef;
        private const int maxIteration = 1000;
        private const double epsilon = 1E-10;
        private const int maxFactorial = 171;
        private const int maxFactorialLog = 1000;
        private const int maxFactorialL = 200;
        private const int maxHalfFactorialLog = 1000;
        private const int maxHalfFactorialL = 200;

        private const string errorMessageIterationOverrun = "P�ekro�en po�et iterac� ve funkci {0}.";
    }
}

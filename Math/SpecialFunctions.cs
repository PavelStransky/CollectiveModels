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
        private static double[] halfFactorialLog;

        private static LongNumber[] factorialL;
        private static LongFraction[] halfFactorialL;

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
            gammaLogKoef[5] = -0.5395239384953E-5;

            // Výpoèet bufferu factoriálù
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
        /// Vrátí faktoriál z pøedvypoèítaných hodnot z bufferu
        /// </summary>
        /// <param name="i">Vstupní hodnota</param>
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
        /// Vrátí faktoriál polovièního èísla z pøedvypoèítaných hodnot z bufferu
        /// </summary>
        /// <param name="i">Vstupní hodnota</param>
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
        /// Vrátí logaritmus faktoriálu z pøedvypoèítaných hodnot z bufferu
        /// </summary>
        /// <param name="i">Vstupní hodnota</param>
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
        /// Vrátí logaritmus faktoriálu poloèíselné hodnoty z pøedvypoèítaných hodnot z bufferu
        /// </summary>
        /// <param name="i">Vstupní hodnota</param>
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
        /// Gaussovská funkce
        /// </summary>
        /// <param name="mean">Støední hodnota</param>
        /// <param name="sd">Standardní odchylka</param>
        public static double Gaussian(double x, double mean, double sd) {
            x -= mean;
            return System.Math.Exp(-x * x / (2.0 * sd * sd)) / (System.Math.Sqrt(2.0 * System.Math.PI) * sd);
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
        public static double Laguerre(double x, int n, double m) {
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
        /// Vrací logaritmus Hermiteova polynomu
        /// </summary>
        /// <param name="result">Výsledek (základ)</param>
        /// <param name="exp">Exponent - výsledné èíslo získáme jako result * Math.Exp(exp)</param>
        /// <param name="n">Øád polynomu</param>
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
                // Pokud je abs malé, nemùžeme dìlat logaritmus - nenormujeme
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
        /// Hodnota Wignerova GOE rozdìlení se støední hodnotou 1
        /// </summary>
        /// <param name="x">x</param>
        public static double GOE(double x) {
            return System.Math.PI / 2.0 * x * System.Math.Exp(-System.Math.PI / 4.0 * x * x);
        }

        /// <summary>
        /// Hodnota Wignerova GOE rozdìlení se støední hodnotou 1
        /// </summary>
        /// <param name="x">x</param>
        public static double GUE(double x) {
            double x2 = x * x;
            return 32.0 / (System.Math.PI * System.Math.PI) * x2 * System.Math.Exp(-4.0 * x2 / System.Math.PI);
        }

        /// <summary>
        /// Hodnota Wignerova GOE rozdìlení se støední hodnotou 1
        /// </summary>
        /// <param name="x">x</param>
        public static double GSE(double x) {
            double x2 = x * x;
            double n = (64.0 / (9.0 * System.Math.PI));
            return n * n * n * x2 * x2 * System.Math.Exp(-n * x2);
        }

        /// <summary>
        /// Hodnota Poissonova rozdìlení se støední hodnotou 1
        /// </summary>
        /// <param name="x">x</param>
        /// <remarks>M. L. Mehta, RMT 1967, p. 10, expression (1.1)</remarks>
        public static double Poisson(double x) {
            return System.Math.Exp(-x);
        }

        /// <summary>
        /// Hodnota Brodyho rozdìlení
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="b">Brodyho parametr</param>
        public static double Brody(double x, double b) {
            double alpha = System.Math.Exp(GammaLog((b + 2.0) / (b + 1.0)) * (b + 1));
            return (b + 1.0) * alpha * System.Math.Pow(x, b) * System.Math.Exp(-alpha * System.Math.Pow(x, b + 1));
        }

        /// <summary>
        /// Hodnota kumulovaného Brodyho rozdìlení
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="b">Brodyho parametr</param>
        public static double CumulBrody(double x, double b) {
            double alpha = System.Math.Exp(GammaLog((b + 2.0) / (b + 1.0)) * (b + 1));
            return 1 - System.Math.Exp(-alpha * System.Math.Pow(x, b + 1));
        }

        /// <summary>
        /// Spherical Bessel functions and their derivatives
        /// </summary>
        /// <param name="x">Value of the parameter</param>
        /// <param name="xnu">Order of the spherical Bessel function</param>
        /// <returns>Vector (BesselJ, BesselY, BesselJ', BesselY')</returns>
        /// <remarks>Numerical Recipies, Chapter 6.7</remarks>
        public static Vector SphericalBesselFunction(double x, double xnu) {
            double order = xnu + 0.5;
            double factor = System.Math.Sqrt(System.Math.PI / (2.0 * x));

            Vector result = BesselFunction(x, order);
            result[0] *= factor;
            result[1] *= factor;
            result[2] *= factor; result[2] -= result[0] / (2.0 * x);
            result[3] *= factor; result[3] -= result[1] / (2.0 * x);

            return result;
        }

        /// <summary>
        /// Bessel functions and their derivatives
        /// </summary>
        /// <param name="x">Value of the parameter</param>
        /// <param name="xnu">Order of the Bessel function</param>
        /// <returns>Vector (BesselJ, BesselY, BesselJ', BesselY')</returns>
        /// <remarks>Numerical Recipies, Chapter 6.7</remarks>
        public static Vector BesselFunction(double x, double xnu) {
            double EPS = 1.0e-16;       // Epsilon
            double FPMIN = 1.0e-30;
            double MAXIT = 10000;       // Maximum iterations
            double XMIN = 2.0;

            Vector result = new Vector(4);

            int nl = (x < XMIN ? (int)(xnu + 0.5) : (int)System.Math.Max(0, xnu - x + 1.5));

            double xmu = xnu - nl;
            double xmu2 = xmu * xmu;
            double xi = 1.0 / x;
            double xi2 = 2.0 * xi;
            double w = xi2 / System.Math.PI;

            int isign = 1;

            double h = xnu * xi;
            if(h < FPMIN) 
                h = FPMIN;

            double b = xi2 * xnu;
            double d = 0.0;
            double c = h;

            int i;

            for(i = 1; i <= MAXIT; i++) {
                b += xi2;
                d = b - d;
                if(System.Math.Abs(d) < FPMIN) 
                    d = FPMIN;

                c = b - 1.0 / c;
                if(System.Math.Abs(c) < FPMIN) 
                    c = FPMIN;

                d = 1.0 / d;
                if(d < 0.0)
                    isign = -isign;

                double del = c * d;
                h = del * h;

                if(System.Math.Abs(del - 1.0) < EPS)
                    break;
            }
            if(i > MAXIT)
                throw new MathException(Messages.EMLargeXBessel);

            double rjl = isign * FPMIN;
            double rjpl = h * rjl;
            double rjl1 = rjl;
            double rjp1 = rjpl;
            double fact = xnu * xi;

            for(int l = nl; l >= 1; l--) {
                double rjtemp = fact * rjl + rjpl;
                fact -= xi;
                rjpl = fact * rjtemp - rjl;
                rjl = rjtemp;
            }

            if(rjl == 0.0) 
                rjl = EPS;

            double rymu = 0.0;
            double ry1 = 0.0;
            double rymup = 0.0;
            double rjmu = 0.0;

            double f = rjpl / rjl;

            if(x < XMIN) {
                double x2 = 0.5 * x;
                double pimu = System.Math.PI * xmu;

                fact = System.Math.Abs(pimu) < EPS ? 1.0 : pimu / System.Math.Sin(pimu);
                d = -System.Math.Log(x2);
                
                double e = xmu * d;
                double fact2 = System.Math.Abs(e) < EPS ? 1.0 : System.Math.Sinh(e) / e;

                Vector besch = ChebyshevExpansion(xmu);

                double ff = 2.0 / System.Math.PI * fact * (besch[0] * System.Math.Cosh(e) + besch[1] * fact2 * d);

                e = System.Math.Exp(e);
                
                double p = e / (besch[2] * System.Math.PI);
                double q = 1.0 / (e * System.Math.PI * besch[3]);
                double pimu2 = 0.5 * pimu;
                double fact3 = System.Math.Abs(pimu2) < EPS ? 1.0 : System.Math.Sin(pimu2) / pimu2;
                double r = System.Math.PI * pimu2 * fact3 * fact3;
                
                c = 1.0;
                d = -x2 * x2;
                
                double sum = ff + r * q;
                double sum1 = p;
                
                for(i = 1; i <= MAXIT; i++) {
                    ff = (i * ff + p + q) / (i * i - xmu2);
                    c *= (d / i);
                    p /= (i - xmu);
                    q /= (i + xmu);
                    double del = c * (ff + r * q);
                    sum += del;
                    double del1 = c * p - i * del;
                    sum1 += del1;

                    if(System.Math.Abs(del) < (1.0 + System.Math.Abs(sum)) * EPS)
                        break;
                }
                if(i > MAXIT)
                    throw new MathException(Messages.EMBesselNotConverge);

                rymu = -sum;
                ry1 = -sum1 * xi2;
                rymup = xmu * xi * rymu - ry1;
                rjmu = w / (rymup - f * rymu);
            }
            else {
                double a = 0.25 - xmu2;
                double p = -0.5 * xi;
                double q = 1.0;
                double br = 2.0 * x;
                double bi = 2.0;

                fact = a * xi / (p * p + q * q);

                double cr = br + q * fact;
                double ci = bi + p * fact;
                double den = br * br + bi * bi;
                double dr = br / den;
                double di = -bi / den;
                double dlr = cr * dr - ci * di;
                double dli = cr * di + ci * dr;
                double temp = p * dlr - q * dli;

                q = p * dli + q * dlr;
                p = temp;

                for(i = 2; i <= MAXIT; i++) {
                    a += 2 * (i - 1);
                    bi += 2.0;
                    dr = a * dr + br;
                    di = a * di + bi;
                    if(System.Math.Abs(dr) + System.Math.Abs(di) < FPMIN) 
                        dr = FPMIN;
                    
                    fact = a / (cr * cr + ci * ci);
                    cr = br + cr * fact;
                    ci = bi - ci * fact;
                    if(System.Math.Abs(cr) + System.Math.Abs(ci) < FPMIN) 
                        cr = FPMIN;

                    den = dr * dr + di * di;
                    dr /= den;
                    di /= -den;
                    dlr = cr * dr - ci * di;
                    dli = cr * di + ci * dr;
                    temp = p * dlr - q * dli;
                    q = p * dli + q * dlr;
                    p = temp;

                    if(System.Math.Abs(dlr - 1.0) + System.Math.Abs(dli) < EPS) 
                        break;
                }
                if(i > MAXIT)
                    throw new MathException(Messages.EMBesselNotConverge);


                double gam = (p - f) / q;
                rjmu = System.Math.Sqrt(w / ((p - f) * gam + q));
                rjmu = rjl > 0 ? System.Math.Abs(rjmu) : -System.Math.Abs(rjmu);
                rymu = rjmu * gam;
                rymup = rymu * (p + q / gam);
                ry1 = xmu * xi * rymu - rymup;
            }
            fact = rjmu / rjl;
            result[0] = rjl1 * fact;

            result[2] = rjp1 * fact;
            for(i = 1; i <= nl; i++) {
                double rytemp = (xmu + i) * xi2 * ry1 - rymu;
                rymu = ry1;
                ry1 = rytemp;
            }
            result[1] = rymu;
            result[3] = xnu * xi * rymu - ry1;

            return result;
        }

        /// <summary>
        /// Chebyshev expansion of a gamma function (for BesselFunction)
        /// </summary>
        /// <param name="x">Parameter x</param>
        private static Vector ChebyshevExpansion(double x) {
            double[] c1 = {
                    -1.142022680371168e0,6.5165112670737e-3,
                    3.087090173086e-4,-3.4706269649e-6,6.9437664e-9,
                    3.67795e-11,-1.356e-13};
            double[] c2 = {
                    1.843740587300905e0,-7.68528408447867e-2,
                    1.2719271366546e-3,-4.9717367042e-6,-3.31261198e-8,
                    2.423096e-10,-1.702e-13,-1.49e-15};
            double xx = 8.0 * x * x - 1.0;

            Vector result = new Vector(4);
            result[0] = ChebyshevEvaluation(-1.0, 1.0, c1, 7, xx);
            result[1] = ChebyshevEvaluation(-1.0, 1.0, c2, 8, xx);
            result[2] = result[1] - x * (result[0]);
            result[3] = result[1] + x * (result[0]);

            return result;
        }

        /// <summary>
        /// Evaluation of a Chebyshev polynomial
        /// </summary>
        private static double ChebyshevEvaluation(double a, double b, double[] c, int m, double x) {
            double d = 0.0;
            double dd = 0.0;
            double y = (2.0 * x - a - b) / (b - a);
            double y2 = 2.0 * y;
            for(int j = m - 1; j >= 1; j--) {
                double sv = d;
                d = y2 * d - dd + c[j];
                dd = sv;
            }
            return y * d - dd + 0.5 * c[0];
        }

        private static double[] gammaLogKoef;
        private const int maxIteration = 1000;
        private const double epsilon = 1E-10;
        private const int maxFactorial = 171;
        private const int maxFactorialLog = 1000;
        private const int maxFactorialL = 200;
        private const int maxHalfFactorialLog = 1000;
        private const int maxHalfFactorialL = 200;

        private const string errorMessageIterationOverrun = "Pøekroèen poèet iterací ve funkci {0}.";
    }
}

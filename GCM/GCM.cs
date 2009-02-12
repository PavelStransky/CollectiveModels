using System;
using System.Collections;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// Základní tøída GCM modelu
    /// </summary>
    public class GCM: GCMParameters {
        /// <summary>
        /// Potenciál
        /// </summary>
        /// <param name="beta">Souøadnice beta</param>
        /// <param name="gamma">Souøadnice gamma</param>
        /// <returns></returns>
        public double VBG(double beta, double gamma) {
            return this.A * beta * beta + this.B * beta * beta * beta * System.Math.Cos(3.0 * gamma) + this.C * beta * beta * beta * beta;
        }

        /// <summary>
        /// Potenciál
        /// </summary>
        /// <param name="x">Souøadnice x</param>
        /// <param name="y">Souøadnice y</param>
        /// <returns></returns>
        public double V(double x, double y) {
            double b2 = x * x + y * y;
            return this.A * b2 + this.B * x * (x * x - 3 * y * y) + this.C * b2 * b2;
        }

        /// <summary>
        /// Beta, pro která nabývá potenciál lokálních extrémù
        /// </summary>
        /// <param name="g">gamma</param>
        public Vector ExtremalBeta(double gamma) {
            double x = -18.0 * this.B * System.Math.Cos(3.0 * gamma);
            double d = x * x - 1152.0 * this.A * this.C; // Diskriminant

            Vector result;
            if(d < 0.0) {
                result = new Vector(1);
                result[0] = 0.0;
            }
            else if(d == 0.0) {
                result = new Vector(1);
                result[0] = x / (48.0 * this.C);
            }
            else {
                result = new Vector(3);

                double y = System.Math.Sqrt(d);
                double betaMin1 = (x + y) / (48.0 * this.C);
                double betaMin2 = (x - y) / (48.0 * this.C);

                double v1 = this.VBG(betaMin1, gamma);
                double v2 = this.VBG(betaMin2, gamma);
                double v3 = 0.0;

                if(v1 <= v2 && v1 < v3) {        // v1 nejmenší
                    result[0] = betaMin1;
                    if(v2 < v3) {
                        result[1] = betaMin2;
                        result[2] = 0.0;
                    }
                    else {
                        result[1] = 0.0;
                        result[2] = betaMin2;
                    }
                }
                else if(v2 < v1 && v2 < v3) {   // v2 nejmenší
                    result[0] = betaMin2;
                    if(v1 < v3) {
                        result[1] = betaMin1;
                        result[2] = 0.0;
                    }
                    else {
                        result[1] = 0.0;
                        result[2] = betaMin1;
                    }
                }
                else {                          // v3 nejmenší
                    result[0] = 0.0;
                    if(v1 < v2) {
                        result[1] = betaMin1;
                        result[2] = betaMin2;
                    }
                    else {
                        result[1] = betaMin2;
                        result[2] = betaMin1;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected GCM() { }

        /// <summary>
        /// Kostruktor standardního lagranžiánu
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <param name="c">C</param>
        /// <param name="k">D</param>
        public GCM(double a, double b, double c, double k)
            : base(a, b, c, k) {
        }

        /// <summary>
        /// Poèítá øešení rovnice V(beta) = E a vrací všechny nalezené koøeny beta
        /// </summary>
        /// <param name="gamma">gamma</param>
        /// <param name="e">Energie</param>
        public Vector Roots(double e, double gamma) {
            const int MEZ = 100;	//Maximalni velikost b
            Vector result;

            GCMBisectionFunction bf = new GCMBisectionFunction(this, gamma, e);
            Bisection bisection = new Bisection(bf.Function);

            Vector extremalBeta = (Vector)this.ExtremalBeta(gamma);
            if(extremalBeta.Length == 1) {  // Jen jeden extrém - minimum v 0
                double extremalV = this.VBG(extremalBeta[0], gamma);
                if(e < extremalV)
                    result = new Vector(0);
                else {
                    result = new Vector(2);
                    result[0] = bisection.Solve(-MEZ, extremalBeta[0]);
                    result[1] = bisection.Solve(extremalBeta[0], MEZ);
                }
            }
            else {
                double extremalV1 = this.VBG(extremalBeta[0], gamma);
                double extremalV2 = this.VBG(extremalBeta[1], gamma);
                double extremalV3 = this.VBG(extremalBeta[2], gamma);

                if(e <= extremalV1) {
                    result = new Vector(0);
                }
                else if((e < extremalV2) || (e > extremalV3)) {
                    result = new Vector(2);
                    result[0] = bisection.Solve(-MEZ, extremalBeta[0]);
                    result[1] = bisection.Solve(extremalBeta[0], MEZ);
                }
                else {
                    result = new Vector(4);
                    if(extremalBeta[0] < extremalBeta[1]) {
                        result[0] = bisection.Solve(-MEZ, extremalBeta[0]);
                        result[1] = bisection.Solve(extremalBeta[0], extremalBeta[2]);
                        result[2] = bisection.Solve(extremalBeta[2], extremalBeta[1]);
                        result[3] = bisection.Solve(extremalBeta[1], MEZ);
                    }
                    else {
                        result[0] = bisection.Solve(-MEZ, extremalBeta[1]);
                        result[1] = bisection.Solve(extremalBeta[1], extremalBeta[2]);
                        result[2] = bisection.Solve(extremalBeta[2], extremalBeta[0]);
                        result[3] = bisection.Solve(extremalBeta[0], MEZ);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Poèítá øešení rovnice V(beta) = E a vrací všechny nenulové koøeny beta
        /// </summary>
        /// <param name="gamma">gamma</param>
        /// <param name="E">Energie</param>
        public Vector NonzeroRoots(double e, double gamma) {
            Vector r = this.Roots(e, gamma);

            int zeroRoots = 0;

            for(int i = 0; i < r.Length - zeroRoots; i++) {
                if(System.Math.Abs(r[i]) < zeroValue) {
                    zeroRoots++;

                    for(int j = i + 1; j < r.Length; j++)
                        r[j - 1] = r[j];

                    i--;
                }

            }

            r.Length = r.Length - zeroRoots;

            return r;
        }

        /// <summary>
        /// Vytvoøí tøídu ekvipotenciální køivky
        /// </summary>
        /// <param name="e">Energie</param>
        private Contour CreateContour(double e) {
            Contour contour = new Contour();

            contour.Begin();
            for(int i = 1; i < equipotentialDivision; i++) {
                double gamma = i * System.Math.PI / equipotentialDivision;
                Vector roots = this.NonzeroRoots(e, gamma);
                contour.Add(roots, gamma);
            }
            contour.End();

            contour.RemoveShort();
            contour.Join();

            return contour;
        }

        /// <summary>
        /// Vypoèítá ekvipotenciální køivky
        /// </summary>
        /// <param name="e">Zadaná energie</param>
        public PointVector[] EquipotentialContours(double e) {
            Contour contour = this.CreateContour(e);
            return contour.GetPointVector();
        }

        /// <summary>
        /// Vypoèítá ekvipotenciální køivky
        /// </summary>
        /// <param name="e">Enegie</param>
        /// <param name="n">Maximální poèet bodù v jedné køivce</param>
        /// <returns></returns>
        public PointVector[] EquipotentialContours(double e, int n) {
            Contour contour = this.CreateContour(e);
            return contour.GetPointVector(n);
        }

        /// <summary>
        /// Napoèítá matici V v promìnných beta, gamma
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="beta">Souøadnice beta</param>
        /// <param name="gamma">Souøadnice gamma</param>
        public Matrix VMatrixBG(double e, double beta, double gamma) {
            return this.VMatrix(e, beta * System.Math.Cos(gamma), beta * System.Math.Sin(gamma));
        }

        /// <summary>
        /// Napoèítá matici V
        /// (podle PRL 98, 234301 (2007))
        /// </summary>
        /// <param name="e">Energie</param>
        public Matrix VMatrix(double e, double x, double y) {
            double b = x * x + y * y;

            double vx = 2.0 * this.A * x + 3.0 * this.B * (x * x - y * y) + 4.0 * this.C * x * b;
            double vy = 2.0 * y * (this.A - 3.0 * this.B * x + 2.0 * this.C * b);

            double vxx = 2.0 * this.A + 6.0 * this.B * x + 4.0 * this.C * (3.0 * x * x + y * y);
            double vxy = 8.0 * this.C * x * y - 6.0 * this.B * y;
            double vyy = 2.0 * this.A - 6.0 * this.B * x + 4.0 * this.C * (x * x + 3.0 * y * y);

            double a = 3.0 / System.Math.Abs((2.0 * (e - this.V(x, y))));

            Matrix result = new Matrix(2);
            result[0, 0] = (a * vx * vx + vxx) / this.K;
            result[0, 1] = (a * vx * vy + vxy) / this.K;
            result[1, 0] = result[0, 1];
            result[1, 1] = (a * vy * vy + vyy) / this.K;

            return result;
        }

        /// <summary>
        /// Najde bod, kde se mìní znaménko nejnižší vlastní hodnoty z kladné na zápornou
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="gamma">Úhel gamma</param>
        /// <param name="lowerEV">True pokud poèitáme nižší vlastní hodnotu</param>
        private Vector VMatrixPMChange(double e, double gamma, bool lowerEV) {
            Vector roots = this.Roots(e, gamma);

            double betamax = 2.0;

            int rlength = roots.Length;
            if(rlength > 0) 
                betamax = 2.0 * System.Math.Max(System.Math.Abs(roots.FirstItem), System.Math.Abs(roots.LastItem));

            VMatrixBisectionFunction vf = new VMatrixBisectionFunction(this, gamma, e, lowerEV);
            Bisection bisection = new Bisection(vf.Function);

            ArrayList a = new ArrayList();

            int l = vmatrixzerodivision / 20;
            double last = vf.Function(-betamax);

            for(int i = 1; i < l; i++) {
                double x = betamax * ((double)(2 * i - l + 1) / l);
                double d = vf.Function(x);
                if((d <= 0 && last >= 0) || (d >= 0 && last <= 0))
                    a.Add(bisection.Solve(betamax * ((double)(2 * i - l - 1) / l), x));
                last = d;
            }

            int count = a.Count;
            Vector result = new Vector(count);
            int index = 0;

            foreach(double d in a)
                result[index++] = d;

            return result;
        }

        /// <summary>
        /// Vypoèítá oblast se zápornými vlastními èísly V matice 
        /// (podle PRL 98, 234301 (2007))
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="n">Poèet bodù v jedné køivce</param>
        /// <param name="lowerEV">True pokud poèítáme nižší vlastní hodnotu</param>
        public PointVector[] VMatrixContours(double e, int n, bool lowerEV) {
            Contour contour = new Contour(6);

            contour.Begin();
            for(int i = 1; i < equipotentialDivision; i++) {
                double gamma = i * System.Math.PI / equipotentialDivision;
                Vector roots = this.VMatrixPMChange(e, gamma, lowerEV);
                contour.Add(roots, gamma);
            }
            contour.End();

            contour.RemoveShort();
            contour.Join();

            return n > 0 ? contour.GetPointVector(n) : contour.GetPointVector();
        }

        private const double zeroValue = 10E-7;
        private const int equipotentialDivision = 4001;
        private const int vmatrixzerodivision = 4001;
    }
}
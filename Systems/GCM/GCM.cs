using System;
using System.Collections;
using System.Text;

using PavelStransky.Math;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
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
        /// <param name="div">Dìlení intervalu 2pi (celkový poèet bodù výpoètu)</param>
        private Contour CreateContour(double e, int div) {
            Contour contour = new Contour();

            if(div <= 0)
                div = defaultDivision;

            contour.Begin();
            for(int i = 1; i < div; i++) {
                double gamma = i * System.Math.PI / div;
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
        /// <param name="e">Enegie</param>
        /// <param name="n">Maximální poèet bodù v jedné køivce</param>
        /// <param name="div">Dìlení intervalu 2pi (celkový poèet bodù výpoètu)</param>
        public PointVector[] EquipotentialContours(double e, int n, int div) {
            Contour contour = this.CreateContour(e, div);
            return contour.GetPointVector(n);
        }

        private const double zeroValue = 10E-7;
        protected const int defaultDivision = 4001;
    }
}
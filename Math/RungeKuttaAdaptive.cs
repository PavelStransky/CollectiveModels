using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Tøída, poèátající adaptivní RK metodou; podle NR, pomocí metody 4. a 5. øádu urèí chybu výpoètu
    /// Jako hodnotící parametr uvažuje i energii
    /// </summary>
    public class RungeKuttaAdaptive: RungeKutta {
        private Vector scale;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="precision">Pøesnost výpoètu</param>
        public RungeKuttaAdaptive(IDynamicalSystem dynamicalSystem, double precision)
            : base(dynamicalSystem, precision != 0.0 ? precision : defaultPrecision) {
        }

        /// <summary>
        /// Jeden krok výpoètu
        /// </summary>
        /// <param name="x">Vektor v èase x</param>
        /// <param name="step">Krok</param>
        /// <param name="newStep">Nový krok</param>
        /// <returns>Vypoèítaný pøírùstek</returns>
        public override Vector Step(Vector x, ref double step, out double newStep) {
            VectorFunction equation = this.dynamicalSystem.Equation;

            do {
                Matrix bstep = b * step;

                Vector rightSide0 = equation(x);
                Vector rightSide1 = equation(Vector.Summarize(x, bstep[0, 0], rightSide0));
                Vector rightSide2 = equation(Vector.Summarize(x, bstep[1, 0], rightSide0, bstep[1, 1], rightSide1));
                Vector rightSide3 = equation(Vector.Summarize(x, bstep[2, 0], rightSide0, bstep[2, 1], rightSide1, bstep[2, 2], rightSide2));
                Vector rightSide4 = equation(Vector.Summarize(x, bstep[3, 0], rightSide0, bstep[3, 1], rightSide1, bstep[3, 2], rightSide2, bstep[3, 3], rightSide3));
                Vector rightSide5 = equation(Vector.Summarize(x, bstep[4, 0], rightSide0, bstep[4, 1], rightSide1, bstep[4, 2], rightSide2, bstep[4, 3], rightSide3, bstep[4, 4], rightSide4));

                Vector error = step * Vector.Summarize(dc[0], rightSide0, dc[2], rightSide2, dc[3], rightSide3, dc[4], rightSide4, dc[5], rightSide5);

                Vector verror = Vector.ItemDiv(error, scale);
                double maxerror = verror.Abs().Max() / this.precision;

                if(maxerror <= 1.0) {
                    if(maxerror > errorCon)
                        newStep = safety * step * System.Math.Pow(maxerror, powerGrow);
                    else
                        newStep = step * 5.0;

                    return step * Vector.Summarize(c5[0], rightSide0, c5[2], rightSide2, c5[3], rightSide3, c5[5], rightSide5);
                }

                newStep = safety * step * System.Math.Pow(maxerror, powerShrink);
                newStep = System.Math.Max(newStep, maxStepChange * step);

                step = newStep;
            } while(true);
        }

        /// <summary>
        /// Øeší rovnici s poèáteèními podmínkami po èas time
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        public override void Init(Vector initialX) {
            // Inicializujeme meze
            Vector bounds = this.dynamicalSystem.Bounds(this.dynamicalSystem.E(initialX));
            
            this.scale = new Vector(2 * this.dynamicalSystem.DegreesOfFreedom);
            for(int i = 0; i < 2 * this.dynamicalSystem.DegreesOfFreedom; i++)
                this.scale[i] = bounds[2 * i + 1] - bounds[2 * i];

            base.Init(initialX);
        }

        /// <summary>
        /// Statický konstruktor (inicializuje konstanty)
        /// </summary>
        static RungeKuttaAdaptive() {
            a = new Vector(5);
            a[0] = 0.2;
            a[1] = 0.3;
            a[2] = 0.6;
            a[3] = 1.0;
            a[4] = 7.0 / 8.0;

            b = new Matrix(5, 5);
            for(int i = 0; i < 4; i++)
                for(int j = i + 1; j < 5; j++)
                    b[i, j] = 0;
            b[0, 0] = 0.2;
            b[1, 0] = 0.075;
            b[1, 1] = 0.225;
            b[2, 0] = 0.3;
            b[2, 1] = -0.9;
            b[2, 2] = 1.2;
            b[3, 0] = -11.0 / 54.0;
            b[3, 1] = 2.5;
            b[3, 2] = -70.0 / 27.0;
            b[3, 3] = 35.0 / 27.0;
            b[4, 0] = 1631.0 / 55296.0;
            b[4, 1] = 175.0 / 512.0;
            b[4, 2] = 575.0 / 13824.0;
            b[4, 3] = 44275.0 / 110592.0;
            b[4, 4] = 253.0 / 4096.0;

            c4 = new Vector(6);
            c4[0] = 2825.0 / 27648.0;
            c4[1] = 0;
            c4[2] = 18575.0 / 48384.0;
            c4[3] = 13525.0 / 55296.0;
            c4[4] = 277.0 / 14336.0;
            c4[5] = 0.25;

            c5 = new Vector(6);
            c5[0] = 37.0 / 378.0;
            c5[1] = 0;
            c5[2] = 250.0 / 621.0;
            c5[3] = 125.0 / 594.0;
            c5[4] = 0;
            c5[5] = 512.0 / 1771.0;

            dc = c5 - c4;
        }

        private static Vector a;
        private static Matrix b;
        private static Vector c4;
        private static Vector c5;
        private static Vector dc;

        private const double powerShrink = -0.25;
        private const double powerGrow = -0.2;
        private const double safety = 0.9;
        private const double maxStepChange = 0.1;
        private double errorCon = System.Math.Pow((5.0 / safety), 1.0 / powerGrow);

        private const double defaultPrecision = 1E-12;
    }
}
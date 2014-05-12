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
        private bool autoScale = false;
        private IDynamicalSystem dynamicalSystem;

        private double precision;
        
        /// <summary>
        /// Pøesnost výpoètu
        /// </summary>
        public override double Precision { get { return this.precision; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="precision">Pøesnost výpoètu</param>
        public RungeKuttaAdaptive(IDynamicalSystem dynamicalSystem, double precision)
            : base(dynamicalSystem) {
            this.dynamicalSystem = dynamicalSystem;
            this.SetPrecision(precision);
        }
        
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="equation">Pravá strana rovnic</param>
        /// <param name="precision">Pøesnost výpoètu</param>
        public RungeKuttaAdaptive(VectorFunction equation, double precision)
            : base(equation) {

            this.autoScale = true;
            this.SetPrecision(precision);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="equation">Pravá strana rovnic</param>
        public RungeKuttaAdaptive(VectorFunction equation)
            : this(equation, defaultPrecision) {
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="precision">Pøesnost výpoètu</param>
        public RungeKuttaAdaptive(IDynamicalSystem dynamicalSystem)
            : this(dynamicalSystem, defaultPrecision) {
        }

        /// <summary>
        /// Nastaví pøesnost: pokud je pøesnost nekladná, bude to defaultPrecision
        /// </summary>
        /// <param name="precision">Pøeesnost</param>
        public void SetPrecision(double precision) {
            if(precision <= 0.0)
                this.precision = defaultPrecision;
            else
                this.precision = precision;
        }

        /// <summary>
        /// Nastaví parametry výpoètu
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        public override void Init(Vector initialX) {
            if(this.dynamicalSystem != null) {
                Vector bounds = this.dynamicalSystem.Bounds(this.dynamicalSystem.E(initialX));
                this.scale = new Vector(2 * this.dynamicalSystem.DegreesOfFreedom);
                for(int i = 0; i < 2 * this.dynamicalSystem.DegreesOfFreedom; i++)
                    this.scale[i] = bounds[2 * i + 1] - bounds[2 * i];
            }

            base.Init(initialX);
        }

        /// <summary>
        /// Jeden krok výpoètu
        /// </summary>
        /// <param name="x">Vektor v èase x</param>
        /// <param name="step">Krok</param>
        /// <param name="newStep">Nový krok</param>
        /// <returns>Vypoèítaný pøírùstek</returns>
        public override Vector Step(Vector x, ref double step, out double newStep) {
            VectorFunction equation = this.equation;

            if(this.autoScale)
                this.SetAutoScale(x);

            do {
                Matrix bstep = b * step;

                Vector rightSide0 = equation(x);
                Vector rightSide1 = equation(Vector.Summarize(x, bstep[0, 0], rightSide0));
                Vector rightSide2 = equation(Vector.Summarize(x, bstep[1, 0], rightSide0, bstep[1, 1], rightSide1));
                Vector rightSide3 = equation(Vector.Summarize(x, bstep[2, 0], rightSide0, bstep[2, 1], rightSide1, bstep[2, 2], rightSide2));
                Vector rightSide4 = equation(Vector.Summarize(x, bstep[3, 0], rightSide0, bstep[3, 1], rightSide1, bstep[3, 2], rightSide2, bstep[3, 3], rightSide3));
                Vector rightSide5 = equation(Vector.Summarize(x, bstep[4, 0], rightSide0, bstep[4, 1], rightSide1, bstep[4, 2], rightSide2, bstep[4, 3], rightSide3, bstep[4, 4], rightSide4));

                Vector error = step * Vector.Summarize(dc[0], rightSide0, dc[2], rightSide2, dc[3], rightSide3, dc[4], rightSide4, dc[5], rightSide5);

                Vector verror = Vector.ItemDiv(error, this.scale);
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

                if(double.IsNaN(newStep))
                    return x;

                step = newStep;
            } while(true);
        }

        /// <summary>
        /// Nastaví manuálnì škálu výpoètu (bez volání funkce Init)
        /// </summary>
        /// <param name="scale">škála</param>
        /// <param name="autoScale">true, pokud budeme nastavovat škálu automaticky v prùbìhu výpoètu</param>
        public void SetScale(Vector scale, bool autoScale) {
            this.scale = scale;
            this.autoScale = autoScale;
        }

        /// <summary>
        /// Nastaví automaticky škálu
        /// </summary>
        /// <param name="x">Vektor x</param>
        private void SetAutoScale(Vector x) {
            int length = this.scale.Length;

            if(this.scale == null)
                this.scale = new Vector(x.Length);

            for(int i = 0; i < length; i++)
                this.scale[i] = System.Math.Max(this.scale[i], System.Math.Abs(x[i]));
        }

        /// <summary>
        /// Statický konstruktor (inicializuje konstanty)
        /// </summary>
        /// <remarks>Koeficienty podle Wikipedie</remarks>
        static void RungeKuttaAdaptiveKonstants() {
            a = new Vector(5);
            a[0] = 1.0 / 4.0;
            a[1] = 3.0 / 8.0;
            a[2] = 12.0 / 13.0;
            a[3] = 1.0;
            a[4] = 1.0 / 2.0;

            b = new Matrix(5, 5);
            for(int i = 0; i < 4; i++)
                for(int j = i + 1; j < 5; j++)
                    b[i, j] = 0;

            b[0, 0] = 1.0 / 4.0;
            b[1, 0] = 3.0 / 32.0;
            b[1, 1] = 9.0 / 32.0;
            b[2, 0] = 1932.0 / 2197.0;
            b[2, 1] = -7200.0 / 2197.0;
            b[2, 2] = 7296.0 / 2197.0;
            b[3, 0] = 439.0 / 216.0;
            b[3, 1] = -8.0;
            b[3, 2] = 3680.0 / 513.0;
            b[3, 3] = -845.0 / 4104.0;
            b[4, 0] = -8.0 / 27.0;
            b[4, 1] = 2.0;
            b[4, 2] = -3544.0 / 2565.0;
            b[4, 3] = 1859.0 / 4104.0;
            b[4, 4] = -11.0 / 40.0;

            c4 = new Vector(6);
            c4[0] = 25.0/216.0;
            c4[1] = 0;
            c4[2] = 1408.0 / 2565.0;
            c4[3] = 2197.0/4104.0;
            c4[4] = -1.0/5.0;
            c4[5] = 0;

            c5 = new Vector(6);
            c5[0] = 16.0 / 135.0;
            c5[1] = 0;
            c5[2] = 6656.0 / 12825.0;
            c5[3] = 28561.0 / 56430.0;
            c5[4] = -9.0 / 50.0;
            c5[5] = 2.0 / 55.0;

            dc = c5 - c4;
        }

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

        private const double defaultPrecision = 1E-14;
    }
}
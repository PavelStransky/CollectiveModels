using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Urèí, zda zadaná trajektorie je regulární nebo chaotická na základì metody SALI
    /// (Smaller Alignment Index)
    /// </summary>
    public class SALI:DynamicalSystem {
        // Aktuální souøadnice a rychlosti
        protected Vector x;

        /// <summary>
        /// Rovnice pro výpoèet deviace
        /// </summary>
        /// <param name="w">Vektor deviace</param>
        protected Vector DeviationEquation(Vector w) {
            return this.dynamicalSystem.Jacobian(this.x) * w;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        public SALI(IDynamicalSystem dynamicalSystem)
            : base(dynamicalSystem) {
        }

        /// <summary>
        /// Alignment index SALI
        /// </summary>
        /// <param name="w1">Vektor první odchylky</param>
        /// <param name="w2">Vektor druhé odchylky</param>
        protected double AlignmentIndex(Vector w1, Vector w2) {
            Vector antiparalel = w1 - w2;
            Vector paralel = w1 + w2;
            
            return System.Math.Min(paralel.EuklideanNorm(), antiparalel.EuklideanNorm());
        }

        /// <summary>
        /// Vypoèítá závislost SALI na èase
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        /// <param name="time">Èas</param>
        public PointVector TimeDependence(Vector initialX, double time) {
            PointVector result = new PointVector(defaultNumPoints);
            RungeKutta rkx = new RungeKutta(this.dynamicalSystem.Equation, defaultPrecision);
            RungeKutta rkw = new RungeKutta(new VectorFunction(this.DeviationEquation), defaultPrecision); 

            int finished = 0;

            this.x = initialX;
            
            Vector w1 = new Vector(initialX.Length);
            Vector w2 = new Vector(initialX.Length);
            w1[0] = 1;
            w2[initialX.Length / 2] = 1;

            double step = defaultPrecision;
            double t = 0;

            do {
                for(int i = 0; i < 1000; i++) {
                    this.x += rkx.Step(this.x, ref step, out step);
                    w1 += rkw.Step(w1, ref step, out step);
                    w2 += rkw.Step(w2, ref step, out step);

                    t += step;
                }

                w1 = w1.EuklideanNormalization();
                w2 = w2.EuklideanNormalization();

                double sali = this.AlignmentIndex(w1, w2);

                result[finished].X = t;
                result[finished].Y = sali;
                finished++;

                if(finished >= result.Length)
                    result.Length = result.Length * 3 / 2;

            } while(t < time);

            result.Length = finished;

            return result;
        }

        /// <summary>
        /// Vypoèítá èas, ve kterém SALI == 0
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        public double TimeZero(Vector initialX) {
            RungeKutta rkx = new RungeKutta(this.dynamicalSystem.Equation, defaultPrecision);
            RungeKutta rkw = new RungeKutta(new VectorFunction(this.DeviationEquation), defaultPrecision);

            this.x = initialX;

            Vector w1 = new Vector(initialX.Length);
            Vector w2 = new Vector(initialX.Length);
            w1[0] = 1;
            w2[initialX.Length / 2] = 1;

            double step = defaultPrecision;
            double time = 0;

            do {
                this.x += rkx.Step(this.x, ref step, out step);
                w1 += rkw.Step(w1, ref step, out step);
                w2 += rkw.Step(w2, ref step, out step);

                time += step;

                w1 = w1.EuklideanNormalization();
                w2 = w2.EuklideanNormalization();

            } while(this.AlignmentIndex(w1, w2) > minSALI && time < maxTime);

            return time;
        }

        /// <summary>
        /// Vrátí true, pokud daná trajektorie je podle SALI regulární
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        public bool IsRegular(Vector initialX) {
            if(this.TimeZero(initialX) >= maxTime)
                return true;
            else
                return false;
        }

        protected const double defaultPrecision = 1E-3;
        protected const int defaultNumPoints = 500;
        protected const double maxTime = 500;
        protected const double minSALI = 10E-7;
    }
}

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
        public SALI(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod)
            : base(dynamicalSystem, precision, rkMethod) {
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
            RungeKutta rkw = new RungeKutta(new VectorFunction(this.DeviationEquation), defaultPrecision);

            int finished = 0;

            this.x = initialX;
            
            Vector w1 = new Vector(initialX.Length);
            Vector w2 = new Vector(initialX.Length);
            w1[0] = 1;
            w2[initialX.Length / 2] = 1;

            double step = this.rungeKutta.Precision;
            double t = 0;

            this.rungeKutta.Init(initialX);

            do {
                for(int i = 0; i < 1000; i++) {
                    double newStep, tStep;

                    this.x += this.rungeKutta.Step(this.x, ref step, out newStep);
                    w1 += rkw.Step(w1, ref step, out tStep);
                    w2 += rkw.Step(w2, ref step, out tStep);

                    t += step;

                    step = newStep;
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
        /// Vrátí true, pokud daná trajektorie je podle SALI regulární
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        public bool IsRegular(Vector initialX) {
            RungeKutta rkw = new RungeKutta(new VectorFunction(this.DeviationEquation), defaultPrecision);

            this.x = initialX;

            Vector w1 = new Vector(initialX.Length);
            Vector w2 = new Vector(initialX.Length);
            w1[0] = 1;
            w2[initialX.Length / 2] = 1;

            double step = defaultPrecision;
            double time = 0;

            double cumulLogSALI = 0;
            Vector logSALIQueue = new Vector(window);    // Realizuje frontu
            int iQueue = 0;

            int i1 = (int)(1.0 / defaultPrecision);
            bool init = true;

            double timeM = initTime;

            this.rungeKutta.Init(initialX);

            do {
                for(int i = 0; i < i1; i++) {
                    double newStep, tStep;

                    this.x += this.rungeKutta.Step(this.x, ref step, out newStep);
                    w1 += rkw.Step(w1, ref step, out tStep);
                    w2 += rkw.Step(w2, ref step, out tStep);

                    time += step;

                    step = newStep;
                }

                w1 = w1.EuklideanNormalization();
                w2 = w2.EuklideanNormalization();

                double ai = this.AlignmentIndex(w1, w2);
                double logAI = (ai <= 0.0 ? 0.0 : -System.Math.Log10(ai));
                double logAIw = logAI / window;
                cumulLogSALI += logAIw - logSALIQueue[iQueue];
                logSALIQueue[iQueue] = logAIw;
                iQueue = (iQueue + 1) % window;

                // Nejprve napoèítáme initTime jednotek, než zaèneme vyøazovat
                if(init && iQueue < initTime)
                    continue;

                init = false;

                // Pokud se objeví jeden bod SALI < 4, pak je trajektorie na rozhraní. Posuneme dále DecisionPoint
                if(logAI > 3.0)
                    timeM = 1000;

                if(cumulLogSALI > 4.0)
                    return false;
                if(cumulLogSALI < (time - timeM) / 500.0)
                    return true;
            } while(true);
        }

        protected const double defaultPrecision = 1E-3;
        protected const int defaultNumPoints = 500;
        protected const double maxTime = 500;
        protected const double minSALI = 10E-7;

        protected const int window = 400;
        protected const int initTime = 100;
    }
}

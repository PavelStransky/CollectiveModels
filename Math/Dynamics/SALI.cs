using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Urèí, zda zadaná trajektorie je regulární nebo chaotická na základì metody SALI
    /// (Smaller Alignment Index)
    /// </summary>
    public class SALI: DynamicalSystem {
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
        public SALI(IDynamicalSystem dynamicalSystem, double precision)
            : base(dynamicalSystem, precision != 0.0 ? precision : defaultPrecision, RungeKuttaMethods.Adapted) {
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
            RungeKuttaAdaptive rkw = new RungeKuttaAdaptive(new VectorFunction(this.DeviationEquation), this.rungeKutta.Precision / 100.0);

            int finished = 0;

            this.x = initialX;
            
            Vector w1 = new Vector(initialX.Length);
            Vector w2 = new Vector(initialX.Length);
            w1[0] = 1;
            w2[initialX.Length / 2] = 1;

            double step = this.rungeKutta.Precision;
            double t = 0;

            this.rungeKutta.Init(initialX);
            
            Vector scale = new Vector(initialX.Length);
            for(int i = 0; i < scale.Length; i++)
                scale[i] = 1.0;

            rkw.SetScale(scale, true);

            do {
                for(int i = 0; i < 1000; i++) {
                    double newStep, tStep1, tStep2;

                    Vector addX = this.rungeKutta.Step(this.x, ref step, out newStep);

                    double oldStepW1 = step;
                    Vector addW1 = rkw.Step(w1, ref step, out tStep1);

                    double oldStepW2 = step;
                    Vector addW2 = rkw.Step(w2, ref step, out tStep2);

                    if(step != oldStepW1 || step != oldStepW2) {
                        step = System.Math.Min(System.Math.Min(step, oldStepW1), oldStepW2);
                        
                        addX = this.rungeKutta.Step(this.x, ref step, out newStep);
                        addW1 = rkw.Step(w1, ref step, out tStep1);
                        addW2 = rkw.Step(w2, ref step, out tStep2);
                    }

                    this.x += addX;
                    w1 += addW1;
                    w2 += addW2;

                    t += step;

                    step = System.Math.Min(System.Math.Min(newStep, tStep1), tStep2);
                }

//                w1 = w1.EuklideanNormalization();
//                w2 = w2.EuklideanNormalization();

                double sali = this.AlignmentIndex(w1.EuklideanNormalization(), w2.EuklideanNormalization());

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
            RungeKutta rkw = new RungeKutta(new VectorFunction(this.DeviationEquation), this.rungeKutta.Precision);

            this.x = initialX;

            Vector w1 = new Vector(initialX.Length);
            Vector w2 = new Vector(initialX.Length);
            w1[0] = 1;
            w2[initialX.Length / 2] = 1;

            double step = this.rungeKutta.Precision;
            double time = 0;

            MeanQueue queue = new MeanQueue(window);

            int i1 = (int)(1.0 / step);

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
                double logAI = (ai <= 0.0 ? 20.0 : -System.Math.Log10(ai));
                queue.Set(logAI);

                double meanSALI = queue.Mean;

                if(meanSALI > 6.0 + time / 1000.0)
                    return false;
                if(meanSALI < (time - 1000.0) / 300.0)
                    return true;
            } while(true);
        }

        protected const double defaultPrecision = 1E-12;
        protected const int defaultNumPoints = 500;
        protected const double maxTime = 500;
        
        protected const int window = 10;
    }

    /// <summary>
    /// Realizuje frontu k vystøedovávání
    /// </summary>
    public class MeanQueue {
        private Vector queue;
        private int index;

        /// <summary>
        /// Délka fronty
        /// </summary>
        public int Length { get { return this.queue.Length; } }

        /// <summary>
        /// Støední hodnota
        /// </summary>
        public double Mean { get { return this.queue.Mean(); } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="length">Délka fronty</param>
        public MeanQueue(int length) {
            this.queue = new Vector(length);
            this.index = 0;
        }

        /// <summary>
        /// Nastaví hodnotu na aktuální pozici
        /// </summary>
        /// <param name="value">Hodnota</param>
        /// <returns>Hodnota z aktuální pozice</returns>
        public void Set(double value) {
            this.queue[this.index++] = value;
            this.index %= this.Length;
        }
    }
}

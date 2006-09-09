using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Poèítá Poincarého øez danou rovinou (rovina dána vektorem (a, b, c, ...) = ax + by + ... == 0)
    /// a zaznamenává souøadnice o indexech i1, i2
    /// </summary>
    public class PoincareSection {
        // Runge-Kutta pro výpoèet
        private RungeKutta rungeKutta;

        // Urèení roviny
        private Vector plane;

        // Indexy
        private int i1, i2;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="rungeKutta">Runge-Kutta pro výpoèet</param>
        /// <param name="plane">Rovina øezu</param>
        /// <param name="i1">Zaznamenávaný index x</param>
        /// <param name="i2">Zaznamenávaný index y</param>
        public PoincareSection(RungeKutta rungeKutta, Vector plane, int i1, int i2) {
            this.rungeKutta = rungeKutta;
            this.plane = plane;
            this.i1 = i1;
            this.i2 = i2;
        }

        /// <summary>
        /// Vypoèítá Poincarého øez
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        /// <param name="numPoints">Poèet bodù øezu</param>
        public PointVector Compute(Vector initialX, int numPoints) {
            PointVector result = new PointVector(numPoints);
            int finished = 0;
            Vector x = initialX;
            double sp = x * this.plane;

            double step = this.rungeKutta.Precision;
            double time = 0;

            do {
                double newStep;
                Vector newx = x + this.rungeKutta.Step(x, ref step, out newStep);
                time += step;
                step = newStep;

                double newsp = newx * this.plane;
                if(newsp * sp <= 0) {
                    Vector v = (newx - x) / (newsp - sp) * sp + x;
                    result[finished].X = v[this.i1];
                    result[finished].Y = v[this.i2];
                    finished++;
                }
                sp = newsp;
                x = newx;
            } while(finished < numPoints);

            return result;
        }
    }
}

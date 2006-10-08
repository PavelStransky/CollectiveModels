using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Poèítá Poincarého øez danou rovinou (rovina dána vektorem (a, b, c, ...) = ax + by + ... == 0)
    /// a zaznamenává souøadnice o indexech i1, i2
    /// </summary>
    public class PoincareSection: DynamicalSystem {
        // Urèení roviny
        private Vector plane;

        // Èíslo, které dourèuje rovinu (plane * x == crossPoint)
        private double crossPoint;

        // Indexy
        private int i1, i2;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="plane">Rovina øezu</param>
        /// <param name="i1">Zaznamenávaný index x</param>
        /// <param name="i2">Zaznamenávaný index y</param>
        /// <param name="precision">Pøesnost výsledku</param>
        /// <param name="rkMethod">Metoda k výpoètu RK</param>
        public PoincareSection(IDynamicalSystem dynamicalSystem, Vector plane, int i1, int i2, double precision, RungeKuttaMethods rkMethod)
            : base(dynamicalSystem, precision, rkMethod) {
            if(plane.Length % 2 == 1) {
                this.crossPoint = plane.LastItem;
                plane.Length = plane.Length - 1;
            }
            else {
                this.crossPoint = 0;
            }
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

            this.rungeKutta.Init(initialX);

            do {
                double newStep;
                Vector newx = x + this.rungeKutta.Step(x, ref step, out newStep);
                time += step;
                step = newStep;

                double newsp = newx * this.plane;
                if((newsp <= this.crossPoint && sp > this.crossPoint) ||
                    (sp <= this.crossPoint && newsp > this.crossPoint)) {
                    Vector v = (x - newx) * (sp / (newsp - sp)) + x;
                    result[finished].X = v[this.i1];
                    result[finished].Y = v[this.i2];
                    finished++;
                }
                sp = newsp;
                x = newx;

                if (finished == 0 && time > maxTimeWithoutCross)
                    throw new PoincareSectionException(errorMessageNoCross);

            } while(finished < numPoints);

            return result;
        }

        private const double maxTimeWithoutCross = 500.0;

        private const string errorMessageNoCross = "Trajektorie s danými poèáteèními podmínkami neprotíná rovinu øezu!";
    }

    /// <summary>
    /// Výjimka ve tøídì Poincaré
    /// </summary>
    public class PoincareSectionException : ApplicationException {
        /// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public PoincareSectionException(string message) : base(message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
        public PoincareSectionException(string message, Exception innerException) : base(message, innerException) { }
    }
}

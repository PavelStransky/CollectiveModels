using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Poèítá Poincarého øez danou rovinou (rovina dána vektorem (a, b, c, ...) = ax + by + ... == 0)
    /// a zaznamenává souøadnice o indexech i1, i2
    /// </summary>
    public class PoincareSection {
        // Systém
        private IDynamicalSystem dynamicalSystem;

        // RungeKutta
        private RungeKutta rungeKutta;

        // Pøesnost výpoètu
        private double precision;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="precision">Pøesnost výsledku</param>
        /// <param name="rkMethod">Metoda k výpoètu RK</param>
        public PoincareSection(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod) {
            this.dynamicalSystem = dynamicalSystem;
            this.rungeKutta = RungeKutta.CreateRungeKutta(dynamicalSystem, precision, rkMethod);
            this.precision = precision > 0.0 ? precision : this.rungeKutta.Precision;
        }

        /// <summary>
        /// Vypoèítá Poincarého øez
        /// </summary>
        /// <param name="plane">Rovina øezu</param>
        /// <param name="i1">Zaznamenávaný index x</param>
        /// <param name="i2">Zaznamenávaný index y</param>
        /// <param name="initialX">Poèáteèní podmínky</param>
        /// <param name="numPoints">Poèet bodù øezu</param>
        /// <param name="oneOrientation">True, pokud chceme zobrazovat jen jednu orientaci prùchodu rovinou</param>
        public PointVector Compute(Vector plane, int i1, int i2, Vector initialX, int numPoints, bool oneOrientation) {
            // Èíslo, které dourèuje rovinu (plane * x == crossPoint)
            double crossPoint;

            if(plane.Length % 2 == 1) {
                crossPoint = plane.LastItem;
                plane.Length = plane.Length - 1;
            }
            else {
                crossPoint = 0;
            }
            
            PointVector result = new PointVector(numPoints);
            int finished = 0;
            Vector x = initialX;
            double sp = x * plane;

            double step = this.precision;
            double time = 0;

            this.rungeKutta.Init(initialX);

            do {
                double newStep;
                Vector newx = x + this.rungeKutta.Step(x, ref step, out newStep);
                time += step;
                step = newStep;

                double newsp = newx * plane;

                // jedna èi obì orientace prùchodu
                if((newsp <= crossPoint && sp > crossPoint) ||
                    (!oneOrientation && sp <= crossPoint && newsp > crossPoint)) {
                    Vector v = (x - newx) * (sp / (newsp - sp)) + x;
                    result[finished].X = v[i1];
                    result[finished].Y = v[i2];
                    finished++;
                }
                sp = newsp;
                x = newx;

                if (finished == 0 && time > maxTimeWithoutCross)
                    throw new PoincareSectionException(Messages.EMNoCross);

            } while(finished < numPoints);

            return result;
        }

        /// <summary>
        /// True, pokud trajektorie protíná rovinu
        /// </summary>
        /// <param name="plane">Rovina øezu</param>
        /// <param name="i1">Zaznamenávaný index x</param>
        /// <param name="i2">Zaznamenávaný index y</param>
        /// <param name="initialX">Poèáteèní podmínky</param>
        public bool CrossPlane(Vector plane, int i1, int i2, Vector initialX) {
            // Èíslo, které dourèuje rovinu (plane * x == crossPoint)
            double crossPoint;

            if(plane.Length % 2 == 1) {
                crossPoint = plane.LastItem;
                plane.Length = plane.Length - 1;
            }
            else {
                crossPoint = 0;
            }

            Vector x = initialX;
            double sp = x * plane;

            double step = this.precision;
            double time = 0;

            this.rungeKutta.Init(initialX);

            do {
                double newStep;
                Vector newx = x + this.rungeKutta.Step(x, ref step, out newStep);
                time += step;
                step = newStep;

                double newsp = newx * plane;

                // jedna èi obì orientace prùchodu
                if((newsp <= crossPoint && sp > crossPoint) || (sp <= crossPoint && newsp > crossPoint))
                    return true;

                sp = newsp;
                x = newx;

            } while(time < maxTimeWithoutCross);

            return false;
        }

        private const double maxTimeWithoutCross = 500.0;
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

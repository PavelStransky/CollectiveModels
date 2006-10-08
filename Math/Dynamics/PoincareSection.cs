using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Po��t� Poincar�ho �ez danou rovinou (rovina d�na vektorem (a, b, c, ...) = ax + by + ... == 0)
    /// a zaznamen�v� sou�adnice o indexech i1, i2
    /// </summary>
    public class PoincareSection: DynamicalSystem {
        // Ur�en� roviny
        private Vector plane;

        // ��slo, kter� dour�uje rovinu (plane * x == crossPoint)
        private double crossPoint;

        // Indexy
        private int i1, i2;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        /// <param name="plane">Rovina �ezu</param>
        /// <param name="i1">Zaznamen�van� index x</param>
        /// <param name="i2">Zaznamen�van� index y</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="rkMethod">Metoda k v�po�tu RK</param>
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
        /// Vypo��t� Poincar�ho �ez
        /// </summary>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        /// <param name="numPoints">Po�et bod� �ezu</param>
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

        private const string errorMessageNoCross = "Trajektorie s dan�mi po��te�n�mi podm�nkami neprot�n� rovinu �ezu!";
    }

    /// <summary>
    /// V�jimka ve t��d� Poincar�
    /// </summary>
    public class PoincareSectionException : ApplicationException {
        /// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
		public PoincareSectionException(string message) : base(message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybov�ho hl�en�</param>
        public PoincareSectionException(string message, Exception innerException) : base(message, innerException) { }
    }
}

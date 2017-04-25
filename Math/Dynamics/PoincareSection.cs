using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Po��t� Poincar�ho �ez danou rovinou (rovina d�na vektorem (a, b, c, ...) = ax + by + ... == 0)
    /// a zaznamen�v� sou�adnice o indexech i1, i2
    /// </summary>
    public class PoincareSection {
        // Syst�m
        private IDynamicalSystem dynamicalSystem;

        // RungeKutta
        private RungeKutta rungeKutta;

        // P�esnost v�po�tu
        private double precision;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="rkMethod">Metoda k v�po�tu RK</param>
        public PoincareSection(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod) {
            this.dynamicalSystem = dynamicalSystem;
            this.rungeKutta = RungeKutta.CreateRungeKutta(dynamicalSystem, precision, rkMethod);
            this.precision = precision > 0.0 ? precision : this.rungeKutta.Precision;
        }

        /// <summary>
        /// Vypo��t� Poincar�ho �ez v defaultn� rovin� (0; 1; 0; 0) pro sou�adnice 0; 2
        /// </summary>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        /// <param name="numPoints">Po�et bod� �ezu</param>
        public PointVector Compute(Vector initialX, int numPoints) {
            Vector plane = new Vector(4);
            plane[1] = 1.0;

            return this.Compute(plane, 0, 2, initialX, numPoints, false, null);
        }

        /// <summary>
        /// Vypo��t� Poincar�ho �ez
        /// </summary>
        /// <param name="plane">Rovina �ezu</param>
        /// <param name="i1">Zaznamen�van� index x</param>
        /// <param name="i2">Zaznamen�van� index y</param>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        /// <param name="numPoints">Po�et bod� �ezu</param>
        /// <param name="oneOrientation">True, pokud chceme zobrazovat jen jednu orientaci pr�chodu rovinou</param>
        public PointVector Compute(Vector plane, int i1, int i2, Vector initialX, int numPoints, bool oneOrientation, IOutputWriter writer) {
            return this.Section(plane, i1, i2, initialX, numPoints, oneOrientation, writer)[0] as PointVector;
        }

        /// <summary>
        /// Vr�t� doby pr�chod� rovinou �ezu
        /// </summary>
        /// <param name="plane">Rovina �ezu</param>
        /// <param name="i1">Zaznamen�van� index x</param>
        /// <param name="i2">Zaznamen�van� index y</param>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        /// <param name="numPoints">Po�et bod� �ezu</param>
        /// <param name="oneOrientation">True, pokud chceme zobrazovat jen jednu orientaci pr�chodu rovinou</param>
        public Vector SectionTimes(Vector plane, int i1, int i2, Vector initialX, int numPoints, bool oneOrientation, IOutputWriter writer) {
            return this.Section(plane, i1, i2, initialX, numPoints, oneOrientation, writer)[1] as Vector;
        }
        
        /// <summary>
        /// Vypo��t� Poincar�ho �ez a �asy pr�chod�
        /// </summary>
        /// <param name="plane">Rovina �ezu</param>
        /// <param name="i1">Zaznamen�van� index x</param>
        /// <param name="i2">Zaznamen�van� index y</param>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        /// <param name="numPoints">Po�et bod� �ezu</param>
        /// <param name="oneOrientation">True, pokud chceme zobrazovat jen jednu orientaci pr�chodu rovinou</param>
        /// <returns>Dva objekty - prvn� s �ezem (PointVector), druh� s �asy pr�chodu rovinou (Vector)</returns>
        private object [] Section(Vector plane, int i1, int i2, Vector initialX, int numPoints, bool oneOrientation, IOutputWriter writer) {
            // ��slo, kter� dour�uje rovinu (plane * x == crossPoint)
            double crossPoint;

            if(plane.Length % 2 == 1) {
                crossPoint = plane.LastItem;
                plane.Length = plane.Length - 1;
            }
            else {
                crossPoint = 0;
            }

            PointVector section = new PointVector(numPoints);
            Vector times = new Vector(numPoints);

            int finished = 0;
            Vector x = initialX;
            double sp = x * plane;

            double step = this.precision;
            double time = 0;
            bool postProcessed = false;

            int write = numPoints / 10;

            this.rungeKutta.Init(initialX);

            do {
                double newStep;
                Vector newx = x + this.rungeKutta.Step(x, ref step, out newStep);
                time += step;
                step = newStep;

                double newsp = newx * plane;

                // Nebyl postprocessing
                if(!postProcessed)
                    // jedna �i ob� orientace pr�chodu
                    if((newsp <= crossPoint && sp > crossPoint) ||
                        (!oneOrientation && sp <= crossPoint && newsp > crossPoint)) {
                        double ratio = (sp - crossPoint) / (newsp - sp);
                        Vector v = (x - newx) * ratio + x;
                        section[finished].X = v[i1];
                        section[finished].Y = v[i2];
                        times[finished] = -step * ratio + time;
                        finished++;

                        if(finished % write == 0 && writer != null)
                            writer.Write(".");
                    }

                sp = newsp;
                postProcessed = this.dynamicalSystem.PostProcess(newx);
                x = newx;

                if(finished == 0 && time > maxTimeWithoutCross)
                    throw new PoincareSectionException(Messages.EMNoCross);

            } while(finished < numPoints);

            object[] result = new object[2];
            result[0] = section;
            result[1] = times;

            return result;
        }

        /// <summary>
        /// True, pokud trajektorie prot�n� rovinu
        /// </summary>
        /// <param name="plane">Rovina �ezu</param>
        /// <param name="i1">Zaznamen�van� index x</param>
        /// <param name="i2">Zaznamen�van� index y</param>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        /// <param name="oneOrientation">True, pokud po��t�me jen jednu orientaci pr�chodu rovinou</param>
        public bool CrossPlane(Vector plane, int i1, int i2, Vector initialX, bool oneOrientation) {
            // ��slo, kter� dour�uje rovinu (plane * x == crossPoint)
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
            bool postProcessed = false;

            this.rungeKutta.Init(initialX);

            do {
                double newStep;
                Vector newx = x + this.rungeKutta.Step(x, ref step, out newStep);
                time += step;
                step = newStep;

                double newsp = newx * plane;

                // jedna �i ob� orientace pr�chodu
                // Nebyl postprocessing
                if(!postProcessed)
                    // jedna �i ob� orientace pr�chodu
                    if((newsp <= crossPoint && sp > crossPoint) ||
                        (!oneOrientation && sp <= crossPoint && newsp > crossPoint))
                        return true;

                sp = newsp;
                postProcessed = this.dynamicalSystem.PostProcess(newx);
                x = newx;

            } while(time < maxTimeWithoutCross);

            return false;
        }

        private const double maxTimeWithoutCross = 1000.0;
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

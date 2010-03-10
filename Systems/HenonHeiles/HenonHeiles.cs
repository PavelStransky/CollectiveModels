using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class HenonHeiles: IDynamicalSystem {
        /// <summary>
        /// Pravá strana pohybové rovnice (rovnice 2. øádu)
        /// </summary>
        /// <param name="x">Souøadnice a rychlosti</param>
        public Vector Equation(Vector x) {
            Vector result = new Vector(4);

            result[0] = x[2];
            result[1] = x[3];

            result[2] = -x[0] - 2.0 * x[0] * x[1];
            result[3] = -x[1] - x[0] * x[0] + x[1] * x[1];

            return result;
        }

        /// <summary>
        /// Matice pro výpoèet SALI (Jakobián)
        /// </summary>
        /// <param name="x">Vektor x v èase t</param>
        public Matrix Jacobian(Vector x) {
            Matrix result = new Matrix(4, 4);

            result[0, 2] = -1 - 2 * x[1];
            result[0, 3] = 2 * x[0];
            result[1, 2] = -2 * x[0];
            result[1, 3] = -1 + 2 * x[1];
            result[2, 0] = 1;
            result[3, 1] = 1;

            return result;
        }

        /// <summary>
        /// Energie systému
        /// </summary>
        /// <param name="x">Souøadnice a rychlosti</param>
        /// <returns></returns>
        public double E(Vector x) {
            return 0.5 * (x[2] * x[2] + x[3] * x[3]) + 0.5 * (x[0] * x[0] + x[1] * x[1]) + x[0] * x[0] * x[1] - x[1] * x[1] * x[1] / 3.0;
        }

        /// <summary>
        /// Poèet stupòù volnosti
        /// </summary>
        public int DegreesOfFreedom { get { return degreesOfFreedom; } }

        /// <summary>
        /// NEFUNGUJE !!!
        /// </summary>
        public Vector IC(double e) {
            Vector result = new Vector(4);
            result[0] = 0.0;
            result[1] = 0.1;
            result[2] = 0.49058;
            result[3] = 0.0;

            return result;
        }

        /// <summary>
        /// NEFUNGUJE !!!
        /// </summary>
        public Vector IC(double e, double j) {
            return this.IC(e);
        }

        /// <summary>
        /// NEFUNGUJE !!!
        /// </summary>
        public bool IC(Vector ic, double e) {
            return false;
        }

        /// <summary>
        /// NEFUNGUJE !!!
        /// </summary>
        public Vector Bounds(double e) {
            return new Vector(0);
        }

        /// <summary>
        /// NEFUNGUJE !!!
        /// </summary>
        public double PeresInvariant(Vector x) {
            return 0;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public HenonHeiles() {
        }

        /// <summary>
        /// Postprocessing
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public bool PostProcess(Vector x) {
            return false;
        }

        private const double poincareTime = 100;
        private const int degreesOfFreedom = 2;
   }
}
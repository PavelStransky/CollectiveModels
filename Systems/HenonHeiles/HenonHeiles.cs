using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class HenonHeiles: IDynamicalSystem {
        /// <summary>
        /// Prav� strana pohybov� rovnice (rovnice 2. ��du)
        /// </summary>
        /// <param name="x">Sou�adnice a rychlosti</param>
        public Vector Equation(Vector x) {
            Vector result = new Vector(4);

            result[0] = x[2];
            result[1] = x[3];

            result[2] = -x[0] - 2.0 * x[0] * x[1];
            result[3] = -x[1] - x[0] * x[0] + x[1] * x[1];

            return result;
        }

        /// <summary>
        /// Matice pro v�po�et SALI (Jakobi�n)
        /// </summary>
        /// <param name="x">Vektor x v �ase t</param>
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
        /// Energie syst�mu
        /// </summary>
        /// <param name="x">Sou�adnice a rychlosti</param>
        /// <returns></returns>
        public double E(Vector x) {
            return 0.5 * (x[2] * x[2] + x[3] * x[3]) + 0.5 * (x[0] * x[0] + x[1] * x[1]) + x[0] * x[0] * x[1] - x[1] * x[1] * x[1] / 3.0;
        }

        /// <summary>
        /// Po�et stup�� volnosti
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
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// NEFUNGUJE !!!
        /// </summary>
        public bool IC(Vector ic, double e) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// NEFUNGUJE !!!
        /// </summary>
        public Vector Bounds(double e) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Kontrola po��te�n�ch podm�nek
        /// </summary>
        /// <param name="bounds">Po��te�n� podm�nky</param>
        public Vector CheckBounds(Vector bounds) {
            return bounds;
        }

        /// <summary>
        /// NEFUNGUJE !!!
        /// </summary>
        public double PeresInvariant(Vector x) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public HenonHeiles() {
        }

        /// <summary>
        /// Postprocessing
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        public bool PostProcess(Vector x) {
            return false;
        }

        /// <summary>
        /// Body pro rozhodnut�, zda je podle SALI dan� trajektorie regul�rn� nebo chaotick�
        /// </summary>
        /// <returns>[time chaotick�, SALI chaotick�, time regul�rn�, SALI regul�rn�, time koncov� bod, SALI koncov� bod]</returns>
        public double[] SALIDecisionPoints() {
            throw new Exception("The method or operation is not implemented.");
        }

        private const double poincareTime = 100;
        private const int degreesOfFreedom = 2;
   }
}
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class PCR3BP : IExportable, IDynamicalSystem {
        private double mu;

        private Random random = new Random();

        /// <summary>
        /// Kinetická energie
        /// </summary>
        /// <param name="x">Souřadnice a hybnosti</param>
        public double T(Vector x) {
            return 0.5 * (x[2] * x[2] + x[3] * x[3]);
        }

        /// <summary>
        /// Potenciál
        /// </summary>
        /// <param name="r">Souřadnice a hybnosti</param>
        public double V(Vector r) {
            double x = r[0], y = r[1];
            double cent = 0.5 * (x * x + y * y);
            double v1 = (1.0 - this.mu) / System.Math.Sqrt((x + this.mu) * (x + this.mu) + y * y);
            double v2 = this.mu / System.Math.Sqrt((x + this.mu - 1.0) * (x + this.mu - 1.0) + y * y);
            return -cent - v1 - v2;
        }

        /// <summary>
        /// Celková energie
        /// </summary>
        /// <param name="x">Souřadnice a hybnosti</param>
        public double E(Vector x) {
            return this.T(x) + this.V(x);
        }

        /// <summary>
        /// Pravá strana Hamiltonových pohybových rovnic
        /// </summary>
        /// <param name="x">Souřadnice a hybnosti</param>
        public Vector Equation(Vector r) {
            Vector result = new Vector(4);

            double x = r[0];
            double y = r[1];

            double z = x + this.mu - 1.0;

            double j1 = System.Math.Pow((x + this.mu) * (x + this.mu) + y * y, 1.5);
            double j2 = System.Math.Pow(z * z + y * y, 1.5);

            double vx = -x + (this.mu + x) * (1.0 - this.mu) / j1 + this.mu * z / j2;
            double vy = y * (-1.0 + (1.0 - this.mu) / j1 + this.mu / j2);

            result[0] = r[2];
            result[1] = r[3];

            // Včetně Coriolisovy síly
            result[2] = 2.0 * r[3] - vx;
            result[3] = -2.0 * r[2] - vy;

            // Bez Coriolisovy síly
//            result[2] = -vx;
//            result[3] = -vy;

            return result;
        }

        /// <summary>
        /// Počet stupňů volnosti
        /// </summary>
        public int DegreesOfFreedom { get { return degreesOfFreedom; } }

        /// <summary>
        /// Konstruktor standardního Lagrangiánu
        /// </summary>
        /// <param name="m0">Hmotnost 0. částice</param>
        /// <param name="m1">Hmotnost 1. částice</param>
        /// <param name="m2">Hmotnost 2. částice</param>
        public PCR3BP(double mu) {
            this.mu = mu;
        }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected PCR3BP() { }

        /// <summary>
        /// Vypíše parametry modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("mu = {0,10:#####0.000}", this.mu));
            s.Append("\n");

            return s.ToString();
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží třídu do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.mu, "mu");
            param.Export(export);
        }

        /// <summary>
        /// Načte třídu ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public PCR3BP(Core.Import import) {
            IEParam param = new IEParam(import);
            this.mu = (double)param.Get(1.0);
        }
        #endregion

        private const int degreesOfFreedom = 2;

        #region IDynamicalSystem Members
        public Matrix Jacobian(Vector x) {
            throw new Exception("The method or operation is not implemented.");
        }

        public Vector IC(double e) {
            // Generování řezu pro y = 0 a x v mezích -1...1
            Vector result = new Vector(4);

            do {
                result[0] = 2.0 * this.random.NextDouble() - 1.0;
//                result[0] = 0.08 * r.NextDouble() - 0.18;
            } while(this.V(result) > e);

            // Koeficient před kinetickým členem
            double tbracket = 2.0 * (e - this.V(result));

            result[2] = this.random.NextDouble() * System.Math.Sqrt(tbracket);
            if(this.random.Next(2) == 0)
               result[2] = -result[2];            

            tbracket -= result[2] * result[2];

            result[3] = System.Math.Sqrt(tbracket) * (this.random.Next(2) == 0 ? -1.0 : 1.0);

            return result;
        }

        public Vector IC(double e, double l) {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool IC(Vector ic, double e) {
            throw new Exception("The method or operation is not implemented.");
        }

        public Vector Bounds(double e) {
            Vector result = new Vector(degreesOfFreedom * 4);
            for(int i = 0; i < degreesOfFreedom * 2; i++) {
                result[2 * i] = -100;
                result[2 * i + 1] = 100;
            }
            return result;
        }

        /// <summary>
        /// Kontrola počátečních podmínek
        /// </summary>
        /// <param name="bounds">Počáteční podmínky</param>
        public Vector CheckBounds(Vector bounds) {
            return bounds;
        }

        /// <summary>
        /// Postprocessing
        /// </summary>
        /// <param name="x">Souřadnice a hybnosti</param>
        public bool PostProcess(Vector x) {
            return false;
        }

        /// <summary>
        /// Body pro rozhodnutí, zda je podle SALI daná trajektorie regulární nebo chaotická
        /// </summary>
        /// <returns>[time chaotická, SALI chaotická, time regulární, SALI regulární, time koncový bod, SALI koncový bod]</returns>
        public double[] SALIDecisionPoints() {
            throw new Exception("The method or operation is not implemented.");
        }

        public double PeresInvariant(Vector x) {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}
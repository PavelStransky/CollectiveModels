using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class ThreeBody: IExportable, IDynamicalSystem {
        private Vector m = new Vector(3);

        /// <summary>
        /// Kinetická energie
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public double T(Vector x) {
            return this.T(x, 0) + this.T(x, 1) + this.T(x, 2);
        }

        /// <summary>
        /// Kinetická energie
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        /// <param name="i">Index èástice</param>
        public double T(Vector x, int i) {
            double p1 = x[2 * i + 6];
            double p2 = x[2 * i + 7];
            return (p1 * p1 + p2 * p2) / (2.0 * this.m[i]);
        }

        /// <summary>
        /// Souøadnice vzdáleností
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        /// <param name="i">Index první èástice</param>
        /// <param name="j">Index druhé èástice</param>
        public double R(Vector x, int i, int j) {
            double dx = x[2 * i] - x[2 * j];
            double dy = x[2 * i + 1] - x[2 * j + 1];
            return System.Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Potenciál
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        /// <param name="i">Index první èástice</param>
        /// <param name="j">Index druhé èástice</param>
        public double V(Vector x, int i, int j) {
            return -this.m[i] * this.m[j] / this.R(x, i, j);
        }

        /// <summary>
        /// Potenciál
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public double V(Vector x) {
            return this.V(x, 0, 1) + this.V(x, 1, 2) + this.V(x, 0, 2);
        }

        /// <summary>
        /// Celková energie
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public double E(Vector x) {
            return this.T(x) + this.V(x);
        }

        /// <summary>
        /// Pravá strana Hamiltonových pohybových rovnic
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        public virtual Vector Equation(Vector x) {
            Vector result = new Vector(12);

            double m0m1 = this.m[0] * this.m[1] / System.Math.Pow(this.R(x, 0, 1), 3);
            double m1m2 = this.m[1] * this.m[2] / System.Math.Pow(this.R(x, 1, 2), 3);
            double m0m2 = this.m[0] * this.m[2] / System.Math.Pow(this.R(x, 0, 2), 3);

            result[6] = -(m0m1 * (x[0] - x[2]) + m0m2 * (x[0] - x[4]));
            result[7] = -(m0m1 * (x[1] - x[3]) + m0m2 * (x[1] - x[5]));
            result[8] = -(m0m1 * (x[2] - x[0]) + m1m2 * (x[2] - x[4]));
            result[9] = -(m0m1 * (x[3] - x[1]) + m1m2 * (x[3] - x[5]));
            result[10] = -(m0m2 * (x[4] - x[0]) + m1m2 * (x[4] - x[2]));
            result[11] = -(m0m2 * (x[5] - x[1]) + m1m2 * (x[5] - x[3]));

            result[0] = x[6] / this.m[0];
            result[1] = x[7] / this.m[0];
            result[2] = x[8] / this.m[1];
            result[3] = x[9] / this.m[1];
            result[4] = x[10] / this.m[2];
            result[5] = x[11] / this.m[2];

            return result;
        }

        /// <summary>
        /// Poèet stupòù volnosti
        /// </summary>
        public int DegreesOfFreedom { get { return degreesOfFreedom; } }

        /// <summary>
        /// Konstruktor standardního Lagrangiánu
        /// </summary>
        /// <param name="m0">Hmotnost 0. èástice</param>
        /// <param name="m1">Hmotnost 1. èástice</param>
        /// <param name="m2">Hmotnost 2. èástice</param>
        public ThreeBody(double m0, double m1, double m2) {
            this.m[0] = m0;
            this.m[1] = m1;
            this.m[2] = m2;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="m">Hmotnosti</param>
        public ThreeBody(Vector m) {
            this.m = m;
        }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected ThreeBody() { }

        /// <summary>
        /// Vypíše parametry modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("m0 = {0,10:#####0.000}\nm1 = {1,10:#####0.000}\nm2 = {2,10:#####0.000}", this.m[0], this.m[1], this.m[2]));
            s.Append("\n");

            return s.ToString();
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží tøídu do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.m, "m");
            param.Export(export);
        }

        /// <summary>
        /// Naète tøídu ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public ThreeBody(Core.Import import) {
            IEParam param = new IEParam(import);
            this.m = (Vector)param.Get();
        }
        #endregion

        private const int degreesOfFreedom = 6;

        #region IDynamicalSystem Members
        public Matrix Jacobian(Vector x) {
            throw new Exception("The method or operation is not implemented.");
        }

        public Vector IC(double e) {
            throw new Exception("The method or operation is not implemented.");
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

        public double PeresInvariant(Vector x) {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}
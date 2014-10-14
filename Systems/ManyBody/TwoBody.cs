using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class TwoBody: IExportable, IDynamicalSystem {
        private Vector m = new Vector(2);

        /// <summary>
        /// Kinetick� energie
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        public double T(Vector x) {
            return this.T(x, 0) + this.T(x, 1);
        }

        /// <summary>
        /// Kinetick� energie
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        /// <param name="i">Index ��stice</param>
        public double T(Vector x, int i) {
            double p1 = x[2 * i + 4];
            double p2 = x[2 * i + 5];
            return (p1 * p1 + p2 * p2) / (2.0 * this.m[i]);
        }

        /// <summary>
        /// Sou�adnice vzd�lenost�
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        public double R(Vector x) {
            double dx = x[2] - x[0];
            double dy = x[3] - x[1];
            return System.Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Potenci�l
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        /// <param name="i">Index prvn� ��stice</param>
        /// <param name="j">Index druh� ��stice</param>
        public double V(Vector x) {
            return -this.m[0] * this.m[1] / this.R(x);
        }

        /// <summary>
        /// Celkov� energie
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        public double E(Vector x) {
            return this.T(x) + this.V(x);
        }

        /// <summary>
        /// Prav� strana Hamiltonov�ch pohybov�ch rovnic
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        public virtual Vector Equation(Vector x) {
            Vector result = new Vector(8);

            double m0m1 = this.m[0] * this.m[1] / System.Math.Pow(this.R(x), 3);

            result[4] = -(m0m1 * (x[0] - x[2]));
            result[5] = -(m0m1 * (x[1] - x[3]));
            result[6] = -result[4];
            result[7] = -result[5];

            result[0] = x[4] / this.m[0];
            result[1] = x[5] / this.m[0];
            result[2] = x[6] / this.m[1];
            result[3] = x[7] / this.m[1];

            return result;
        }

        /// <summary>
        /// Po�et stup�� volnosti
        /// </summary>
        public int DegreesOfFreedom { get { return degreesOfFreedom; } }

        /// <summary>
        /// Konstruktor standardn�ho Lagrangi�nu
        /// </summary>
        /// <param name="m0">Hmotnost 0. ��stice</param>
        /// <param name="m1">Hmotnost 1. ��stice</param>
        public TwoBody(double m0, double m1) {
            this.m[0] = m0;
            this.m[1] = m1;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="m">Hmotnosti</param>
        public TwoBody(Vector m) {
            this.m = m;
        }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected TwoBody() { }

        /// <summary>
        /// Vyp�e parametry modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("m0 = {0,10:#####0.000}\nm1 = {1,10:#####0.000}", this.m[0], this.m[1]));
            s.Append("\n");

            return s.ToString();
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� t��du do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public virtual void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.m, "m");
            param.Export(export);
        }

        /// <summary>
        /// Na�te t��du ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public TwoBody(Core.Import import) {
            IEParam param = new IEParam(import);
            this.m = (Vector)param.Get();
        }
        #endregion

        private const int degreesOfFreedom = 4;

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

        /// <summary>
        /// Kontrola po��te�n�ch podm�nek
        /// </summary>
        /// <param name="bounds">Po��te�n� podm�nky</param>
        public Vector CheckBounds(Vector bounds) {
            return bounds;
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

        public double PeresInvariant(Vector x) {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
    }
}
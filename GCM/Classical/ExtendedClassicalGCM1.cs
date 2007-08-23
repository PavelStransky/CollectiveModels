using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.GCM {
    /// <summary>
    /// Klasick� GCM s hmotou �m�rnou beta^2
    /// </summary>
    public class ExtendedClassicalGCM1 : ClassicalGCM, IDynamicalSystem, IExportable {
        // Gener�tor n�hodn�ch ��sel
        private Random random = new Random();

        // Roz���en� parametr
        private double kappa;
        public double Kappa { get { return this.kappa; } set { this.kappa = value; } }

        /// <summary>
        /// Kinetick� energie
        /// </summary>
        /// <param name="x">Sou�adnice x</param>
        /// <param name="y">Sou�adnice y</param>
        /// <param name="px">Hybnost x</param>
        /// <param name="py">Hybnost y</param>
        public override double T(double x, double y, double px, double py) {
            double b2 = x * x + y * y;
            return 1.0 / (2.0 * this.K * (1 + this.Kappa * b2)) * (px * px + py * py);
        }

        /// <summary>
        /// Prav� strana Hamiltonov�ch pohybov�ch rovnic
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        public override Vector Equation(Vector x) {
            Vector result = new Vector(4);

            double b2 = x[0] * x[0] + x[1] * x[1];
            double dVdx = 2.0 * this.A * x[0] + 3.0 * this.B * (x[0] * x[0] - x[1] * x[1]) + 4.0 * this.C * x[0] * b2;
            double dVdy = 2.0 * x[1] * (this.A - 3.0 * this.B * x[0] + 2.0 * this.C * b2);

            double e = 1.0 / (this.K * (1 + this.Kappa * b2));
            double f = this.K * this.Kappa * e * e * (x[2] * x[2] + x[3] * x[3]);

            result[0] = e * x[2];
            result[1] = e * x[3];

            result[2] = f * x[0] - dVdx;
            result[3] = f * x[1] - dVdy;

            return result;
        }

        /// <summary>
        /// Matice pro v�po�et SALI (Jakobi�n)
        /// </summary>
        /// <param name="x">Vektor x v �ase t</param>
        public override Matrix Jacobian(Vector x) {
            Matrix result = new Matrix(4, 4);

            double b2 = x[0] * x[0] + x[1] * x[1];
            double dV2dxdx = 2.0 * this.A + 6.0 * this.B * x[0] + 4.0 * this.C * (3.0 * x[0] * x[0] + x[1] * x[1]);
            double dV2dxdy = (-6.0 * this.B + 8.0 * this.C * x[0]) * x[1];
            double dV2dydy = 2.0 * (this.A - 3.0 * this.B * x[0] + 2.0 * this.C * (x[0] * x[0] + 3.0 * x[1] * x[1]));

            double e = 1.0 / (this.K * (1 + this.Kappa * b2));
            double f = this.K * this.Kappa * e * e * (x[2] * x[2] + x[3] * x[3]);
            double g = 4.0 * this.K * this.Kappa * e;
            double h = -2.0 * this.Kappa * e;

            result[0, 0] = h * x[0] * x[2];
            result[0, 1] = h * x[1] * x[2];
            result[0, 2] = e;

            result[1, 0] = h * x[0] * x[3];
            result[1, 1] = h * x[1] * x[3];
            result[1, 3] = e;

            result[2, 0] = f * (1 - g * x[0] * x[0]) - dV2dxdx;
            result[2, 1] = -g * x[0] * x[1] - dV2dxdy;
            result[2, 2] = -result[0, 0];
            result[2, 3] = -result[1, 0];

            result[3, 0] = result[2, 1];
            result[3, 1] = f * (1 - g * x[1] * x[1]) - dV2dydy;
            result[3, 2] = -result[0, 1];
            result[3, 3] = -result[1, 1];

            return result;
        }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        public ExtendedClassicalGCM1() { }

        /// <summary>
        /// Konstruktor roz���en�ho Lagrangi�nu
        /// </summary>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr K</param>
        /// <param name="kappa">Parametr Kappa</param>
        public ExtendedClassicalGCM1(double a, double b, double c, double k, double kappa)
            : base(a, b, c, k) {
            this.kappa = kappa;
        }

        /// <summary>
        /// Vyp�e parametry GCM modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("A = {0,10:#####0.000}\nB = {1,10:#####0.000}\nC = {2,10:#####0.000}\nK = {3,10:#####0.000}\nkappa = {4,10:#####0.000}", this.A, this.B, this.C, this.K, this.Kappa));
            s.Append(string.Format("I = {0,10:#####0.000}", this.Invariant));
            s.Append("\n\n");

            Vector beta = this.ExtremalBeta(0.0);
            s.Append(string.Format("Extr�my:"));

            for(int i = 0; i < beta.Length; i++)
                s.Append(string.Format("\nV({0,1:0.000}) = {1,1:0.000}", beta[i], this.VBG(beta[i], 0.0)));

            return s.ToString();
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� GCM t��du do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public override void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.A, "A");
            param.Add(this.B, "B");
            param.Add(this.C, "C");
            param.Add(this.K, "K");
            param.Add(this.Kappa, "Kappa");

            param.Export(export);
        }

        /// <summary>
        /// Na�te GCM t��du ze souboru textov�
        /// </summary>
        /// <param name="import">Import</param>
        public override void Import(Core.Import import) {
            IEParam param = new IEParam(import);

            this.A = (double)param.Get(-1.0);
            this.B = (double)param.Get(1.0);
            this.C = (double)param.Get(1.0);
            this.K = (double)param.Get(1.0);
            this.Kappa = (double)param.Get(1.0);
        }
        #endregion

        private const double poincareTime = 100;
        private const int degreesOfFreedom = 2;

        private const string errorMessageInitialCondition = "Pro zadanou energii {0} nelze nagenerovat po��te�n� podm�nky.";
        private const string errorMessageNonzeroJ = "T��da {0} um� po��tat pouze s nulov�m �hlov�m momentem. Pro nenulov� �hlov� moment pou�ij {1}.";
    }
}
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class CoupledHarmonicOscillator : IDynamicalSystem, IExportable {
        // Gener�tor n�hodn�ch ��sel
        private Random random = new Random();

        // Vazbov� konstanta
        private double lambda;

        // Hmotnost
        private double m;

        // Tuhost
        private double k;

        public double Lambda { get { return this.lambda; } set { this.lambda = value; } }
        public double K { get { return this.k; } set { this.k = value; } }
        public double M { get { return this.m; } set { this.m = value; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="k">Tuhost oscil�toru</param>
        /// <param name="m">Hmotnost</param>
        /// <param name="lambda">Vazbov� konstanta</param>
        public CoupledHarmonicOscillator(double m, double k, double lambda) {
            this.m = m;
            this.k = k;
            this.lambda = lambda;
        }
        
        /// <summary>
        /// Kinetick� energie
        /// </summary>
        /// <param name="px">Hybnost x</param>
        /// <param name="py">Hybnost y</param>
        public double T(double px, double py) {
            return (px * px + py * py) / (2.0 * this.m);
        }

        public double V(double x, double y) {
            return (x * x + y * y) * this.k / 2.0 + this.lambda * x * x * y * y;
        }

        /// <summary>
        /// Energie
        /// </summary>
        /// <param name="v">Sou�adnice a hybnosti</param>
        public double E(Vector v) {
            return this.T(v[2], v[3]) + this.V(v[0], v[1]);
        }

        /// <summary>
        /// Hamiltonovy pohybov� rovnice
        /// </summary>
        /// <param name="v">Sou�adnice a hybnosti</param>
        public Vector Equation(Vector v) {
            double x = v[0], y = v[1];

            double dVdx = x * (this.k + 2.0 * this.lambda * y * y);
            double dVdy = y * (this.k + 2.0 * this.lambda * x * x);

            Vector result = new Vector(4);

            result[0] = v[2] / this.m;
            result[1] = v[3] / this.m;

            result[2] = -dVdx;
            result[3] = -dVdy;

            return result;
        }

        /// <summary>
        /// Jakobi�n v�zan�ch oscil�tor�
        /// </summary>
        /// <param name="v">Sou�adnice a hybnosti</param>
        public Matrix Jacobian(Vector v) {
            Matrix result = new Matrix(4);

            double x = v[0], y = v[1];

            double dV2dxdx = this.k + 2.0 * this.lambda * y * y;
            double dV2dxdy = 4.0 * this.lambda * x * y;
            double dV2dydy = this.k + 2.0 * this.lambda * x * x;

            result[0, 2] = 1 / this.m;
            result[1, 3] = result[0, 2];

            result[2, 0] = -dV2dxdx;
            result[2, 1] = -dV2dxdy;

            result[3, 0] = result[2, 1];
            result[3, 1] = -dV2dydy;

            return result;
        }

        /// <summary>
        /// Generov�n� po��te�n� podm�nky
        /// </summary>
        /// <param name="e">Energie</param>
        public Vector IC(double e) {
            Vector result = new Vector(4);

            // Po�et hled�n�
            int iteration = 0;

            do {
                // Podm�nka plynouc� z odmocniny v Hamiltoni�nu zn�:
                // x^2 + y^2 + px^2 + py^2 <= 2
                // Pomoc� n� nagenerujeme x, y, px
                double max2 = 2.0;
                result[0] = (2.0 * this.random.NextDouble() - 1) * System.Math.Sqrt(max2);
                max2 -= result[0] * result[0];
                result[1] = (2.0 * this.random.NextDouble() - 1) * System.Math.Sqrt(max2);

                result[2] = 0.0;
                result[3] = 0.0;

                if(this.E(result) < e) {
                    result[2] = double.NaN;
                    result[3] = double.NaN;
                    if(this.IC(result, e))
                        break;
                }

                iteration++;

                if(iteration > maxIteration)
                    throw new Exception("Chyba, p�ekro�en po�et iterac�");

            } while(true);

            return result;
        }

        /// <summary>
        /// Generov�n� hybnost� v po��te�n� podm�nce
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="ic">Polohy po�. podm�nky</param>
        /// <returns>True, pokud se generov�n� poda�ilo</returns>        
        public bool IC(Vector ic, double e) {
            // Koeficient p�ed kinetick�m �lenem
            double koef = 1.0 / (2 * this.m);
            double tbracket = (e - this.V(ic[0], ic[1])) / koef;

            if(double.IsNaN(ic[2]) && double.IsNaN(ic[3])) {
                ic[2] = this.random.NextDouble() * System.Math.Sqrt(tbracket);

                if(this.random.Next(2) == 0)
                    ic[2] = -ic[2];
            }

            if(double.IsNaN(ic[2]))
                tbracket -= ic[3] * ic[3];
            else
                tbracket -= ic[2] * ic[2];

            if(tbracket < 0.0)
                return false;

            if(double.IsNaN(ic[2]))
                ic[2] = System.Math.Sqrt(tbracket) * (this.random.Next(2) == 0 ? -1.0 : 1.0);
            else
                ic[3] = System.Math.Sqrt(tbracket) * (this.random.Next(2) == 0 ? -1.0 : 1.0);

            return true;
        }

        public Vector IC(double e, double l) {
            if(l == 0.0)
                return this.IC(e);
            else
                throw new Exception(string.Format(errorMessageNonzeroJ));
        }

        /// <summary>
        /// Meze v sou�adnic�ch x, y, px, py se�azen� do vektoru o 8 slo�k�ch (xmin, xmax, ...)
        /// </summary>
        /// <param name="e">Energie</param>
        public Vector Bounds(double e) {
            Vector result = new Vector(8);
            result[0] = -System.Math.Sqrt(2.0 * e / this.k);
            result[1] = -result[0];

            result[2] = result[0];
            result[3] = result[1];

            result[4] = -System.Math.Sqrt(2.0 * this.m * e);
            result[5] = -result[4];

            result[6] = result[4];
            result[7] = result[5];

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
        /// Po�et stup�� volnosti
        /// </summary>
        public int DegreesOfFreedom {
            get { return degreesOfFreedom; }
        }

        public double PeresInvariant(Vector x) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Postprocessing
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        public bool PostProcess(Vector x) {
            return false;
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� t��du do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.m, "Mass");
            param.Add(this.k, "Rigidity");
            param.Add(this.lambda, "Coupling");

            param.Export(export);
        }

        /// <summary>
        /// Na�te t��du ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public CoupledHarmonicOscillator(Core.Import import) {
            IEParam param = new IEParam(import);

            this.m = (double)param.Get();
            this.k = (double)param.Get();
            this.lambda = (double)param.Get();
        }
        #endregion

        private const int degreesOfFreedom = 2;
        private const double maxIteration = 10000;
        private const string errorMessageNonzeroJ = "T��da {0} um� po��tat pouze s nulov�m �hlov�m momentem.";
    }
}

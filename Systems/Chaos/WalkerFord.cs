using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class WalkerFord : IExportable, IDynamicalSystem {
        private double a, b;
        private double maxJ; // Nejvyšší hodnota J pro počáteční podmínky

        private Random random = new Random();

        /// <summary>
        /// Kinetická energie
        /// </summary>
        /// <param name="x">Souřadnice a hybnosti</param>
        public double H0(Vector x) {
            return x[2] + x[3] - x[2] * x[2] - 3.0 * x[2] * x[3] + x[3] * x[3];
        }

        /// <summary>
        /// Potenciál
        /// </summary>
        /// <param name="x">Souřadnice a hybnosti</param>
        public double H1(Vector x) {
            return this.a * x[2] * x[3] * System.Math.Cos(2.0 * (x[0] - x[1]));
        }

        public double H2(Vector x) {
            return this.b * x[2] * System.Math.Pow(x[3], 1.5) * System.Math.Cos(2.0 * x[0] - 3.0 * x[1]);
        }

        /// <summary>
        /// Celková energie
        /// </summary>
        /// <param name="x">Souřadnice a hybnosti</param>
        public double E(Vector x) {
            return this.H0(x) + this.H1(x) + this.H2(x);
        }

        /// <summary>
        /// Pravá strana Hamiltonových pohybových rovnic
        /// </summary>
        /// <param name="x">Souřadnice a hybnosti</param>
        public Vector Equation(Vector x) {
            Vector result = new Vector(4);

            double x22 = 2.0 * (x[0] - x[1]);
            double c22 = System.Math.Cos(x22);
            double s22 = System.Math.Sin(x22);

            double x23 = 2.0 * x[0] - 3.0 * x[1];
            double c23 = System.Math.Cos(x23);
            double s23 = System.Math.Sin(x23);

            result[0] = 1.0 - 2.0 * x[2] - 3.0 * x[3] + this.a * x[3] * c22 + this.b * System.Math.Pow(x[3], 1.5) * c23;
            result[1] = 1.0 - 3.0 * x[2] + 2.0 * x[3] + this.a * x[2] * c22 + this.b * x[2] * System.Math.Sqrt(x[3]) * c23;

            result[2] = 2.0 * x[2] * x[3] * (this.a * s22 + this.b * System.Math.Sqrt(x[3]) * s23);
            result[3] = -x[2] * x[3] * (2.0 * this.a * s22 + 3.0 * this.b * System.Math.Sqrt(x[3]) * s23);
            
            return result;
        }

        /// <summary>
        /// Počet stupňů volnosti
        /// </summary>
        public int DegreesOfFreedom { get { return degreesOfFreedom; } }

        /// <summary>
        /// Konstruktor standardního Lagrangiánu
        /// </summary>
        /// <param name="a">Parametr 2-2 rezonance</param>
        /// <param name="b">Parametr 2-3 rezonance</param>
        public WalkerFord(double a, double b, double maxJ) {
            this.a = a;
            this.b = b;
            this.maxJ = maxJ;
        }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected WalkerFord() { }

        /// <summary>
        /// Vypíše parametry modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("a = {0,10:#####0.000}\nb = {0,10:#####0.000}", this.a, this.b));
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
            param.Add(this.a, "A");
            param.Add(this.b, "B");
            param.Add(this.maxJ, "MaxJ");
            param.Export(export);
        }

        /// <summary>
        /// Načte třídu ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public WalkerFord(Core.Import import) {
            IEParam param = new IEParam(import);
            this.a = (double)param.Get(0.0);
            this.b = (double)param.Get(0.0);
            this.maxJ = (double)param.Get(5.0);
        }
        #endregion

        private const int degreesOfFreedom = 2;

        #region IDynamicalSystem Members
        public Matrix Jacobian(Vector x) {
            throw new Exception("The method or operation is not implemented.");
        }

        public Vector IC(double e) {
            // Počáteční podmínky generujeme tak, aby H2 = 0 (výrazně to zjednodušší výpočet na hledání kořenů kvadratické rovnice)

            Vector result = new Vector(4);
            result[0] = this.random.NextDouble() * 2.0 * System.Math.PI;
            result[1] = (2.0 * result[0] - 0.5 * System.Math.PI) / 3.0;

            this.PostProcess(result);

            double t = System.Math.Cos(2.0 * result[0] - 3.0 * result[1]);

            double z = (this.a * System.Math.Cos(2.0 * (result[0] - result[1]))) - 3.0;

            while (true) {
                double j1 = this.random.NextDouble() * this.maxJ;
                double d = j1 * j1 * (z * z + 4.0) + j1 * (2.0 * z - 4.0) + 4.0 * e + 1.0;
                if(d < 0)
                    continue;
                d = System.Math.Sqrt(d);
                double j21 = 0.5 * (-1.0 - j1 * z + d);
                double j22 = 0.5 * (-1.0 - j1 * z - d);

                result[2] = j1;

                if(j21 > 0 && j21 < this.maxJ && this.random.Next(2) == 0) {
                    result[3] = j21;
                    break;
                }
                if(j22 > 0 && j22 < this.maxJ) {
                    result[3] = j22;
                    break;
                }
            }

            double f = this.E(result);

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
            double pi = System.Math.PI;
            double pi2 = 2.0 * pi;
            bool result = false;

            if(x[0] > pi) {
                x[0] -= pi2;
                result = true;
            }
            if(x[0] < -pi) {
                x[0] += pi2;
                result = true;
            }


            if(x[1] > pi) {
                x[1] -= pi2;
                result = true;
            }
            if(x[1] < -pi) {
                x[1] += pi2;
                result = true;
            }

            return result;
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
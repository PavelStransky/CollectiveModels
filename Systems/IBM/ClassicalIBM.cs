using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class ClassicalIBM : IDynamicalSystem, IExportable {
        // Generátor náhodných èísel
        private Random random = new Random();

        // Kostanty v pohybových rovnicích
        private double eta, chi;

        private double Eta { get { return this.eta; } set { this.CheckEtaChi(value, this.chi); this.eta = value; } }
        private double Chi { get { return this.chi; } set { this.CheckEtaChi(this.eta, value); this.chi = value; } }

        private double A { get { return this.eta / 2.0; } }
        private double B { get { return 2.0 * (1 - this.eta); } }
        private double C { get { return -this.chi * this.B / System.Math.Sqrt(7.0); } }
        private double D { get { return -2.0 * (this.chi * this.chi) * this.B / 7.0; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="eta">parametr eta</param>
        /// <param name="chi">Parametr chi</param>
        public ClassicalIBM(double eta, double chi) {
            this.CheckEtaChi(eta, chi);

            this.eta = eta;
            this.chi = chi;
        }

        /// <summary>
        /// Zkontroluje, zda parametry eta, chi spadají do Castenova trojúhelníku
        /// </summary>
        /// <param name="eta">parametr eta</param>
        /// <param name="chi">Parametr chi</param>
        public void CheckEtaChi(double eta, double chi) {
            if(eta < minEta || eta > maxEta)
                throw new SystemsException(string.Format(errorMessageParameterOutOfRange, "eta", eta, minEta, maxEta));

            if(chi < minChi || chi > maxChi)
                throw new SystemsException(string.Format(errorMessageParameterOutOfRange, "chi", chi, minChi, maxChi));
        }

        /// <summary>
        /// Energie
        /// </summary>
        /// <param name="v">Souøadnice a hybnosti</param>
        public double E(Vector v) {
            double x = v[0], y = v[1], px = v[2], py = v[3];

            double b2 = x * x + y * y;
            double p2 = px * px + py * py;

            double bm2 = y * y - x * x;
            double pm2 = py * py - px * px;

            double tau = b2 + p2;
            double taum = bm2 + pm2;
            double taumix = y * x + py * px;

            double r = x * (2 * y * y + taum) + 2 * y * py * px;
            double sqrt = System.Math.Sqrt(1 - tau / 2.0);

            return this.A * tau - this.B * sqrt * sqrt * b2 + this.C * sqrt * r + this.D * (4.0 * taumix * taumix + taum * taum) / 8.0;
        }

        /// <summary>
        /// Hamiltonovy pohybové rovnice
        /// </summary>
        /// <param name="v">Souøadnice a hybnosti</param>
        public Vector Equation(Vector v) {
            double x = v[0], y = v[1], px = v[2], py = v[3];

            double b2 = x * x + y * y;
            double p2 = px * px + py * py;

            double bm2 = y * y - x * x;
            double pm2 = py * py - px * px;

            double tau = b2 + p2;
            double taum = bm2 + pm2;
            double taumix = y * x + py * px;

            double r = x * (2 * y * y + taum) + 2 * y * py * px;
            double sqrt = System.Math.Sqrt(1 - tau / 2.0);

            double p = this.Eta + this.B * b2;
            double q = this.Eta + this.B * (b2 - 2.0 * sqrt * sqrt);

            Vector result = new Vector(4);
            result[0] = px * p + this.C * (2.0 * sqrt * (y * py - x * px) - px * r / sqrt / 2.0) + this.D * (py * taumix - px * taum / 2.0);
            result[1] = py * p + this.C * (2.0 * sqrt * (x * py + y * px) - py * r / sqrt / 2.0) + this.D * (px * taumix + py * taum / 2.0);

            result[2] = -x * q - this.C * (sqrt * (3.0 * bm2 + pm2) - x * r / sqrt / 2.0) - this.D * (y * taumix - x * taum / 2.0);
            result[3] = -y * q - this.C * (sqrt * (6.0 * y * x + 2.0 * py * px) - y * r / sqrt / 2.0) - this.D * (x * taumix + y * taum / 2.0);

            return result;
        }

        /// <summary>
        /// Jakobián IBM
        /// </summary>
        /// <param name="v">Souøadnice a hybnosti</param>
        public Matrix Jacobian(Vector v) {
            Matrix result = new Matrix(4);

            double x = v[0], y = v[1], px = v[2], py = v[3];

            double b2 = x * x + y * y;
            double p2 = px * px + py * py;

            double bm2 = y * y - x * x;
            double pm2 = py * py - px * px;

            double tau = b2 + p2;
            double taum = bm2 + pm2;
            double taumix = y * x + py * px;

            double r = x * (2 * y * y + taum) + 2 * y * py * px;
            double sqrt = System.Math.Sqrt(1 - tau / 2.0);
            double rsqrt2 = 0.5 / (sqrt * sqrt);
            double csqrt = this.C * sqrt;

            double p = this.Eta + this.B * b2;
            double q = this.Eta + this.B * (b2 - 2.0 * sqrt * sqrt);

            double t1 = 2.0 * (y * py - x * px);
            double t2 = 2.0 * (x * py + y * px);
            double t3 = 3.0 * bm2 + pm2;
            double t4 = 2.0 * (3.0 * x * y + px * py);

            result[0, 0] = 2.0 * this.B * x * px - csqrt * (2.0 * px + rsqrt2 * (x * t1 + px * (t3 + r * x * rsqrt2))) + this.D * (x * px + y * py);
            result[0, 1] = 2.0 * this.B * y * px + csqrt * (2.0 * py - rsqrt2 * (y * t1 + px * (t4 + r * y * rsqrt2))) + this.D * (x * py - y * px);
            result[0, 2] = p - csqrt * (2.0 * x + rsqrt2 * (r + px * (2.0 * t1 + r * px * rsqrt2))) + this.D * (p2 - taum / 2.0);
            result[0, 3] = csqrt * (2.0 * y - rsqrt2 * (py * t1 + px * (t2 + r * py * rsqrt2))) + this.D * taumix;

            result[1, 0] = 2.0 * this.B * x * py + csqrt * (2.0 * py - rsqrt2 * (x * t2 + py * (t3 + r * x * rsqrt2))) + this.D * (y * px - x * py);
            result[1, 1] = 2.0 * this.B * y * py + csqrt * (2.0 * px - rsqrt2 * (y * t2 + py * (t4 + r * y * rsqrt2))) + this.D * (x * px + y * py);
            result[1, 2] = result[0, 3];
            result[1, 3] = p + csqrt * (2.0 * x - rsqrt2 * (r + py * (2.0 * t2 + r * py * rsqrt2))) + this.D * (p2 + taum / 2.0);

            result[2, 0] = -q - 4.0 * this.B * x * x + csqrt * (6.0 * x + rsqrt2 * (r + x * (2.0 * t3 + r * x * rsqrt2))) - this.D * (b2 - taum / 2.0);
            result[2, 1] = -4.0 * this.B * x * y - csqrt * (6.0 * y - rsqrt2 * (y * t3 + x * (t4 + r * y * rsqrt2))) - this.D * taumix;
            result[2, 2] = -result[0, 0];
            result[2, 3] = -result[1, 0];

            result[3, 0] = result[2, 1];
            result[3, 1] = -q - 4.0 * this.B * y * y - csqrt * (6.0 * x - rsqrt2 * (r + y * (2.0 * t4 + r * y * rsqrt2))) - this.D * (b2 + taum / 2.0);
            result[3, 2] = -result[0, 1];
            result[3, 3] = -result[1, 1];

            return result;
        }

        /// <summary>
        /// Generování poèáteèní podmínky
        /// </summary>
        /// <param name="e">Energie</param>
        public Vector IC(double e) {
            Vector result = new Vector(4);

            // Poèet hledání
            int iteration = 0;

            do {
                // Podmínka plynoucí z odmocniny v Hamiltoniánu zní:
                // x^2 + y^2 + px^2 + py^2 <= 2
                // Pomocí ní nagenerujeme x, y, px
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
                    throw new Exception("Chyba, pøekroèen poèet iterací");

            } while(true);

            return result;
        }

        /// <summary>
        /// Generování pouze hybností v poèáteèní podmínce
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="ic">Polohy poè. podmínky</param>
        /// <returns>True, pokud se generování podaøilo</returns>
        public bool IC(Vector ic, double e) {
            // Poèet hledání
            int iteration = 0;

            double cMax2 = 2.0 - ic.SquaredEuklideanNorm(0, 1);
            bool is2 = !double.IsNaN(ic[2]);

            do {
                double max2 = cMax2;

                if(!is2) {
                    ic[2] = (2.0 * this.random.NextDouble() - 1) * System.Math.Sqrt(max2);
                    max2 -= ic[2] * ic[2];
                }

                double k = max2 / division;
                double k2 = 2 * k;

                IBMBisectionFunction bf = new IBMBisectionFunction(this, ic, 3, e);
                Bisection b = new Bisection(bf.Function);

                ArrayList roots = new ArrayList();

                // py mùže být v rozmezí [-max2; max2] - tento interval rozdìlíme na division èástí a v každé
                // zkoušíme pùlit interval
                for(int i = 0; i < division; i++) {
                    double py1 = k2 * i - max2;
                    double py2 = py1 + k;

                    double r = b.Solve(py1, py2);
                    if(!double.IsNaN(r))
                        roots.Add(r);
                }

                if(roots.Count > 0) {
                    ic[3] = (double)roots[this.random.Next(roots.Count)];
                    return true;
                }
                else if(is2)
                    return false;

                iteration++;

            } while(iteration < maxIteration);

            return false;
        }

        public Vector IC(double e, double l) {
            if(l == 0.0)
                return this.IC(e);
            else
                throw new SystemsException(string.Format(errorMessageNonzeroJ, this.GetType().FullName));
        }

        /// <summary>
        /// NEFUNGUJE!!!
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public Vector Bounds(double e) {
            Vector result = new Vector(8);

            for(int i = 0; i < 8; i++) {
                result[i] = System.Math.Sqrt(2.0);
                if(i % 2 == 0)
                    result[i] =- result[i];
            }

            return result;
        }

        /// <summary>
        /// Poèet stupòù volnosti
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
        /// <param name="x">Souøadnice a hybnosti</param>
        public bool PostProcess(Vector x) {
            return false;
        }

        /// <summary>
        /// Vypíše parametry IBM modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("eta = {0,10:#####0.000}\nchi = {1,10:#####0.000}", this.Eta, this.Chi));
            return s.ToString();
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží IBM tøídu do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.Eta, "Eta");
            param.Add(this.Chi, "Chi");

            param.Export(export);
        }

        /// <summary>
        /// Naète IBM tøídu ze souboru textovì
        /// </summary>
        /// <param name="import">Import</param>
        public ClassicalIBM(Core.Import import) {
            IEParam param = new IEParam(import);

            this.Eta = (double)param.Get();
            this.Chi = (double)param.Get();
        }
        #endregion

        private const int division = 1000;
        private const int maxIteration = 100;
        private const int degreesOfFreedom = 2;

        private const double minEta = 0.0, maxEta = 1.0;
        private static double minChi = -System.Math.Sqrt(7.0) / 2.0;
        private static double maxChi = System.Math.Sqrt(7.0) / 2.0;

        private const string errorMessageNonzeroJ = "Tøída {0} umí poèítat pouze s nulovým úhlovým momentem.";
        private const string errorMessageParameterOutOfRange = "Parametr {0} = {1} musí ležet v intervalu [{2}, {3}].";
    }
}
    
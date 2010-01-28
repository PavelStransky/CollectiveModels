using System;
using System.IO;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.DLLWrapper;

namespace PavelStransky.PT {
    /// <summary>
    /// System for studying quantum phase transitions (CUSP potential)
    /// V = x^4 + a x^2 + b x
    /// </summary>
    public partial class PT3: PT1, IMinMax {
        // Parametry modelu
        protected double a, b;

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected PT3() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="omega0">Basis frequency</param>
        /// <param name="a">Parameter a (quadratic term)</param>
        /// <param name="b">Parameter b (linear term)</param>
        /// <param name="hbar">Planck constant</param>
        public PT3(double a, double b, double omega0, double hbar)
            : base(0.0, omega0, hbar) {
            this.a = a;
            this.b = b;
        }

        /// <summary>
        /// Vypoèítá Hamiltonovu matici (v tomto pøípadì lze poèítat algebraicky)
        /// </summary>
        /// <param name="maxn">Nejvyšší hodnota kvantového èísla (energie)</param>
        public override SymmetricBandMatrix HamiltonianSBMatrix(int maxn) {
            SymmetricBandMatrix matrix = new SymmetricBandMatrix(maxn, 4);

            double alpha2 = this.omega0 / this.hbar;
            double alpha = System.Math.Sqrt(alpha2);
            double alpha4 = alpha2 * alpha2;

            double c = this.a - 0.5 * this.omega0 * this.omega0;

            // Temporary variables
            double r = this.hbar * this.omega0;
            double s = 0.5 / alpha4;
            double t = 0.5 * c / alpha2;
            double v = this.b / alpha;

            double rsqr2 = 1.0 / System.Math.Sqrt(2.0);

            for(int i = 0; i < maxn; i++) {
                double i1 = System.Math.Sqrt(i + 1);
                double i2 = i1 * System.Math.Sqrt(i + 2);
                double i3 = i2 * System.Math.Sqrt(i + 3);
                double i4 = i3 * System.Math.Sqrt(i + 4);

                matrix[i, i] = r * (i + 0.5) + 3.0 * s * (i * i + i + 0.5) + t * (2 * i + 1);
                if(i + 1 < maxn)
                    matrix[i, i + 1] = v * i1 * rsqr2;
                if(i + 2 < maxn)
                    matrix[i, i + 2] = i2 * (s * (2.0 * i + 3.0) + t);
                if(i + 4 < maxn)
                    matrix[i, i + 4] = 0.5 * s * i4;
            }

            return matrix;
        }

        /// <summary>
        /// Potential
        /// </summary>
        /// <param name="a">Parameter a</param>
        /// <param name="b">Parameter b</param>
        public static double V(double x, double a, double b) {
            return x * (x * x * x + a * x + b);
        }

        /// <summary>
        /// Potential
        /// </summary>
        public double V(double x) {
            return V(x, this.a, this.b);
        }

        /// <summary>
        /// Funkce pro nalezení minima a maxima (derivace potenciálu)
        /// </summary>
        public double BisectionMinMax(double x) {
            return x * (4.0 * x * x + 2.0 * this.a) + this.b;
        }

        /// <summary>
        /// Zpøesní polohu daného avoided crossingu
        /// </summary>
        /// <param name="n1">První hladina</param>
        /// <param name="n2">Druhá hladina</param>
        /// <param name="b1">Zaèátek intervalu, na kterém se hledá</param>
        /// <param name="b2">Konec intervalu, na kterém se hledá</param>
        /// <param name="maxn">Poèet bázových stavù pro diagonalizaci</param>
        /// <param name="precision">Koneèná pøesnost</param>
        public PointD AvoidedCrossing(int n1, int n2, double b1, double b2, int maxn, double precision) {
            Minimum m = new Minimum(this.a, b1, b2, this.omega0, this.hbar, n1, n2, maxn);
            return m.Compute(precision);
        }

        /// <summary>
        /// Vypoèítá polohy všech avoided crossingù pro všechny hladiny
        /// </summary>
        public PointVector[] AvoidedCrossings(int maxn, int precision, IOutputWriter writer) {
            double bmax = System.Math.Abs(4.0 * this.a / 3.0 * System.Math.Sqrt(-this.a / 6.0));
            double emax = this.a * this.a / 12.0;

            if(writer != null)
                writer.Write("Poèet poèítaných hladin...");

            PT3 pt3 = new PT3(this.a, -bmax, this.omega0, this.hbar);
            pt3.Compute(maxn, false, 0, null);
            Vector ev = pt3.GetEigenValues();
            int numev = ev.Length;

            // Nalezení poètu hladin, které budeme poèítat
            for(int i = 0; i < numev; i++) {
                if(ev[i] > emax) {
                    numev = i;
                    break;
                }
            }
            
            if(writer != null)
                writer.WriteLine(numev);

            // Poèet avoided crossingù pro nejvyšší hladinu
            int numav = 2 * numev - 1;

            // Støední vzdálenost mezi crossingy
            double distance = 2.0 * bmax / numav;

            // Pøesnost - aspoò 1/precision støední vzdálenosti
            double step = distance / precision;

            ArrayList[] crossings = new ArrayList[numev];
            for(int i = 0; i < numev; i++)
                crossings[i] = new ArrayList();

            // Dvì pøedhodnoty
            pt3 = new PT3(this.a, -bmax - 2.0 * step, this.omega0, this.hbar);
            pt3.Compute(maxn, false, numev, null);
            Vector ev0 = pt3.GetEigenValues();

            pt3 = new PT3(this.a, -bmax - step, this.omega0, this.hbar);
            pt3.Compute(maxn, false, numev, null);
            Vector ev1 = pt3.GetEigenValues();

            // Vlastní cyklus
            int j = 0;
            for(double b = -bmax; b <= bmax; b += step) {
                pt3 = new PT3(this.a, b, this.omega0, this.hbar);
                pt3.Compute(maxn, false, numev, null);
                ev = pt3.GetEigenValues();

                for(int i = 0; i < numev; i++) {
                    if(i > 0) {
                        double diff0 = ev0[i] - ev0[i - 1];
                        double diff1 = ev1[i] - ev1[i - 1];
                        double diff = ev[i] - ev[i - 1];

                        if(diff0 > diff1 && diff > diff1)
                            crossings[i].Add(new PointD(b - step, (ev1[i] + ev1[i - 1]) / 2.0));
                    }
                    if(i < numev - 1) {
                        double diff0 = ev0[i + 1] - ev0[i];
                        double diff1 = ev1[i + 1] - ev1[i];
                        double diff = ev[i + 1] - ev[i];

                        if(diff0 > diff1 && diff > diff1)
                            crossings[i].Add(new PointD(b - step, (ev1[i] + ev1[i + 1]) / 2.0));
                    }
                }

                ev0 = ev1;
                ev1 = ev;

                if(writer != null) {
                    if(j % numav == 0) 
                        writer.Write(j / numav);
                    
                    j++;
                    if((j % numav) % 10 == 0)
                        writer.Write('.');

                    if(j % numav == 0) 
                        writer.WriteLine();
                }
            }

            PointVector [] result = new PointVector[numev];
            for(int i = 0; i < numev; i++) {
                ArrayList a = crossings[i];
                int count = a.Count;
                PointVector v = new PointVector(count);
                j = 0;
                foreach(PointD p in a)
                    v[j++] = p;
                result[i] = v;
            }

            return result;
        }

        #region Implementace IMinMax
        public PointD VPoint(double x) {
            return new PointD(x, this.V(x));
        }

        /// <summary>
        /// Urèí lokální minima potenciálu
        /// </summary>
        public PointVector Minima(double precision) {
            PointVector result;
            Bisection bis = new Bisection(this.BisectionMinMax);

            if(this.a >= 0) {
                result = new PointVector(1);
                result[0] = this.VPoint(0.0);
            }
            else {
                if(System.Math.Abs(this.b) < 4.0 / (3.0 * System.Math.Sqrt(6.0))) {
                    result = new PointVector(2);
                    if(b < 0) {
                        result[0] = this.VPoint(bis.Solve(0.0, 1.0 - this.b, precision));
                        result[1] = this.VPoint(bis.Solve(this.b - 1.0, -System.Math.Sqrt(-this.a / 6.0), precision));
                    }
                    else {
                        result[0] = this.VPoint(bis.Solve(System.Math.Sqrt(-this.a / 6.0), 1.0 + this.b, precision));
                        result[1] = this.VPoint(bis.Solve(-1.0 - this.b, 0.0, precision));
                    }
                }
                else {
                    result = new PointVector(1);
                    if(b < 0)
                        result[0] = this.VPoint(bis.Solve(0.0, 1.0 - this.b, precision));
                    else
                        result[0] = this.VPoint(bis.Solve(-1.0 - this.b, 0.0, precision));
                }
            }

            if(double.IsNaN(result[0].X))
                return null;
            return result;
        }

        public PointVector Maxima(double precision) {
            PointVector result;

            if(this.a < 0 && System.Math.Abs(this.b) < 4.0 / (3.0 * System.Math.Sqrt(6.0))) {
                result = new PointVector(1);
                Bisection bis = new Bisection(this.BisectionMinMax);
                if(b < 0)
                    result[0] = this.VPoint(bis.Solve(-System.Math.Sqrt(-this.a / 6.0), 0.0, precision));
                else
                    result[0] = this.VPoint(bis.Solve(0.0, System.Math.Sqrt(-this.a / 6.0), precision));
            }
            else
                result = new PointVector(0);

            return result;
        }
        #endregion

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public override void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.omega0, "Omega");
            param.Add(this.a, "a");
            param.Add(this.b, "b");
            param.Add(this.hbar, "HBar");
            param.Add(this.isComputed, "IsComputed");

            if(this.isComputed) {
                param.Add(this.eigenValues, "EigenValues");

                int numEV = this.NumEV;
                param.Add(numEV, "EigenVector Number");

                for(int i = 0; i < numEV; i++)
                    param.Add(this.eigenVectors[i]);
            }

            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public PT3(Core.Import import) {
            IEParam param = new IEParam(import);

            this.omega0 = (double)param.Get(1.0);
            this.a = (double)param.Get(1.0);
            this.b = (double)param.Get(1.0);
            this.hbar = (double)param.Get(0.1);
            this.isComputed = (bool)param.Get(false);

            if(this.isComputed) {
                this.eigenValues = (Vector)param.Get(null);

                int numEV = (int)param.Get(0);
                this.eigenVectors = new Vector[numEV];

                for(int i = 0; i < numEV; i++)
                    this.eigenVectors[i] = (Vector)param.Get();
            }
        }
        #endregion
    }
}
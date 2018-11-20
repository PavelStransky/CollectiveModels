using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;

namespace PavelStransky.Systems {
    public class IntrinsicIBM : IExportable {
        private Random random = new Random();

        // Kostanty v pohybových rovnicích
        private double rho, b0;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="rho">parametr rho</param>
        /// <param name="beta0">Parametr beta0</param>
        public IntrinsicIBM(double beta0, double rho) {
            this.b0 = beta0;

            if(rho > 1.0 / beta0)
                this.rho = 1.0 / beta0 - rho;
            else
                this.rho = rho;
        }

        /// <summary>
        /// Vypíše parametry IBM modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("beta0 = {0,10:#####0.000}\nrho = {1,10:#####0.000}", this.b0, this.rho));
            return s.ToString();
        }

        public Vector LevelDensity(double emax, double precision, IOutputWriter writer) {

            int num = (int)(1 / precision);
            double[] x = new double[num];
            double[] y = new double[num];
            double[] px = new double[num];
            double[] py = new double[num];
            bool[] notUsed = new bool[num];

            // Generování počátečních podmínek Monte Carlo
            int i = 0;
            do {
                double a = this.Random();
                double b = this.Random();

                if(a * a + b * b > 2)
                    continue;

                x[i] = a;
                y[i] = b;
                notUsed[i] = true;

                i++;
            } while(i < num);
            i = 0;
            do {
                double a = this.Random();
                double b = this.Random();

                if(a * a + b * b > 2)
                    continue;

                px[i] = a;
                py[i] = b;

                i++;
            } while(i < num);


            // Celkový objem FP
            double volume = 64;

            int length = (int)System.Math.Sqrt(1 / precision);
            Vector result = new Vector(length + 1);

            if(writer != null)
                writer.Write("0");

            DateTime time = DateTime.Now;
            for(i = 0; i <= length; i++) {
                double e = i * emax / length;

                if(writer != null) {
                    if(i % 10 == 0)
                        writer.Write(".");
                    if(i % 100 == 0) {
                        writer.WriteLine(DateTime.Now - time);
                        writer.Write(e);
                        time = DateTime.Now;
                    }
                }

                int d = 0;
                for(int j = 0; j < num; j++) {
                    if(notUsed[j]) {
                        double h = this.H(x[j], y[j], px[j], py[j]);
                        if(e > h) {
                            d++;
                            notUsed[j] = false;
                        }
                    }
                }

                if(i == 0)
                    result[i] = d * volume / num;
                else
                    result[i] = result[i - 1] + d * volume / num;
            }

            return result;
        }

        private double H(double x, double y, double px, double py) {
            double h = 0;

            double r = x * x + y * y;
            double t = px * px + py * py;
            double h0 = 0.5 * (r + t);

            double a = h0 * h0 + this.b0 * this.b0 * (1 - h0) * h0;
            double b = (x * py - y * px); b *= b;
            double c = System.Math.Sqrt(0.5 * (1 - h0)) * (x * (py * py - px * px) + 2 * y * px * py - x * x * x + 3 * x * y * y);

            if(this.rho >= 0)
                h = a + this.rho * this.b0 * this.b0 * (this.rho * b + c);
            else {
                double d = x * px + y * py;
                double e = r - t;
                h = a + b + this.b0 * c - this.rho * (d * d + 0.25 * e * e - this.b0 * this.b0 * (1 - h0) * (e - this.b0 * this.b0 * (1 - h0)));
            }

            return h;
        }


        private double Random() {
            return 2.0 * System.Math.Sqrt(2) * (0.5 - this.random.NextDouble());
        }

        public double LevelDensityInt(double e, double precision, IOutputWriter writer) {
            if(e < 0)
                return 0;

            double steppb = System.Math.Sqrt(2.0) * precision;
            double steppg = precision;
            double stepb = steppb;
            double stepg = 2.0 * System.Math.PI / 3.0 * precision;

            double result = 0;

            int num = (int)(1 / precision);

            int i = 0;
            DateTime time = DateTime.Now;

            for(double pb = 0; pb <= System.Math.Sqrt(2.0); pb += steppb) {
                if(writer != null) {
                    writer.Write(".");
                    if(++i % (num / 10) == 0) {
                        writer.WriteLine(DateTime.Now - time);
                        time = DateTime.Now;
                    }
                }

                for(double pg = 0; pg <= 1; pg += steppg) {
                    double a = this.rho * this.b0 * pg; a *= a;

                    for(double b = stepb; b <= System.Math.Sqrt(2.0); b += stepb) {
                        double h0 = this.H0(b, pb, pg);
                        double c = h0 * h0 + this.b0 * this.b0 * (1.0 - h0) * h0;
                        double d = this.rho * this.b0 * this.b0 * System.Math.Sqrt(0.5 * (1 - h0));
                        double k = pg * pg / b - b * (pb * pb + b * b);
                        double l = 2.0 * pb * pg;

                        for(double g = 0; g <= 2 * System.Math.PI / 3; g += stepg) {
                            double h1 = c + a + d * (k * System.Math.Cos(3 * g) + l * System.Math.Sin(3 * g));
                            if(h1 < e)
                                result += System.Math.Cos(g) * (System.Math.Cos(g) + System.Math.Sin(g) / b);
                        }
                    }
                }
            }
            return 3 * result * stepb * stepg * steppb * steppg;
        }

        private double H0(double b, double pb, double pg) {
            return 0.5 * (this.T(b, pb, pg) + b * b);
        }

        private double T(double b, double pb, double pg) {
            return pb * pb + pg * pg / (b * b);
        }


        #region Implementace IExportable
        /// <summary>
        /// Uloží IBM třídu do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.b0, "Beta0");
            param.Add(this.rho, "Rho");

            param.Export(export);
        }

        /// <summary>
        /// Načte IBM třídu ze souboru textově
        /// </summary>
        /// <param name="import">Import</param>
        public IntrinsicIBM(Core.Import import) {
            IEParam param = new IEParam(import);

            this.b0 = (double)param.Get();
            this.rho = (double)param.Get();
        }
        #endregion
    }
}

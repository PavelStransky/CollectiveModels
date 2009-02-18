using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Tøída, která na základì zprùmìrovaného Peresova invariantu vytvoøí konturovaný graf 
    /// (zatím pro speciální øez rovinou y)
    /// </summary>
    public class AverageInvariant: DynamicalSystem {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="precision">Pøesnost výsledku</param>
        /// <param name="rkMethod">Metoda k výpoètu RK</param>
        public AverageInvariant(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod)
            : base(dynamicalSystem, precision, rkMethod) {
            if(dynamicalSystem.DegreesOfFreedom != 2)
                throw new Exception(errorMessageBadDimension);
        }

        /// <summary>
        /// Poèítá pro danou energii
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="n1">Rozmìr x výsledné matice</param>
        /// <param name="n2">Rozmìr vx výsledné matice</param>
        /// <param name="writer">Výpis na konzoli</param>
        public ArrayList Compute(double e, double time, int n1, int n2, IOutputWriter writer) {
            // Výpoèet mezí
            Vector boundX = ExactBounds.ComputeExactBounds(this.dynamicalSystem, e, n1, n2, 3);

            // Koeficienty pro rychlý pøepoèet mezi indexy a souøadnicemi n = kx + x0
            double kx = (boundX[1] - boundX[0]) / (n1 - 1);
            double x0 = boundX[0];
            double ky = (boundX[5] - boundX[4]) / (n2 - 1);
            double y0 = boundX[4];

            // Poèáteèní podmínky
            Vector ic = new Vector(4);

            int total = 0;          // Celkový poèet trajektorií

            Matrix m = new Matrix(n1, n2);
            int[,] trPassed = new int[n1, n2];

            DateTime startTime = DateTime.Now;

            for(int i = 0; i < n1; i++) {
                for(int j = 0; j < n2; j++) {
                    if((i * n2 + j + 1) % 100 == 0 && writer != null)
                        writer.Write('.');

                    // Na aktuálním bodì už máme spoèítanou trajektorii
                    if(trPassed[i, j] != 0)
                        continue;

                    ic[0] = kx * i + x0;
                    ic[1] = 0.0;
                    ic[2] = ky * j + y0; if(ic[2] == 0.0) ic[2] = double.Epsilon;
                    ic[3] = double.NaN;

                    if(this.dynamicalSystem.IC(ic, e)) {
                        ArrayList ps = new ArrayList();
                        double invariant = this.ComputePS(ps, ic, time);

                        total++;

                        bool[,] actPassed = new bool[n1, n2];

                        // Zaznamenáme i poèáteèní podmínku
                        m[i, j] = invariant;
                        trPassed[i, j]++;
                        actPassed[i, j] = true;

                        foreach(PointD p in ps) {
                            int n1x = (int)((p.X - x0) / kx);
                            int n2x = (int)((p.Y - y0) / ky);
                            if(n1x < n1 && n2x < n2 && !actPassed[n1x, n2x]) {
                                m[n1x, n2x] += invariant;
                                trPassed[n1x, n2x]++;
                                actPassed[n1x, n2x] = true;
                            }
                        }
                    }
                }

                if((i + 1) % 10 == 0 && writer != null) {
                    writer.WriteLine(string.Format("{0} ({1})", SpecialFormat.Format(DateTime.Now - startTime), total));
                }
            }

            // Total number of points
            int ntot = 0;

            for(int i = 0; i < n1; i++)
                for(int j = 0; j < n2; j++) {
                    if(trPassed[i, j] != 0) {
                        m[i, j] /= trPassed[i, j];
                        ntot++;
                    }
                    else
                        m[i, j] = -1;
                }

            ArrayList result = new ArrayList();
            result.Add(m);
            result.Add(boundX);
            result.Add(total);
            result.Add(ntot);

            return result;
        }

        private double ComputePS(ArrayList result, Vector ic, double requiredTime) {
            double invariant = 0;

            Vector x = ic;
            double y = x[1];

            double step = this.rungeKutta.Precision;
            double time = 0;

            this.rungeKutta.Init(ic);

            do {
                double newStep;
                Vector newx = x + this.rungeKutta.Step(x, ref step, out newStep);
                invariant += this.dynamicalSystem.PeresInvariant(newx) * step;

                time += step;
                step = newStep;

                double newy = newx[1];

                if(y * newy <= 0) {
                    Vector v = (x - newx) * (y / (newy - y)) + x;
                    result.Add(new PointD(v[0], v[2]));
                }

                x = newx;
                y = newy;
            } while(time < requiredTime);

            return invariant / time;
        }

        /// <summary>
        /// Vypoèítá závislost Peresova invariantu na èase
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        /// <param name="time">Èas</param>
        public PointVector TimeDependence(Vector initialX, double time) {
            PointVector result = new PointVector(defaultNumPoints);

            int finished = 0;
            
            Vector x = initialX;

            double step = this.rungeKutta.Precision;
            double t = 0.0;

            this.rungeKutta.Init(initialX);

            // Nultý krok
            result[finished].X = t;
            result[finished].Y = this.dynamicalSystem.PeresInvariant(x);
            finished++;

            do {
                double newStep;

                x += this.rungeKutta.Step(x, ref step, out newStep);

                t += step;
                step = newStep;

                result[finished].X = t;
                result[finished].Y = this.dynamicalSystem.PeresInvariant(x);
                finished++;

                if(finished >= result.Length)
                    result.Length = result.Length * 3 / 2;

            } while(t < time);

            result.Length = finished;

            return result;
        }

        private const int defaultNumPoints = 500;
        private const string errorMessageBadDimension = "Poèet stupòù volnosti pro výpoèet konturovaného grafu podle SALI musí být rovnen 2.";
    }
}

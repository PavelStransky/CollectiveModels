using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Tøída, která na základì SALI vytvoøí konturovaný graf 
    /// (zatím pro speciální øez rovinou y)
    /// </summary>
    public class SALIContourGraph: SALI {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamický systém</param>
        /// <param name="precision">Pøesnost výsledku</param>
        /// <param name="rkMethod">Metoda k výpoètu RK</param>
        public SALIContourGraph(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod)
            : base(dynamicalSystem, precision, rkMethod) {
            if(dynamicalSystem.DegreesOfFreedom != 2)
                throw new Exception(errorMessageBadDimension);
        }

        /// <summary>
        /// Poèítá SALI + Poincarého øez
        /// </summary>
        /// <param name="initialX">Poèáteèní podmínky</param>
        /// <param name="section">Body øezu (výstup)</param>
        /// <param name="time">Èas</param>
        public double TimeZeroWithPS(Vector initialX, PointVector section, double time) {
            RungeKutta rkw = new RungeKutta(new VectorFunction(this.DeviationEquation), defaultPrecision);

            this.x = initialX;
            double oldy = this.x[1];
            int finished = 0;
            section.Length = defaultNumPoints;

            Vector w1 = new Vector(initialX.Length);
            Vector w2 = new Vector(initialX.Length);
            w1[0] = 1;
            w2[initialX.Length / 2] = 1;

            double step = defaultPrecision;
            double t = 0;

            this.rungeKutta.Init(initialX);

            do {
                Vector oldx = this.x;

                double newStep, tStep;

                this.x += this.rungeKutta.Step(this.x, ref step, out newStep);
                w1 += rkw.Step(w1, ref step, out tStep);
                w2 += rkw.Step(w2, ref step, out tStep);

                t += step;

                double y = this.x[1];
                if(y * oldy <= 0) {
                    Vector v = (oldx - this.x) * (y / (y - oldy)) + oldx;
                    section[finished].X = v[0];
                    section[finished].Y = v[2];
                    finished++;

                    if(finished >= section.Length)
                        section.Length = section.Length * 3 / 2;
                }
                oldy = y;

                w1 = w1.EuklideanNormalization();
                w2 = w2.EuklideanNormalization();

            } while(t < time);

            section.Length = finished;

            return this.AlignmentIndex(w1, w2);
        }

        /// <summary>
        /// Poèítá pro danou energii
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="time">Doba výpoètu</param>
        /// <param name="n1">Rozmìr x výsledné matice</param>
        /// <param name="n2">Rozmìr vx výsledné matice</param>
        public Matrix Compute(double e, double time, int n1, int n2) {
            // Výpoèet mezí
            Vector boundX = this.dynamicalSystem.Bounds(e);

            // Koeficienty pro rychlý pøepoèet mezi indexy a souøadnicemi n = kx + x0
            double kx = (boundX[1] - boundX[0]) / (n1 - 1);
            double x0 = boundX[0];
            double ky = 2.0 * boundX[5] / (n2 - 1);
            double y0 = boundX[4];

            // Poèáteèní podmínky
            Vector ic = new Vector(4);
            ic[3] = 1.0;

            // Koeficient, který stojí pøed kinetickým èlenem
            double kic = 1.0 / this.dynamicalSystem.E(ic);

            Matrix result = new Matrix(n1, n2);
            int[,] trPassed = new int[n1, n2];

            for(int i = 0; i < n1; i++)
                for(int j = 0; j < n2; j++) {
                    // Na aktuálním bodì už máme spoèítanou trajektorii
                    if(trPassed[i, j] != 0)
                        continue;

                    ic[0] = kx * i + x0;
                    ic[1] = 0;
                    ic[2] = ky * j + y0;
                    ic[3] = 0;

                    double e0 = this.dynamicalSystem.E(ic);
                    if(e0 <= e) {
                        ic[3] = System.Math.Sqrt((e - e0) * kic);
                        
                        PointVector section = new PointVector(0);
                        double sali = this.TimeZeroWithPS(ic, section, time);

                        // 0.0 bude regulární oblast (pro SALI > 1E-5), mezi 1E-5 a 1E-15 budeme 
                        // brát log10 / 10, pro SALI < 1E-15 bude sali = 1;
                        if(sali < 1E-15)
                            sali = 1.0;
                        else
                            sali = System.Math.Max(0.1 * (-System.Math.Log10(sali) - 5.0), 0.0);

                        // Zaznamenáme i poèáteèní podmínku
                        result[i, j] = sali;
                        trPassed[i, j]++;

                        for(int k = 0; k < section.Length; k++) {
                            int n1x = (int)((section[k].X - x0) / kx);
                            int n2x = (int)((section[k].Y - y0) / ky);
                            if(result[n1x, n2x] == 0) {
                                result[n1x, n2x] += sali;
                                trPassed[n1x, n2x]++;
                            }
                        }
                    }
                }

            for(int i = 0; i < n1; i++)
                for(int j = 0; j < n2; j++)
                    if(trPassed[i, j] != 0)
                        result[i, j] /= trPassed[i, j];

            return result;
        }

        private const string errorMessageBadDimension = "Poèet stupòù volnosti pro výpoèet konturovaného grafu podle SALI musí být rovnen 2.";
    }
}

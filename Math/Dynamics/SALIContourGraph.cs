using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// T��da, kter� na z�klad� SALI vytvo�� konturovan� graf 
    /// (zat�m pro speci�ln� �ez rovinou y)
    /// </summary>
    public class SALIContourGraph: SALI {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="rkMethod">Metoda k v�po�tu RK</param>
        public SALIContourGraph(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod)
            : base(dynamicalSystem, precision, rkMethod) {
            if(dynamicalSystem.DegreesOfFreedom != 2)
                throw new Exception(errorMessageBadDimension);
        }

        /// <summary>
        /// Po��t� SALI + Poincar�ho �ez
        /// </summary>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        /// <param name="section">Body �ezu (v�stup)</param>
        /// <param name="time">�as</param>
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
        /// Po��t� pro danou energii
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="time">Doba v�po�tu</param>
        /// <param name="n1">Rozm�r x v�sledn� matice</param>
        /// <param name="n2">Rozm�r vx v�sledn� matice</param>
        public Matrix Compute(double e, double time, int n1, int n2) {
            // V�po�et mez�
            Vector boundX = this.dynamicalSystem.Bounds(e);

            // Koeficienty pro rychl� p�epo�et mezi indexy a sou�adnicemi n = kx + x0
            double kx = (boundX[1] - boundX[0]) / (n1 - 1);
            double x0 = boundX[0];
            double ky = 2.0 * boundX[5] / (n2 - 1);
            double y0 = boundX[4];

            // Po��te�n� podm�nky
            Vector ic = new Vector(4);
            ic[3] = 1.0;

            // Koeficient, kter� stoj� p�ed kinetick�m �lenem
            double kic = 1.0 / this.dynamicalSystem.E(ic);

            Matrix result = new Matrix(n1, n2);
            int[,] trPassed = new int[n1, n2];

            for(int i = 0; i < n1; i++)
                for(int j = 0; j < n2; j++) {
                    // Na aktu�ln�m bod� u� m�me spo��tanou trajektorii
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

                        // 0.0 bude regul�rn� oblast (pro SALI > 1E-5), mezi 1E-5 a 1E-15 budeme 
                        // br�t log10 / 10, pro SALI < 1E-15 bude sali = 1;
                        if(sali < 1E-15)
                            sali = 1.0;
                        else
                            sali = System.Math.Max(0.1 * (-System.Math.Log10(sali) - 5.0), 0.0);

                        // Zaznamen�me i po��te�n� podm�nku
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

        private const string errorMessageBadDimension = "Po�et stup�� volnosti pro v�po�et konturovan�ho grafu podle SALI mus� b�t rovnen 2.";
    }
}

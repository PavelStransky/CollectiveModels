using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// T��da, kter� na z�klad� zpr�m�rovan�ho Peresova invariantu vytvo�� konturovan� graf 
    /// (zat�m pro speci�ln� �ez rovinou y)
    /// </summary>
    public class AverageInvariant: DynamicalSystem {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="dynamicalSystem">Dynamick� syst�m</param>
        /// <param name="precision">P�esnost v�sledku</param>
        /// <param name="rkMethod">Metoda k v�po�tu RK</param>
        public AverageInvariant(IDynamicalSystem dynamicalSystem, double precision, RungeKuttaMethods rkMethod)
            : base(dynamicalSystem, precision, rkMethod) {
            if(dynamicalSystem.DegreesOfFreedom != 2)
                throw new Exception(errorMessageBadDimension);
        }

        /// <summary>
        /// Po��t� pro danou energii
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="n1">Rozm�r x v�sledn� matice</param>
        /// <param name="n2">Rozm�r vx v�sledn� matice</param>
        /// <param name="writer">V�pis na konzoli</param>
        public ArrayList Compute(double e, double time, int n1, int n2, IOutputWriter writer) {
            // V�po�et mez�
            Vector boundX = ExactBounds.ComputeExactBounds(this.dynamicalSystem, e, n1, n2, 3);

            // Koeficienty pro rychl� p�epo�et mezi indexy a sou�adnicemi n = kx + x0
            double kx = (boundX[1] - boundX[0]) / (n1 - 1);
            double x0 = boundX[0];
            double ky = (boundX[5] - boundX[4]) / (n2 - 1);
            double y0 = boundX[4];

            // Po��te�n� podm�nky
            Vector ic = new Vector(4);

            int total = 0;          // Celkov� po�et trajektori�

            Matrix m = new Matrix(n1, n2);
            int[,] trPassed = new int[n1, n2];

            DateTime startTime = DateTime.Now;

            for(int i = 0; i < n1; i++) {
                for(int j = 0; j < n2; j++) {
                    if((i * n2 + j + 1) % 100 == 0 && writer != null)
                        writer.Write('.');

                    // Na aktu�ln�m bod� u� m�me spo��tanou trajektorii
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

                        // Zaznamen�me i po��te�n� podm�nku
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
        /// Vypo��t� z�vislost Peresova invariantu na �ase
        /// </summary>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        /// <param name="time">�as</param>
        public PointVector TimeDependence(Vector initialX, double time) {
            PointVector result = new PointVector(defaultNumPoints);

            int finished = 0;
            
            Vector x = initialX;

            double step = this.rungeKutta.Precision;
            double t = 0.0;

            this.rungeKutta.Init(initialX);

            // Nult� krok
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
        private const string errorMessageBadDimension = "Po�et stup�� volnosti pro v�po�et konturovan�ho grafu podle SALI mus� b�t rovnen 2.";
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

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
        public SALIContourGraph(IDynamicalSystem dynamicalSystem, double precision)
            : base(dynamicalSystem, precision) {
            if(dynamicalSystem.DegreesOfFreedom != 2)
                throw new Exception(errorMessageBadDimension);
        }

        /// <summary>
        /// Vr�t� true, pokud dan� trajektorie je podle SALI regul�rn�
        /// </summary>
        /// <param name="initialX">Po��te�n� podm�nky</param>
        /// <param name="section">Body �ezu (v�stup)</param>
        public bool IsRegularPS(Vector initialX, PointVector poincareSection) {
            RungeKutta rkw = new RungeKutta(new VectorFunction(this.DeviationEquation), this.rungeKutta.Precision);

            this.x = initialX;
            double oldy = this.x[1];
            int finished = 0;
            bool result = false;

            poincareSection.Length = defaultNumPoints;

            Vector w1 = new Vector(initialX.Length);
            Vector w2 = new Vector(initialX.Length);
            w1[0] = 1;
            w2[initialX.Length / 2] = 1;

            double step = this.rungeKutta.Precision;
            double time = 0.0;

            MeanQueue queue = new MeanQueue(window);

            int i1 = (int)(1.0 / step);

            this.rungeKutta.Init(initialX);

            do {
                for(int i = 0; i < i1; i++) {
                    Vector oldx = this.x;
                    double newStep, tStep;

                    this.x += this.rungeKutta.Step(this.x, ref step, out newStep);
                    w1 += rkw.Step(w1, ref step, out tStep);
                    w2 += rkw.Step(w2, ref step, out tStep);

                    time += step;

                    step = newStep;

                    double y = this.x[1];
                    if(y * oldy <= 0) {
                        Vector v = (oldx - this.x) * (y / (y - oldy)) + oldx;
                        poincareSection[finished].X = v[0];
                        poincareSection[finished].Y = v[2];
                        finished++;

                        if(finished >= poincareSection.Length)
                            poincareSection.Length = poincareSection.Length * 3 / 2;
                    }
                    oldy = y;
                }

                w1 = w1.EuklideanNormalization();
                w2 = w2.EuklideanNormalization();

                double ai = this.AlignmentIndex(w1, w2);
                double logAI = (ai <= 0.0 ? 20.0 : -System.Math.Log10(ai));
                queue.Set(logAI);

                double meanSALI = queue.Mean;

                if(meanSALI > 6.0 + time / 1000.0) {
                    result = false;
                    break;
                }
                if(meanSALI < (time - 1000.0) / 300.0) {
                    result = true;
                    break;
                }

            } while(true);

            poincareSection.Length = finished;
            return result;
        }

        /// <summary>
        /// Zp�esn�n� mez� pro v�po�et
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="bounds">Vstupn� meze</param>
        /// <param name="n1">Rozm�r x v�sledn� matice</param>
        /// <param name="n2">Rozm�r vx v�sledn� matice</param>
        private Vector ExactBounds(Vector boundX, double e, int n1, int n2) {
            // Koeficienty pro rychl� p�epo�et mezi indexy a sou�adnicemi n = kx + x0
            double kx = (boundX[1] - boundX[0]) / (n1 - 1);
            double x0 = boundX[0];
            double ky = (boundX[5] - boundX[4]) / (n2 - 1);
            double y0 = boundX[4];

            // Po��te�n� podm�nky
            Vector ic = new Vector(4);

            // Hled�n� optim�ln� oblasti
            bool foundIC = false;

            // Sm�r 1
            for(int i = 1; i < n1; i++) {
                for(int j = 1; j < n2; j++) {
                    ic[0] = kx * i + x0;
                    ic[1] = 0.0;
                    ic[2] = ky * j + y0; if(ic[2] == 0.0) ic[2] = double.Epsilon;
                    ic[3] = 0.0;

                    if(this.dynamicalSystem.IC(ic, e)) {
                        foundIC = true;
                        break;
                    }
                }
                if(foundIC) {
                    boundX[0] = kx * (i - 2) + x0;
                    break;
                }
            }

            kx = (boundX[1] - boundX[0]) / (n1 - 1);
            x0 = boundX[0];

            foundIC = false;

            // Sm�r 4
            for(int j = 1; j < n2; j++) {
                for(int i = 1; i < n1; i++) {
                    ic[0] = kx * i + x0;
                    ic[1] = 0.0;
                    ic[2] = ky * j + y0; if(ic[2] == 0.0) ic[2] = double.Epsilon;
                    ic[3] = 0.0;

                    if(this.dynamicalSystem.IC(ic, e)) {
                        foundIC = true;
                        break;
                    }
                }
                if(foundIC) {
                    boundX[4] = ky * (j - 2) + y0;
                    break;
                }
            }

            ky = (boundX[5] - boundX[4]) / (n2 - 1);
            y0 = boundX[4];

            foundIC = false;

            // Sm�r 3
            for(int i = n2 - 2; i >= 0; i--) {
                for(int j = 1; j < n2; j++) {
                    ic[0] = kx * i + x0;
                    ic[1] = 0.0;
                    ic[2] = ky * j + y0; if(ic[2] == 0.0) ic[2] = double.Epsilon;
                    ic[3] = 0.0;

                    if(this.dynamicalSystem.IC(ic, e)) {
                        foundIC = true;
                        break;
                    }
                }
                if(foundIC) {
                    boundX[1] = kx * (i + 1) + x0;
                    break;
                }
            }

            kx = (boundX[1] - boundX[0]) / (n1 - 1);
            x0 = boundX[0];

            foundIC = false;

            // Sm�r 2
            for(int j = n2 - 2; j >= 0; j--) {
                for(int i = 1; i < n1; i++) {
                    ic[0] = kx * i + x0;
                    ic[1] = 0.0;
                    ic[2] = ky * j + y0; if(ic[2] == 0.0) ic[2] = double.Epsilon;
                    ic[3] = 0.0;

                    if(this.dynamicalSystem.IC(ic, e)) {
                        foundIC = true;
                        break;
                    }
                }
                if(foundIC) {
                    boundX[5] = ky * (j + 1) + y0;
                    break;
                }
            }

            return boundX;
        }

        /// <param name="e">Energie</param>
        /// <param name="n1">Rozm�r x v�sledn� matice</param>
        /// <param name="n2">Rozm�r vx v�sledn� matice</param>
        public ArrayList Compute(double e, int n1, int n2) {
            return this.Compute(e, n1, n2, null);
        }

        /// <summary>
        /// Po��t� pro danou energii
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="n1">Rozm�r x v�sledn� matice</param>
        /// <param name="n2">Rozm�r vx v�sledn� matice</param>
        /// <param name="writer">V�pis na konzoli</param>
        public ArrayList Compute(double e, int n1, int n2, IOutputWriter writer) {
            // V�po�et mez�
            Vector boundX = this.dynamicalSystem.Bounds(e);
            boundX = this.ExactBounds(boundX, e, n1, n2);
            boundX = this.ExactBounds(boundX, e, n1, n2);

            // Koeficienty pro rychl� p�epo�et mezi indexy a sou�adnicemi n = kx + x0
            double kx = (boundX[1] - boundX[0]) / (n1 - 1);
            double x0 = boundX[0];
            double ky = (boundX[5] - boundX[4]) / (n2 - 1);
            double y0 = boundX[4];

            // Po��te�n� podm�nky
            Vector ic = new Vector(4);

            int regular = 0;        // Po�et regul�rn�ch trajektori�
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
                    ic[3] = 0.0;

                    if(this.dynamicalSystem.IC(ic, e)) {
                        PointVector section = new PointVector(0);
                        int sali = this.IsRegularPS(ic, section) ? 1 : 0;
                        regular += sali;
                        total++;

                        bool[,] actPassed = new bool[n1, n2];

                        // Zaznamen�me i po��te�n� podm�nku
                        m[i, j] = sali;
                        trPassed[i, j]++;
                        actPassed[i, j] = true;

                        for(int k = 0; k < section.Length; k++) {
                            int n1x = (int)((section[k].X - x0) / kx);
                            int n2x = (int)((section[k].Y - y0) / ky);
                            if(n1x < n1 && n2x < n2 && !actPassed[n1x, n2x]) {
                                m[n1x, n2x] += sali;
                                trPassed[n1x, n2x]++;
                                actPassed[n1x, n2x] = true;
                            }
                        }
                    }
                }

                if((i + 1) % 10 == 0 && writer != null) {
                    writer.WriteLine(string.Format("{0} ({1}, {2})", SpecialFormat.Format(DateTime.Now - startTime), total, regular));
                }
            }

            // Number of regular points
            double nreg = 0.0;

            // Total number of points
            int ntot = 0;

            for(int i = 0; i < n1; i++)
                for(int j = 0; j < n2; j++) {
                    if(trPassed[i, j] != 0) {
                        m[i, j] /= trPassed[i, j];
                        ntot++;
                        nreg += m[i, j];
                    }
                    else
                        m[i, j] = -1;
                }

            ArrayList result = new ArrayList();
            result.Add(m);
            result.Add(boundX);
            result.Add(total);
            result.Add(regular);
            result.Add(ntot);
            result.Add(nreg);

            return result;
        }

        private const string errorMessageBadDimension = "Po�et stup�� volnosti pro v�po�et konturovan�ho grafu podle SALI mus� b�t rovnen 2.";
    }
}

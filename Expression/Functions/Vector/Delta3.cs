using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the Delta3 spectral rigidity
    /// </summary>
    public class Delta3: Fnc {
        public override string Help { get { return Messages.HelpNumberVariance; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.PInterval, Messages.PIntervalDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector data = (arguments[0] as Vector).Sort() as Vector;
            Vector lengths = arguments[1] as Vector;

            int length = data.Length;
            int lengthL = lengths.Length;

            Vector mean = new Vector(lengthL);
            Vector variance = new Vector(lengthL);

            for(int i = 0; i < lengthL; i++) {
                double L = lengths[i];
                int numInterval = (int)(length / L);
                Vector v = new Vector(numInterval);

                // Bohigas, Giannoni, Proceedings I-39 
                // (reference to Bohigas, Giannoni, Ann. Phys. N.Y. 89, 1975, 393
                double alpha = 0.0;

                // The first and the last index of the data array in a given interval
                int j1 = 0;
                int j2 = 0;           

                // First index (we start from energy 0, so there can happen to be
                // some levels below 0)
                while(data[j1] < 0)
                    j1++;

                // All intervals
                for(int j = 0; j < numInterval; j++) {
                    // Last index in the interval
                    while(j2 < data.Length && data[j2] < alpha + L)
                        j2++;

                    double shift = alpha + L / 2.0;
                    int n = j2 - j1;                // Number of elements in this interval

                    // Individual summands of the expression I-39
                    double x1 = n * n / 16.0;
                    double x2 = 0.0;
                    double x3 = 0.0;                // x3 == x4, only a coeficient is different
                    double x5 = 0.0;
                    for(int k = j1; k < j2; k++) {
                        double xt = data[k] - shift;
                        x2 += xt;
                        x3 += xt * xt;
                        x5 += (n - 2 * (k - j1) - 1) * xt;
                    }

                    v[j] = x1
                        - x2 * x2 / (L * L)
                        + 3.0 * n / (2.0 * L * L) * x3
                        - 3.0 / (L * L * L * L) * x3 * x3
                        + x5 / L;

                    if(j2 >= data.Length)
                        break;

                    j1 = j2;
                    alpha += L;
                }

                mean[i] = v.Mean();
                variance[i] = v.Variance();
            }

            List result = new List();
            result.Add(new PointVector(lengths, mean));
            result.Add(variance);
            return result;
        }
    }
}

/*
        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector data = (arguments[0] as Vector).Sort() as Vector;
            Vector lengths = arguments[1] as Vector;

            int length = data.Length;
            int lengthL = lengths.Length;

            Vector mean = new Vector(lengthL);
            Vector variance = new Vector(lengthL);

            for(int i = 0; i < lengthL; i++) {
                double L = lengths[i];
                int numInterval = (int)(length / L);
                Vector i1 = new Vector(numInterval);
                Vector i2 = new Vector(numInterval);
                Vector i3 = new Vector(numInterval);
                Vector totali = new Vector(numInterval);

                int currenti = 0;
                double currentA = data[0];

                for(int j = 1; j < length; j++) {
                    double e1 = data[j - 1];
                    double e2 = data[j];

                    if(e2 <= currentA + L) {
                        i1[currenti] += (e2 - e1) * j * j;
                        i2[currenti] += (e2 - e1) * j;
                        i3[currenti] += (0.5 * (e2 - e1) + j) * (e2 - e1);
                    }
                    else {
                        double oldcurrentA = currentA;
                        while(currentA < e2) {
                            currentA += L;
                            i1[currenti] += (currentA - oldcurrentA) * j * j;
                            i2[currenti] += (currentA - oldcurrentA) * j;
                            i3[currenti] += (0.5 * (currentA - oldcurrentA) + j) * (currentA - oldcurrentA);
                            currenti++;
                            if(currenti >= numInterval)
                                break;
                            oldcurrentA = currentA;
                        }

                        if(currenti >= numInterval)
                            break;

                        i1[currenti] += (e2 - currentA) * j * j;
                        i2[currenti] += (e2 - currentA) * j;
                        i3[currenti] += (0.5 * (e2 - currentA) + j) * (e2 - currentA);
                    }
                }

                for(int j = 0; j < numInterval; j++) {
                    i1[j] /= L;
                    i2[j] /= L; i2[j] *= i2[j];
                    i3[j] /= L * L; i3[j] *= i3[j];
                    totali[j] = i1[j] - i2[j] - 12.0 * i3[j];
                }

                mean[i] = totali.Mean();
                variance[i] = totali.Variance();
            }

            List result = new List();
            result.Add(new PointVector(lengths, mean));
            result.Add(variance);
            return result;
        }
*/
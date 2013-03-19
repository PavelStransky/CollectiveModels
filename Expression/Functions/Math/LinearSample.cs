using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the mean value of a given pointvector on an interval
    /// </summary>
    public class LinearSample : Fnc {
        public override string Help { get { return Messages.HelpLinearSample; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.PInterval, Messages.PIntervalDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector pv = (PointVector)arguments[0];
            Vector interval = (Vector)arguments[1];

            int length = (int)interval[2];
            PointVector result = new PointVector(length);

            double minx = interval[0];
            double maxx = interval[1];
            double koef = (maxx - minx) / length;

            int first = -1;
            int last = -1;

            for(int i = 0; i < length; i++) {
                double x1 = minx + koef * (i - 0.5);
                double x2 = x1 + koef;
                double x = minx + i * koef;

                double r = 0.0;
                int n = 0;
                for(int j = 0; j < pv.Length; j++)
                    if(pv[j].X >= x1 && pv[j].X <= x2) {
                        r += pv[j].Y;
                        n++;
                    }

                if(n > 0) {
                    if(first < 0)
                        first = i;
                    last = i;
                    result[i] = new PointD(x, r / n);
                }
                else
                    result[i] = new PointD(x, 0.0);
            }

            // Interpolace
            for(int i = first + 1; i < last; i++)
                if(result[i].Y == 0.0) {
                    int j = i;
                    while(result[++j].Y == 0.0) ;

                    koef = (result[j].Y - result[i - 1].Y) / (result[j].X - result[i - 1].X);

                    for(int k = i; k < j; k++)
                        result[k].Y = result[j].Y - (result[j].X - result[k].X) * koef;
                }

            return result;
        }
    }
}
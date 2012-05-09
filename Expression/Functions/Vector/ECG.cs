using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Analysis of the ECG signal
    /// </summary>
    public class ECG: Fnc {
        public override string Help { get { return Messages.HelpECG; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, false, true, true, Messages.PAverageInterval, Messages.PAverageIntervalDescription, 2.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector data = arguments[0] as PointVector;
            double interval = (double)arguments[1];

            object[] peak = this.FindNextPeak(data, 0, interval);

            ArrayList max = new ArrayList();
            while(peak != null && (peak[0] as PointVector).Length > 0) {
                PointVector pv = peak[0] as PointVector;
                max.Add(pv[pv.VectorY.MaxIndex()]);
                object[] peak1 = this.FindNextPeak(data, (int)peak[4], interval);
                peak = peak1;
            }
                
            int length = max.Count;
            PointVector result = new PointVector(length);
            for(int i = 0; i < length; i++)
                result[i] = (PointD)max[i];

            return result;
        }

        private object[] FindNextPeak(PointVector data, int start, double interval) {
            double t = data[start].X + interval;

            if(t > data.LastItem.X)
                return null;

            double mean = 0;
            int i = 0;
            for(i = start; data[i].X < t; i++)
                mean += data[i].Y;
            mean /= i;

            double var = 0;
            double max = System.Math.Abs(data[start].Y - mean);
            for(i = start; data[i].X < t; i++) {
                var += (data[i].Y - mean) * (data[i].Y - mean);
                max = System.Math.Max(System.Math.Abs(data[i].Y - mean), max);
            }
            var = System.Math.Sqrt(var / i);

            double coef = System.Math.Max(var * 2, max / 3);

            PointVector peak = new PointVector(i);
            int length = 0;
            bool init = true;
            int j = 0;
            for(j = start; data[j].X < t; j++)
                if(data[j].Y - mean > coef) {
                    if(!init)
                        peak[length++] = data[j];
                }
                else{
                    init = false;
                    if(length > 0)
                        break;
                }

            peak.Length = length;

            object[] result = new object[5];
            result[0] = peak;
            result[1] = mean;
            result[2] = var;
            result[3] = i;
            result[4] = j;
            return result;
        }
    }
}

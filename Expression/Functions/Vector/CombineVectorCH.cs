using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns cumulative histogram of the series which contains all the possible sums of elements of given vectors
    /// </summary>
    public class CombineVectorCH : Fnc {
        public override string Help { get { return Messages.HelpCombineVectorCH; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, false, Messages.PArray, Messages.PArrayDescription, null, typeof(TArray));
            this.SetParam(1, true, true, true, Messages.P1Histogram, Messages.P1HistogramDescription, null, typeof(Vector), typeof(double));
            this.SetParam(2, false, true, true, Messages.PWidth, Messages.PWidthDescription, 1.0, typeof(double));
            this.SetParam(3, false, true, false, Messages.PType, Messages.PTypeDescription, 0, typeof(int));
        }

        private class Bins {
            private double min, max;
            private int num;
            private double halfwidth;

            private double mini, maxi;

            private long sum;
            private double coef, h;

            private Vector histogram;

            public enum Types { Rectangular = 0, Cosine = 1};

            private Types type = Types.Cosine;

            public Bins(double min, double max, int num, double width, int type) {
                this.min = min;
                this.max = max;
                this.num = num;
                this.halfwidth = 0.5 * width;

                this.type = (Types)type;

                this.histogram = new Vector(this.num);

                this.coef = this.num / (this.max - this.min);
                this.h = this.coef / width;

                this.mini = 0.0;
                this.maxi = this.num * (1.0 - 1E-15);
            }

            public void Add(double d) {
                double e = (d - this.min) * coef;
                double start = e - this.halfwidth;
                double end = e + this.halfwidth;

                start = System.Math.Max(start, this.mini);
                end = System.Math.Min(end, this.maxi);

                if(start >= end)
                    return;

                this.sum++;

//                return;

                for(int k = (int)start; k < (int)end; k++) {
                    this.histogram[k] += this.Value(start, k + 1, e);                    
                    start = k + 1;
                }

                if(start < end)
                    this.histogram[(int)end] += this.Value(start, end, e);
            }

            private double Value(double start, double end, double middle) {
                switch(this.type) {
                    case Types.Rectangular:
                        return (end - start) * this.h;
                    case Types.Cosine: {
                            double x1 = System.Math.PI * (start - middle) / this.halfwidth;
                            double x2 = System.Math.PI * (end - middle) / this.halfwidth;
                            return System.Math.Sin(x2) - System.Math.Sin(x1) + x2 - x1;
                        }
                }

                return -1.0;
            }

            public PointVector GetHistogram() {
                PointVector result = new PointVector(num);

                for(int i = 0; i < this.num; i++) {
                    result[i].X = (i + 0.5) / coef + this.min;
                    result[i].Y = this.histogram[i];
                }

                return result;
            }

            public long Num { get { return this.sum; } }
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            TArray vs = arguments[0] as TArray;
            int length = vs.Length;

            double min = 0.0;
            double max = 0.0;
            int num = 0;

            for(int i = 0; i < length; i++) {
                    Vector v = vs[i] as Vector;
                    v = v.Sort() as Vector;
                    min += v.FirstItem;
                    max += v.LastItem;
                }

            if(arguments[1] is Vector) {
                Vector lim = arguments[1] as Vector;
                if(lim.Length >= 3) {
                    min = lim[0];
                    max = lim[1];
                    num = (int)lim[2];
                }
                else if(lim.Length == 2) {
                    max = lim[0];
                    num = (int)lim[1];
                }
                else if(lim.Length == 1)
                    num = (int)lim[0];
            }
            else
                num = (int)((double)arguments[1]);

            double width = (double)arguments[2];

            Bins bins = new Bins(min, max, num, width, (int)arguments[3]);

            int k = 0;
            this.Recursion(bins, ref k, vs, vs.Length, 0.0, guider);

//            return bins.Num;
            return bins.GetHistogram();
        }

        private void Recursion(Bins bins, ref int k, TArray vs, int i, double sum, Guider guider) {
            if(i > 0) {
                i--;

                Vector v = vs[i] as Vector;
                int length = v.Length;
                int coef = length / 100;

                for(int j = 0; j < length; j++) {
                    this.Recursion(bins, ref k, vs, i, sum + v[j], null);
                    if(guider != null && (j + 1) % coef == 0) {
                        guider.Write('.');
                        if((j + 1) % (10 * coef) == 0)
                            guider.WriteLine(bins.Num);
                    }
                }
            }
            else
                bins.Add(sum);
        }
    }
}
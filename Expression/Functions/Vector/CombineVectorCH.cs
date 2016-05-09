using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Core;

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

            private long sum;
            private double coef;

            private double h;

            private Vector histogram;

            public enum Types { Rectangular = 0, Cosine = 1};

            private Types type = Types.Rectangular;

            public Bins(double min, double max, int num, double width, int type) {
                this.min = min;
                this.max = max;
                this.num = num;
                this.halfwidth = 0.5 * width;

                this.h = System.Math.PI / (this.halfwidth + 0.5);

                this.type = (Types)type;
                this.histogram = new Vector(this.num + 1);
                this.coef = this.num / (this.max - this.min);
            }

            public void Add(PointD p) {
                double d = p.X;
                double v = p.Y;

                double e = (d - this.min) * this.coef;

                double start = e - this.halfwidth;
                int starti = (int)start;

                double end = e + this.halfwidth;
                int endi = (int)end;

                if(starti < 0)
                    starti = 0;
                if(endi > this.num)
                    endi = this.num;

                if(starti > endi)
                    return;

                this.sum++;

                if(starti == endi && starti > 0 && endi < this.num) {
                    this.histogram[starti] += v;
                    return;
                }

                switch(this.type) {
                    case Types.Rectangular: {
                            v /= (2.0 * this.halfwidth);

                            if(start > 0)
                                this.histogram[starti] += v * (1.0 - start + starti);
                            else
                                this.histogram[starti] += v;

                            for(int k = starti + 1; k < endi; k++)
                                this.histogram[k] += v;

                            if(end < this.num)
                                this.histogram[endi] += v * (end - endi);

                            break;
                        }
                    case Types.Cosine: {
                            v /= 2.0 * System.Math.PI;

                            double x1 = 0.0;

                            if(start < 0)
                                x1 = this.h * (1.0 - start + starti);

                            double l = 2.0 - start;

                            for(int k = starti; k < endi; k++) {
                                double x2 = this.h * (k + l);
                                x2 = x2 - System.Math.Sin(x2);
                                this.histogram[k] += v * (x2 - x1);
                                x1 = x2;
                            }

                            if(end < this.num)
                                this.histogram[endi] += v * (2.0 * System.Math.PI - x1);

                            break;
                        }
                }
            }

            public PointVector GetHistogram() {
                PointVector result = new PointVector(num);

                for(int i = 0; i < this.num; i++) {
                    result[i].X = (i + 0.5) / this.coef + this.min;
                    result[i].Y = this.coef * this.histogram[i];
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

            if(vs.GetItemType() == typeof(Vector)) {
                PointVector[] p = new PointVector[length];

                Vector v = new Vector((vs[0] as Vector).Length);

                for(int i = 1; i < vs.Length; i++)
                    p[i] = new PointVector(vs[i] as Vector, v);

                for(int i = 0; i < v.Length; i++)
                    v[i] = 1.0;

                p[0] = new PointVector(vs[0] as Vector, v);
                vs = new TArray(p);
            }

            for(int i = 0; i < length; i++) {
                    PointVector v = vs[i] as PointVector;
                    v = v.Sort() as PointVector;
                    min += v.VectorX.FirstItem;
                    max += v.VectorX.LastItem;
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
           
            this.Recursion(bins, vs, vs.Length, new PointD(0.0, 0.0), guider);
            return bins.GetHistogram();
        }

        private void Recursion(Bins bins, TArray vs, int i, PointD sum, Guider guider) {
            if(i > 0) {
                i--;

                PointVector v = vs[i] as PointVector;
                int length = v.Length;
                int coef = length / 100;
                DateTime t = DateTime.Now;

                for(int j = 0; j < length; j++) {
                    this.Recursion(bins, vs, i, sum + v[j]);

                    if(guider != null && (j + 1) % coef == 0) {
                        guider.Write('.');
                        if((j + 1) % (10 * coef) == 0) {
                            guider.Write(bins.Num);
                            guider.Write("...");
                            guider.WriteLine(SpecialFormat.Format(DateTime.Now - t));
                            t = DateTime.Now;
                        }
                    }
                }
            }
            else
                bins.Add(sum);
        }

        private void Recursion(Bins bins, TArray vs, int i, PointD sum) {
            if(i > 0) {
                i--;

                PointVector v = vs[i] as PointVector;
                int length = v.Length;
                int coef = length / 100;

                for(int j = 0; j < length; j++) 
                    this.Recursion(bins, vs, i, sum + v[j]);
            }
            else
                bins.Add(sum);
        }
    }
}
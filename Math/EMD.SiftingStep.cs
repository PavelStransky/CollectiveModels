using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Empirical Mode Decomposition
    /// </summary>
    /// <remarks>Irving y Emmanuel, marzo 2011</remarks>
    public partial class EMD {
        private class SiftingStep {
            private PointVector maxima;
            private PointVector minima;
            private double errorU = 0.0;
            private double errorL = 0.0;
            private int symmetryBreak = 0;
            private PointVector result;

            /// <summary>
            /// Number of (real) maxima
            /// </summary>
            public int MaxNum { get { return this.maxima.Length; } }

            /// <summary>
            /// Number of (real) minima
            /// </summary>
            public int MinNum { get { return this.minima.Length; } }

            /// <summary>
            /// Number of maxima below zero
            /// </summary>
            public int MaxNumBelow0 {
                get {
                    int length = this.maxima.Length - 1;
                    int result = 0;
                    for(int i = 1; i < length; i++)
                        if(this.maxima[i].Y <= 0.0)
                            result++;
                    return result;
                }
            }

            /// <summary>
            /// Number of minima above zero
            /// </summary>
            public int MinNumAbove0 {
                get {
                    int length = this.minima.Length - 1;
                    int result = 0;
                    for(int i = 1; i < length; i++)
                        if(this.minima[i].Y >= 0.0)
                            result++;
                    return result;
                }
            }

            /// <summary>
            /// Number of extreemes that are on the wrong side of the x axis
            /// (and therefore determine the difference between extreemes and zero crossings)
            /// </summary>
            public int NumAssymetryExtreemes {
                get {
                    return this.MinNumAbove0 + this.MaxNumBelow0;
                }
            }

            /// <summary>
            /// We found the residuum
            /// </summary>
            public bool IsResiduum { get { return this.MaxNum < 2 || this.MinNum < 2; } }

            /// <summary>
            /// Result - a candidate for an IMF
            /// </summary>
            public PointVector Result { get { return this.result; } }

            /// <summary>
            /// Upper cumulative error
            /// </summary>
            public double ErrorU { get { return this.errorU; } }

            /// <summary>
            /// Lower cumulative error
            /// </summary>
            public double ErrorL { get { return this.errorL; } }

            /// <summary>
            /// Number of points that breaks the symmetry condition |U+L|/|U,L| leq delta
            /// </summary>
            public int SymmetryBreak { get { return this.symmetryBreak; } }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="data">Data</param>
            /// <param name="flat">True if the flat parts of the level density is going to be considered 
            /// as a source of maxima / minima</param>
            /// <param name="delta">A special parameter for the symmetry condition |U+L|/|U,L| leq delta</param>
            public SiftingStep(PointVector data, bool flat, double delta) {
                this.maxima = this.Maxima(data, flat);
                this.minima = this.Minima(data, flat);

                if(this.IsResiduum)
                    return;

                Spline maximaSpline = new Spline(this.maxima);
                Spline minimaSpline = new Spline(this.minima);

                int length = data.Length;
                this.result = new PointVector(length);

                Vector x = data.VectorX;
                Vector u = maximaSpline.GetValue(x);
                Vector l = minimaSpline.GetValue(x);

                for(int i = 0; i < length; i++) {
                    double m = 0.5 * (u[i] + l[i]);

                    this.result[i] = new PointD(data[i].X, data[i].Y - m);

                    double uc = System.Math.Abs(m) / System.Math.Abs(u[i]);
                    double lc = System.Math.Abs(m) / System.Math.Abs(l[i]);

                    if(delta > 0.0 && (uc > delta || lc > delta))
                        this.symmetryBreak++;

                    this.errorU += uc;
                    this.errorL += lc;
                }
            }

            /// <summary>
            /// Calculates maxima (with all corrections)
            /// </summary>
            private PointVector Maxima(PointVector data, bool flat) {
                int length = data.Length;
                ArrayList maxima = new ArrayList();

                int lastIndex = 0;
                bool max = true;
                bool flatMax = false;

                bool firstMax = false;

                for(int i = 1; i < length; i++) {
                    if(data[i].Y > data[lastIndex].Y) {
                        if(flatMax && flat) {
                            if(lastIndex == 0)
                                firstMax = true;
                            else if(firstMax) {
                                if(data[lastIndex].Y > ((PointD)maxima[0]).Y)
                                    maxima.RemoveAt(0);
                                firstMax = false;
                            }
                            maxima.Add(data[lastIndex]);
                        }

                        lastIndex = i;
                        max = true;
                        flatMax = false;
                    }
                    else if(data[i].Y < data[lastIndex].Y) {
                        if(max) {
                            if(lastIndex == 0)
                                firstMax = true;
                            else if(firstMax) {
                                if(data[lastIndex].Y > ((PointD)maxima[0]).Y)
                                    maxima.RemoveAt(0);
                                firstMax = false;
                            }
                            maxima.Add(new PointD(0.5 * (data[lastIndex].X + data[i - 1].X), data[lastIndex].Y));
                        }
                        else if(flatMax && flat) {
                            if(firstMax) {
                                if(data[i - 1].Y > ((PointD)maxima[0]).Y)
                                    maxima.RemoveAt(0);
                                firstMax = false;
                            }
                            maxima.Add(data[i - 1]);
                        }

                        flatMax = false;
                        max = false;
                        lastIndex = i;
                    }
                    else if(data[i].Y == data[lastIndex].Y)
                        flatMax = true;
                }

                // Last element
                if(max && maxima.Count > 0 && data[lastIndex].Y > ((PointD)maxima[maxima.Count - 1]).Y)
                    maxima.Add(new PointD(0.5 * (data[lastIndex].X + data.LastItem.X), data[lastIndex].Y));

                length = maxima.Count;

                if(length > 1)
                    length += 2;
                PointVector result = new PointVector(length);

                int j = (length > 1) ? 1 : 0;
                foreach(PointD p in maxima)
                    result[j++] = p;

                // Add a maximum to the beginning and the end
                if(length > 1) {
                    double df = result[2].X - result[1].X;
                    double lf = result[1].X - data.FirstItem.X;
                    if(df > lf)
                        result.FirstItem = new PointD(result[1].X - df, result[1].Y);
                    else
                        result.FirstItem = new PointD(data.FirstItem.X - lf, result[1].Y);

                    df = result[length - 2].X - result[length - 3].X;
                    lf = data.LastItem.X - result[length - 2].X;
                    if(df > lf)
                        result.LastItem = new PointD(result[length - 2].X + df, result[length - 2].Y);
                    else
                        result.LastItem = new PointD(data.LastItem.X + lf, result[length - 2].Y);
                }

                return result;
            }

            /// <summary>
            /// Calculates minima (with all corrections)
            /// </summary>
            private PointVector Minima(PointVector data, bool flat) {
                int length = data.Length;
                ArrayList minima = new ArrayList();

                int lastIndex = 0;
                bool min = true;
                bool flatMin = false;

                bool firstMin = false;

                for(int i = 1; i < length; i++) {
                    if(data[i].Y < data[lastIndex].Y) {
                        if(flatMin && flat) {
                            if(lastIndex == 0)
                                firstMin = true;
                            else if(firstMin) {
                                if(data[lastIndex].Y < ((PointD)minima[0]).Y)
                                    minima.RemoveAt(0);
                                firstMin = false;
                            }
                            minima.Add(data[lastIndex]);
                        }

                        lastIndex = i;
                        min = true;
                        flatMin = false;
                    }
                    else if(data[i].Y > data[lastIndex].Y) {
                        if(min) {
                            if(lastIndex == 0)
                                firstMin = true;
                            else if(firstMin) {
                                if(data[lastIndex].Y < ((PointD)minima[0]).Y)
                                    minima.RemoveAt(0);
                                firstMin = false;
                            }
                            minima.Add(new PointD(0.5 * (data[lastIndex].X + data[i - 1].X), data[lastIndex].Y));
                        }
                        else if(flatMin && flat) {
                            if(firstMin) {
                                if(data[i - 1].Y < ((PointD)minima[0]).Y)
                                    minima.RemoveAt(0);
                                firstMin = false;
                            }
                            minima.Add(data[i - 1]);
                        }

                        flatMin = false;
                        min = false;
                        lastIndex = i;
                    }
                    else if(data[i].Y == data[lastIndex].Y)
                        flatMin = true;
                }

                // Last element
                if(min && minima.Count > 0 && data[lastIndex].Y < ((PointD)minima[minima.Count - 1]).Y)
                    minima.Add(new PointD(0.5 * (data[lastIndex].X + data.LastItem.X), data[lastIndex].Y));

                length = minima.Count;

                if(length > 1)
                    length += 2;
                PointVector result = new PointVector(length);

                int j = (length > 1) ? 1 : 0;
                foreach(PointD p in minima)
                    result[j++] = p;

                // Add a minimum to the beginning and the end
                if(length > 1) {
                    double df = result[2].X - result[1].X;
                    double lf = result[1].X - data.FirstItem.X;
                    if(df > lf)
                        result.FirstItem = new PointD(result[1].X - df, result[1].Y);
                    else
                        result.FirstItem = new PointD(data.FirstItem.X - lf, result[1].Y);

                    df = result[length - 2].X - result[length - 3].X;
                    lf = data.LastItem.X - result[length - 2].X;
                    if(df > lf)
                        result.LastItem = new PointD(result[length - 2].X + df, result[length - 2].Y);
                    else
                        result.LastItem = new PointD(data.LastItem.X + lf, result[length - 2].Y);
                }

                return result;
            }        
        }
    }
}

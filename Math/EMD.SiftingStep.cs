using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Empirical Mode Decomposition
    /// </summary>
    /// <remarks>Irving y Emmanuel, marzo 2011</remarks>
    public partial class EMD {
        private class SiftingStep {
            private PointVector maxima, maximaBorder;
            private PointVector minima, minimaBorder;
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
                    int length = this.maximaBorder.Length - 1;
                    int result = 0;
                    for(int i = 1; i < length; i++)
                        if(this.maximaBorder[i].Y <= 0.0)
                            result++;
                    return result;
                }
            }

            /// <summary>
            /// Number of minima above zero
            /// </summary>
            public int MinNumAbove0 {
                get {
                    int length = this.minimaBorder.Length - 1;
                    int result = 0;
                    for(int i = 1; i < length; i++)
                        if(this.minimaBorder[i].Y >= 0.0)
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
            public bool IsResiduum { get { return this.MaxNum <= 1 || this.MinNum <= 1; } }

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
            /// <param name="boundary">Boundary condition</param>
            public SiftingStep(PointVector data, bool flat, double delta, EMD.Boundary boundary) {
                this.maxima = data.Maxima(flat);
                this.minima = data.Minima(flat);

                if(this.IsResiduum)
                    return;

                this.maximaBorder = this.CorrectBorder(this.maxima, data.FirstItem.X, data.LastItem.X, boundary);
                this.minimaBorder = this.CorrectBorder(this.minima, data.FirstItem.X, data.LastItem.X, boundary);

                Spline maximaSpline = new Spline(this.maximaBorder);
                Spline minimaSpline = new Spline(this.minimaBorder);

                int length = data.Length;
                this.result = new PointVector(length);

                for(int i = 0; i < length; i++) {
                    double x = data[i].X;
                    double u = maximaSpline.GetValue(x);
                    double l = minimaSpline.GetValue(x);
                    double m = 0.5 * (u + l);

                    this.result[i] = new PointD(data[i].X, data[i].Y - m);

                    double uc = System.Math.Abs(m) / System.Math.Abs(u);
                    double lc = System.Math.Abs(m) / System.Math.Abs(l);

                    if(delta > 0.0 && (uc > delta || lc > delta))
                        this.symmetryBreak++;

                    this.errorU += uc;
                    this.errorL += lc;
                }
            }

            /// <summary>
            /// Corrected the initial and final values of the given time series
            /// </summary>
            /// <param name="source">Source time series</param>
            /// <param name="minX">First x value of the source data</param>
            /// <param name="maxX">Last x value of the source data</param>
            /// <param name="boundary">Boundary condition</param>
            private PointVector CorrectBorder(PointVector source, double minX, double maxX, EMD.Boundary boundary) {
                int addPoints = 0;
                if(source.FirstItem.X > minX)
                    addPoints++;
                if(source.LastItem.X < maxX)
                    addPoints++;

                if(addPoints == 0)
                    return source;

                int length = source.Length;
                int newLength = length + addPoints;
                PointVector result = new PointVector(newLength);

                // We add one maximum to the beginning of the time series
                if(source.FirstItem.X > minX) {
                    for(int i = length; i > 0; i--)
                        result[i] = source[i - 1];
                    if(boundary == Boundary.First)
                        result.FirstItem = new PointD(minX, source.FirstItem.Y);
                    else {
                        if(source.Length > 1 && source.FirstItem.X - minX < source[1].X - source.FirstItem.X)
                            result.FirstItem = new PointD(source.FirstItem.X - source[1].X, source.FirstItem.Y);
                        else
                            result.FirstItem = new PointD(minX - source.FirstItem.X, source.FirstItem.Y);
                    }
                }
                else {
                    for(int i = 0; i < length; i++)
                        result[i] = source[i];

                    result[0].Y = source[1].Y;
                }

                // We add one maximum to the end of the time series
                if(source.LastItem.X < maxX) {
                    if(boundary == Boundary.First)
                        result.LastItem = new PointD(maxX, source.LastItem.Y);
                    else {
                        if(source.Length > 1 && maxX - source.LastItem.X < source.LastItem.X - source[source.Length - 2].X)
                            result.LastItem = new PointD(2.0*source.LastItem.X - source[source.Length - 2].X, source.LastItem.Y);
                        else
                            result.LastItem = new PointD(2.0*maxX - source.FirstItem.X, source.LastItem.Y);
                    }
                }
                else
                    result.LastItem.Y = source[source.Length - 2].Y;

                return result;
            }
        }
    }
}

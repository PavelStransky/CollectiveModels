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
            public bool IsResiduum { get { return this.MaxNum <= 2 || this.MinNum <= 2; } }

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
                this.maxima = this.CorrectBorder(data.Maxima(flat), data.FirstItem.X, data.LastItem.X);
                this.minima = this.CorrectBorder(data.Minima(flat), data.FirstItem.X, data.LastItem.X);

                if(this.IsResiduum)
                    return;

                Spline maximaSpline = new Spline(this.maxima);
                Spline minimaSpline = new Spline(this.minima);

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
            private PointVector CorrectBorder(PointVector source, double minX, double maxX) {
                int length = source.Length;
                int newLength = length;

                if(source.FirstItem.X != minX)
                    newLength++;
                if(source.LastItem.X != maxX)
                    newLength++;

                if(length == newLength)
                    return source;
                
                PointVector result = new PointVector(newLength);

                if(source.FirstItem.X != minX) {    // Reflection
                    result.FirstItem.X = 2 * minX - source.FirstItem.X;
                    result.FirstItem.Y = source.FirstItem.Y;

                    for(int i = 0; i < length; i++)
                        result[i + 1] = source[i];
                }
                else
                    for(int i = 0; i < length; i++)
                        result[i] = source[i];

                if(source.LastItem.X != maxX) {
                    result.LastItem.X = 2 * maxX - source.LastItem.X;
                    result.LastItem.Y = source.LastItem.Y;
                }

                return result;
            }
        }
    }
}

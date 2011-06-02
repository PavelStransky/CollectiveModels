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
            /// Constructor
            /// </summary>
            /// <param name="data">Data</param>
            /// <param name="flat">True if the flat parts of the level density is going to be considered 
            /// as a source of maxima / minima</param>
            public SiftingStep(PointVector data, bool flat) {
                this.maxima = data.Maxima(flat);
                this.minima = data.Minima(flat);

                if(this.IsResiduum)
                    return;

                this.maximaBorder = this.CorrectBorder(this.maxima, data.FirstItem.X, data.LastItem.X);
                this.minimaBorder = this.CorrectBorder(this.minima, data.FirstItem.X, data.LastItem.X);

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
                    this.errorU += System.Math.Abs(m) / System.Math.Abs(u);
                    this.errorL += System.Math.Abs(m) / System.Math.Abs(l);
                }
            }

            /// <summary>
            /// Corrected the initial and final values of the given time series
            /// </summary>
            /// <param name="source">Source time series</param>
            private PointVector CorrectBorder(PointVector source, double minX, double maxX) {
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
                    result.FirstItem = new PointD(minX, source.FirstItem.Y);
                }
                else
                    for(int i = 0; i < length; i++)
                        result[i] = source[i];

                // We add one maximum to the end of the time series
                if(source.LastItem.X < maxX)
                    result.LastItem = new PointD(maxX, source.LastItem.Y);

                return result;
            }
        }
    }
}

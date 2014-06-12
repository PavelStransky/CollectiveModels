using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Maximum envelope of the given data
    /// </summary>
    public class ExtremesEnvelope {
        private PointVector data;
        private bool flat;

        private PointVector extremes;
        private Spline spline;

        public PointVector Extremes { get { return this.extremes; } }

        /// <summary>
        /// Number of (real) maxima
        /// </summary>
        public int NumExtremes { get { return this.extremes.Length; } }

        /// <summary>
        /// Number of maxima below zero
        /// </summary>
        public int NumExtremesBelow0 {
            get {
                int length = this.extremes.Length - 1;
                int result = 0;
                for(int i = 1; i < length; i++)
                    if(this.extremes[i].Y <= 0.0)
                        result++;
                return result;
            }
        }

        /// <summary>
        /// A value on the envelope approximated by the cubic spline
        /// </summary>
        /// <param name="x">x coordinates on the curve</param>
        public Vector GetValue(Vector x) {
            return this.spline.GetValue(x);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="flat">True if the flat parts of the level density is going to be considered 
        /// as a source of maxima / minima</param>
        public ExtremesEnvelope(PointVector data, bool flat) {
            this.data = data;
            this.flat = flat;

            this.Compute();

            if(this.extremes.Length > 1)
                this.spline = new Spline(this.extremes);
            else
                this.spline = null;
        }

        /// <summary>
        /// Calculates maxima (with all corrections)
        /// </summary>
        private void Compute() {
            int length = this.data.Length;
            ArrayList maxima = new ArrayList();

            int lastIndex = 0;
            bool max = true;
            bool flatMax = false;

            bool firstMax = false;

            for(int i = 1; i < length; i++) {
                if(this.data[i].Y > this.data[lastIndex].Y) {
                    if(flatMax && this.flat) {
                        if(lastIndex == 0)
                            firstMax = true;
                        else if(firstMax) {
                            if(this.data[lastIndex].Y > ((PointD)maxima[0]).Y)
                                maxima.RemoveAt(0);
                            firstMax = false;
                        }
                        maxima.Add(this.data[lastIndex]);
                    }

                    lastIndex = i;
                    max = true;
                    flatMax = false;
                }
                else if(this.data[i].Y < this.data[lastIndex].Y) {
                    if(max) {
                        if(lastIndex == 0)
                            firstMax = true;
                        else if(firstMax) {
                            if(this.data[lastIndex].Y > ((PointD)maxima[0]).Y)
                                maxima.RemoveAt(0);
                            firstMax = false;
                        }
                        maxima.Add(new PointD(0.5 * (this.data[lastIndex].X + this.data[i - 1].X), this.data[lastIndex].Y));
                    }
                    else if(flatMax && this.flat) {
                        if(firstMax) {
                            if(this.data[i - 1].Y > ((PointD)maxima[0]).Y)
                                maxima.RemoveAt(0);
                            firstMax = false;
                        }
                        maxima.Add(this.data[i - 1]);
                    }

                    flatMax = false;
                    max = false;
                    lastIndex = i;
                }
                else if(this.data[i].Y == this.data[lastIndex].Y)
                    flatMax = true;
            }

            // Last element
            if(max && maxima.Count > 0 && this.data[lastIndex].Y > ((PointD)maxima[maxima.Count - 1]).Y)
                maxima.Add(new PointD(0.5 * (this.data[lastIndex].X + this.data.LastItem.X), this.data[lastIndex].Y));

            length = maxima.Count;

            if(length > 1)
                length += 2;
            this.extremes = new PointVector(length);

            int j = (length > 1) ? 1 : 0;
            foreach(PointD p in maxima)
                this.extremes[j++] = p;

            // Add a maximum to the beginning and the end
            if(length > 1) {
                double df = this.extremes[2].X - this.extremes[1].X;
                double lf = this.extremes[1].X - this.data.FirstItem.X;
                if(df > lf)
                    this.extremes.FirstItem = new PointD(this.extremes[1].X - df, this.extremes[1].Y);
                else
                    this.extremes.FirstItem = new PointD(this.data.FirstItem.X - lf, this.extremes[1].Y);

                df = this.extremes[length - 2].X - this.extremes[length - 3].X;
                lf = this.data.LastItem.X - this.extremes[length - 2].X;
                if(df > lf)
                    this.extremes.LastItem = new PointD(this.extremes[length - 2].X + df, this.extremes[length - 2].Y);
                else
                    this.extremes.LastItem = new PointD(this.data.LastItem.X + lf, this.extremes[length - 2].Y);
            }
        }
    }
}


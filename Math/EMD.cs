using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Empirical Mode Decomposition
    /// </summary>
    /// <remarks>Irving y Emmanuel, marzo 2011</remarks>
    public class EMD {
        PointVector data;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Time series</param>
        public EMD(PointVector data){
            int length = data.Length;
            this.data = data;
        }
               
        public PointVector[] Compute(IOutputWriter writer, int s) {
            int length = this.data.Length;

            PointVector resid = this.data.Clone() as PointVector;
            ArrayList result = new ArrayList();

            for(int i = 0; i < 100; i++) {
                if(writer != null)
                    writer.Write(string.Format("IMF{0}...", i));

                PointVector imf = resid.Clone() as PointVector;                
                int[] errors = this.Sifting(imf, s, writer);
                if(errors[0] == 0) {
                    result.Add(imf);

                    if(writer != null)
                        writer.WriteLine(string.Format("{0} (Max; Min; Zero) = ({1}; {2}; {3})",
                            errors[4], errors[1], errors[2], errors[3]));

                    resid = new PointVector(resid.VectorX, resid.VectorY - imf.VectorY);
                }
                else {
                    if(writer != null)
                        writer.WriteLine(string.Format("residuum found (Max; Min) = ({0}; {1})",
                            errors[1], errors[2]));

                    result.Add(resid);
                    break;
                }
            }

            int count = result.Count;
            PointVector[] resultPV = new PointVector[count];
            for(int i = 0; i < count; i++)
                resultPV[i] = result[i] as PointVector;
            return resultPV;
        }

        /// <summary>
        /// Sifting iterative process for 1 IMF
        /// </summary>
        /// <param name="source">Source data</param>
        /// <param name="s">Number of iterations after the condition (MaxNum + MinNum - Cross0) is reached</param>
        private int[] Sifting(PointVector source, int s, IOutputWriter writer) {
            int i = 0;
            int iterations = 0;

            int[] errors = null;

            while(i < s) {
                errors = this.SiftH(source);
                if(errors[0] > 0)           // We found the residuum
                    return errors;

                if(System.Math.Abs(errors[1] + errors[2] - errors[3]) <= 2)
                    i++;
                else
                    i = 0;

                iterations++;
            }

            errors[4] = iterations;
            return errors;
        }

        /// <summary>
        /// One sifting step
        /// </summary>
        private int[] SiftH(PointVector source) {
            PointVector maxima = source.Maxima();
            PointVector minima = source.Minima();

            int[] errors = new int[5];
            errors[1] = maxima.Length;          // Number of real maxima
            errors[2] = minima.Length;          // Number of real minima

            if(errors[1] <= 1 || errors[2] <= 1) {
                errors[0] = 1;                  // We find residuum
                return errors;
            }

            PointVector maximaBorder = this.CorrectBorder(maxima, source.FirstItem.X, source.LastItem.X);
            PointVector minimaBorder = this.CorrectBorder(minima, source.FirstItem.X, source.LastItem.X);

            Spline maximaSpline = new Spline(maximaBorder);
            Spline minimaSpline = new Spline(minimaBorder);

            int length = source.Length;
            PointVector result = new PointVector(length);
            for(int i = 0; i < length; i++) {
                double x = source[i].X;
                double m = 0.5 * (maximaSpline.GetValue(x) + minimaSpline.GetValue(x));
                source[i].Y -= m;

                if(i != 0 && (source[i - 1].Y == 0.0 || source[i - 1].Y * source[i].Y < 0))
                    errors[3]++;                // Number of zero crossings
            }

            return errors;
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
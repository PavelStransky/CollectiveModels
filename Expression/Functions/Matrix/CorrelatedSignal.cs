using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates correlation matrix according to the article PRE 71, 046116 (2005)                                                 
    /// </summary>
    /// <remarks>
    /// Detection and characterization of changes of the correlation structure 
    /// in multivariate time series                                            
    ///</remarks>
    public class CorrelatedSignal: Fnc {
        public override string Help { get { return Messages.HelpRegression; } }

        protected override void CreateParameters() {
            this.SetNumParams(7);
            this.SetParam(0, true, true, false, Messages.P1CorrelatedSignal, Messages.P1CorrelatedSignalDescription, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.P2CorrelatedSignal, Messages.P2CorrelatedSignalDescription, null, typeof(Matrix));
            this.SetParam(2, true, true, false, Messages.P3CorrelatedSignal, Messages.P3CorrelatedSignalDescription, null, typeof(int));
            this.SetParam(3, false, true, false, Messages.P4CorrelatedSignal, Messages.P4CorrelatedSignalDescription, 0, typeof(int));
            this.SetParam(4, false, true, false, Messages.P5CorrelatedSignal, Messages.P5CorrelatedSignalDescription, null, typeof(Matrix));
            this.SetParam(5, false, true, false, Messages.P6CorrelatedSignal, Messages.P6CorrelatedSignalDescription, 0, typeof(int));
            this.SetParam(6, false, true, false, Messages.P7CorrelatedSignal, Messages.P7CorrelatedSignalDescription, 256, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector f = (Vector)arguments[0];
            Matrix delta1 = (Matrix)arguments[1];
            int t1 = (int)arguments[2];
            int t2 = (int)arguments[3];
            Matrix delta2 = (Matrix)arguments[4];
            int t3 = (int)arguments[5];
            int tbase = (int)arguments[6];

            int Nf = f.Length;
            int M = delta1.LengthX;

            Matrix result = new Matrix(M, t1 + t2 + t3);

            // Region 1
            if(t1 > 0) {
                if(guider != null)
                    guider.Write(Messages.MsgCorrelatedSignalRegion1Start);
                for(int i = 0; i < M; i++) {
                    if(guider != null)
                        guider.Write('.');
                    for(int j = 0; j < Nf; j++) {
                        double d = delta1[i, j];
                        double k = 2.0 * System.Math.PI * f[j] / tbase;

                        for(int t = 0; t < t1; t++)
                            result[i, t] += System.Math.Sin(k * t + d);
                    }
                }
                if(guider != null)
                    guider.WriteLine();
            }

            // Region 2
            if(t2 > 0) {
                if(guider != null)
                    guider.Write(Messages.MsgCorrelatedSignalRegion2Start);
                for(int i = 0; i < M; i++) {
                    if(guider != null)
                        guider.Write('.');
                    for(int j = 0; j < Nf; j++) {
                        double d1 = delta1[i, j];
                        double d2 = delta2[i, j];
                        double kd = (d2 - d1)/t2;
                        double k = 2.0 * System.Math.PI * f[j] / tbase;

                        for(int t = 0; t < t2; t++)
                            result[i, t + t1] += System.Math.Sin(k * (t + t1) + d1 + kd * t);
                    }
                }
                if(guider != null)
                    guider.WriteLine();
            }

            // Region 3
            if(t3 > 0) {
                if(guider != null)
                    guider.Write(Messages.MsgCorrelatedSignalRegion3Start);
                for(int i = 0; i < M; i++) {
                    if(guider != null)
                        guider.Write('.');
                    for(int j = 0; j < Nf; j++) {
                        double d = delta2[i, j];
                        double k = 2.0 * System.Math.PI * f[j] / tbase;

                        for(int t = 0; t < t3; t++)
                            result[i, t + t1 + t2] += System.Math.Sin(k * (t + t1 + t2) + d);
                    }
                }
                if(guider != null)
                    guider.WriteLine();
            }

            return result;
        }
    }
}
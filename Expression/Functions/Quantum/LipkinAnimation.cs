using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;
using PavelStransky.Core;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Makes a psychadelic animation of the Lipkin model
    /// </summary>
    public class LipkinAnimation : Fnc {
        public override string Help { get { return Messages.HelpLipkinAnimation; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.PSize, Messages.PSizeDescription, 0, typeof(int));
            this.SetParam(2, false, true, false, Messages.PType, Messages.PTypeDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector parameters = arguments[0] as PointVector;

            int length = parameters.Length;
            int size = (int)arguments[1];
            int type = (int)arguments[2];

            Vector max = new Vector(2);
            max[0] = size;
            max[1] = size;

            if(guider != null) {
                guider.Write("Animation=");
                guider.Write(length);
            }

            DateTime startTime = DateTime.Now;

            Matrix[] m = null;
            if (type == 0) {
                m = new Matrix[size+1];
                for (int i = 0; i <= size; i++)
                    m[i] = new Matrix(length, size + 1);
            }
            else {
                m = new Matrix[length];
                for (int i = 0; i < length; i++)
                    m[i] = new Matrix(size + 1); 
            }

            for (int i = 0; i < length; i++) {
                LipkinFull l = new LipkinFull(parameters[i].X, parameters[i].Y);
                l.EigenSystem.Diagonalize(max, true, 0, null, ComputeMethod.LAPACKBand);

                for (int j = 0; j <= size; j++) {
                    Vector ev = l.EigenSystem.GetEigenVector(j);
                    for (int k = 0; k <= size; k++) {
                        double d = ev[k]; d *= d;
                        if (type == 0)
                            m[j][i, k] = d;
                        else
                            m[i][j, k] = d;
                    }
                }

                if (guider != null)
                    guider.Write(".");

            }

            if (guider != null)
                guider.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));

            return new TArray(m);
        }
    }
}
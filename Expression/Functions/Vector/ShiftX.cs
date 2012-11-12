using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Shifts the X values of a PointVector so that the first auxiliary PointVector fits the second auxiliary PointVector
    /// </summary>
    public class ShiftX: Fnc {
        public override string Help { get { return Messages.HelpShiftX; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.PSource, Messages.PSourceDescription, null, typeof(PointVector));
            this.SetParam(2, true, true, false, Messages.PTarget, Messages.PTargetDescription, null, typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector vector = arguments[0] as PointVector;
            PointVector source = arguments[1] as PointVector;
            PointVector target = arguments[2] as PointVector;

            int length = vector.Length;
            
            int lengths = source.Length;
            int lengtht = target.Length;

            PointVector result = new PointVector(length);
            int r = 0;

            for(int i = 0; i < length; i++) {
                double x = vector[i].X;
                double y = vector[i].Y;

                int j = 0;
                while(j < lengths && source[j].Y > y)
                    j++;
                if(j == 0 || j == lengths)
                    continue;

                int k = 0;
                while(k < lengths && target[k].Y > y)
                    k++;
                if(k == 0 || k == lengtht)
                    continue;

                double xs = (source[j].X - source[j - 1].X) / (source[j].Y - source[j - 1].Y) * (y - source[j - 1].Y) + source[j - 1].X;
                double xt = (target[k].X - target[k - 1].X) / (target[k].Y - target[k - 1].Y) * (y - target[k - 1].Y) + target[k - 1].X;

                result[r] = new PointD(x + xt - xs, y);
                r++;
            }

            result.Length = r;
            return result;
        }
    }
}

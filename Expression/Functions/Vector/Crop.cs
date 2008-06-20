using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Removes values from a vector or pointvector that are larger or smaller than given bounds
    /// </summary>
    public class Crop: Fnc {
        public override string Help { get { return Messages.HelpCrop; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector), typeof(PointVector));
            this.SetParam(1, true, true, true, Messages.PXMinBound, Messages.PXMinBoundDescription, null, typeof(double));
            this.SetParam(2, true, true, true, Messages.PXMaxBound, Messages.PXMaxBoundDescription, null, typeof(double));
            this.SetParam(3, false, true, true, Messages.PYMinBound, Messages.PYMinBoundDescription, double.MinValue, typeof(double));
            this.SetParam(4, false, true, true, Messages.PYMinBound, Messages.PYMaxBoundDescription, double.MaxValue, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double minx = (double)arguments[1];
            double maxx = (double)arguments[2];
            double miny = (double)arguments[3];
            double maxy = (double)arguments[4];

            if(arguments[0] is Vector) {
                Vector v = (Vector)arguments[0];
                int length = v.Length;

                Vector result = new Vector(length);
                int j = 0;
                for(int i = 0; i < length; i++)
                    if(v[i] >= minx && v[i] <= maxx)
                        result[j++] = v[i];

                result.Length = j;

                return result;
            }
            else {
                PointVector pv = (PointVector)arguments[0];
                int length = pv.Length;

                PointVector result = new PointVector(length);

                int j = 0;
                for(int i = 0; i < length; i++)
                    if(pv[i].X >= minx && pv[i].X <= maxx && pv[i].Y >= miny && pv[i].Y <= maxy)
                        result[j++] = pv[i];

                result.Length = j;

                return result;
            }
        }
    }
}

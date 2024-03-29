using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Removes bad points (NaN, Infinity) from Vector or PointVector
    /// </summary>
    public class RemoveBadPoints: Fnc {
        public override string Help { get { return Messages.HelpRemoveBadPoints; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PObjectWithBadPoints, Messages.PObjectWithBadPointsDescription, null, typeof(Vector), typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector) {
                Vector v = item as Vector;

                int index = 0;
                int length = v.Length;

                Vector result = new Vector(length);

                for(int i = 0; i < length; i++)
                    if(!double.IsNaN(v[i]) && !double.IsInfinity(v[i]))
                        result[index++] = v[i];

                result.Length = index;

                return result;
            }

            else if(item is PointVector) {
                PointVector pv = item as PointVector;

                int index = 0;
                int length = pv.Length;

                PointVector result = new PointVector(length);

                for(int i = 0; i < length; i++) {
                    PointD p = pv[i];
                    if(!double.IsNaN(p.X) && !double.IsInfinity(p.X)
                        && !double.IsNaN(p.Y) && !double.IsInfinity(p.Y))
                        result[index++] = p;
                }

                result.Length = index;

                return result;
            }

            return null;
        }
    }
}
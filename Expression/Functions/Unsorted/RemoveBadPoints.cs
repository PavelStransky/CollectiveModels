using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Removes bad points (NaN, Infinity) from Vector or PointVector
    /// </summary>
    public class RemoveBadPoints: FunctionDefinition {
        public override string Help { get { return Messages.RemoveBadPointsHelp; } }

        protected override void CreateParameters() {
            this.NumParams(1);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector), typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector) {
                Vector v = arguments[0] as Vector;

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
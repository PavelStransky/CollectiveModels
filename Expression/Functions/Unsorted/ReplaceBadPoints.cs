using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Replaces bad points (NaN, Infinity) in Vector or PointVector by a given value
    /// </summary>
    public class ReplaceBadPoints: Fnc {
        public override string Help { get { return Messages.HelpReplaceBadPoints; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PObjectWithBadPoints, Messages.PObjectWithBadPointsDescription, null, typeof(Vector), typeof(PointVector), typeof(Matrix));
            this.SetParam(1, false, true, true, Messages.PValue, Messages.PValueDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];
            double d = (double)arguments[1];

            if(item is Vector) {
                Vector v = item as Vector;

                int length = v.Length;
                Vector result = v.Clone() as Vector;

                for(int i = 0; i < length; i++)
                    if(double.IsNaN(result[i]) || double.IsInfinity(result[i]))
                        result[i] = d;

                return result;
            }

            else if(item is PointVector) {
                PointVector pv = item as PointVector;

                int length = pv.Length;
                PointVector result = pv.Clone() as PointVector;

                for(int i = 0; i < length; i++) {
                    PointD p = pv[i];
                    if(double.IsNaN(p.X) || double.IsInfinity(p.X))
                        pv[i].X = d;
                    if(double.IsNaN(p.Y) || double.IsInfinity(p.Y))
                        pv[i].Y = d;
                }

                return result;
            }

            else if(item is Matrix) {
                Matrix m = item as Matrix;

                int lengthx = m.LengthX;
                int lengthy = m.LengthY;

                Matrix result = m.Clone() as Matrix;

                for(int i = 0; i < lengthx; i++)
                    for(int j = 0; j < lengthy; j++)
                        if(double.IsInfinity(result[i, j]) || double.IsNaN(result[i, j]))
                            result[i, j] = d;

                return result;
            }

            return null;
        }
    }
}
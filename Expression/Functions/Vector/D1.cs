using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the first derivative
    /// </summary>
    public class D1 : Fnc {
        public override string Help { get { return Messages.HelpD1; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, false, true, true, Messages.PStartingPoint, Messages.PStartingPointDetail, 0.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PStep, Messages.PStepDescription, 1.0, typeof(double));
            this.SetParam(3, false, true, false, Messages.POrder, Messages.POrderDetail, 2, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = arguments[0] as Vector;
            double x0 = (double)arguments[1];
            double step = (double)arguments[2];
            int order = (int)arguments[3];

            x0 += (order / 2) * step;

            int length = v.Length - order;

            PointVector result = new PointVector(length);
            for(int i = 0; i < length; i++) {
                result[i].X = x0 + i * step;
                switch(order) {
                    case 2:
                        result[i].Y = (-v[i] / 2.0 + v[i + 2] / 2.0) / step;
                        break;
                    case 4:
                        result[i].Y = (v[i] / 12.0 - 2.0 * v[i + 1] / 3.0 + 2.0 * v[i + 3] / 3.0 - v[i + 4] / 12.0) / step;
                        break;
                    case 6:
                        result[i].Y = (-v[i] / 60.0 + 3.0 * v[i + 1] / 20.0 - 3.0 * v[i + 2] / 4.0 + 3.0 * v[i + 4] / 4.0 - 3.0 * v[i + 5] / 20.0 + v[i + 6] / 60.0) / step;
                        break;
                    case 8:
                        result[i].Y = (v[i] / 280.0 - 4.0 * v[i + 1] / 105.0 + v[i + 2] / 5.0 - 4.0 * v[i + 3] / 5.0 + 4.0 * v[i + 5] / 5.0 - v[i + 6] / 5.0 + 4.0 * v[i + 7] / 105.0 - v[i + 8] / 280.0) / step;
                        break;
                    default:
                        throw new FncException(this, string.Format(Messages.EMBadDerivativeOrder, order));
                }                
            }

            return result;
        }
    }
}
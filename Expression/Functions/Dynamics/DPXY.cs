using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Transforms angles of DoublePendulum system to (X, Y) coordinates of each body
    /// </summary>
    public class DPXY: Fnc {
        public override string Help { get { return Messages.HelpDPXY; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PDoublePendulum, Messages.PDoublePendulumDescription, null, typeof(ClassicalDP));
            this.SetParam(1, true, true, false, Messages.PX, Messages.PXDetail, null, typeof(PointD), typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            ClassicalDP dp = (ClassicalDP)arguments[0];

            if(arguments[1] is PointD) {
                PointD p = (PointD)arguments[1];
                TArray result = new TArray(typeof(PointD), 2);
                result[0] = dp.Body1(p.X);
                result[1] = dp.Body2(p.X, p.Y);
                return result;
            }
            else if(arguments[1] is PointVector) {
                PointVector pv = (PointVector)arguments[1];
                int length = pv.Length;

                PointVector result1 = new PointVector(length);
                PointVector result2 = new PointVector(length);
                for(int i = 0; i < length; i++) {
                    result1[i] = dp.Body1(pv[i].X);
                    result2[i] = dp.Body2(pv[i].X, pv[i].Y);
                }

                TArray result = new TArray(typeof(PointVector), 2);
                result[0] = result1;
                result[1] = result2;
                return result;
            }

            return null;
        }
    }
}
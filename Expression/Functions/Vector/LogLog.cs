using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// From both x and y values of a pointvector calculates log_10
    /// </summary>
    public class LogLog: Fnc {
        public override string Help { get { return Messages.HelpLogLog; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, false, true, true, Messages.PScaleParameter, Messages.PScaleParameterDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector pv = arguments[0] as PointVector;
            double s = (double)arguments[1];

            int length = pv.Length;
            PointVector result = new PointVector(length);

            for(int i = 0; i < length; i++) 
                result[i] = new PointD(System.Math.Log10(pv[i].X * s), System.Math.Log10(pv[i].Y / s));

            return result;
        }
    }
}
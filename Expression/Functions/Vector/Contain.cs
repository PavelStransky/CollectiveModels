using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns true if the given vector contains the given value within given error
    /// </summary>
    public class Contain: Fnc {
        public override string Help { get { return Messages.HelpContain; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, true, true, true, Messages.PValue, Messages.PValueDescription, 0.0, typeof(double));
            this.SetParam(2, false, true, false, Messages.PError, Messages.PErrorDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = (Vector)arguments[0];
            double x = (double)arguments[1];
            double error = (double)arguments[2];
            int length = v.Length;

            for(int i = 0; i < length; i++)
                if(System.Math.Abs(v[i] - x) <= error)
                    return true;

            return false;
        }
    }
}

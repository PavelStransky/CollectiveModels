using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns true if the values are equal within given difference
    /// </summary>
    public class IsEqual: Fnc {
        public override string Help { get { return Messages.HelpIsEqual; } }

        protected override void CreateParameters() {
            this.SetNumParams(2, true);

            this.SetParam(0, false, true, true, Messages.PError, Messages.PErrorDescription, 0.0, typeof(double));
            this.SetParam(1, true, true, true, Messages.PValue, Messages.PValueDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double error = (double)arguments[0];

            int count = arguments.Count;

            // Nalezení maxima a minima
            double max = (double)arguments[1];
            double min = max;
            for(int i = 2; i < count; i++) {
                double d = (double)arguments[i];
                if(d > max)
                    max = d;
                if(d < min)
                    min = d;
            }

            for(int i = 2; i < count; i++) {
                double d = (double)arguments[i];
                if(System.Math.Abs(d - max) > error)
                    return false;
                if(System.Math.Abs(d - min) > error)
                    return false;
            }

            return true;
        }
    }
}

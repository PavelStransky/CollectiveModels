using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.PT;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns minima of a given function
    /// </summary>
    public class Minima: Fnc {
        public override string Help { get { return Messages.HelpMinima; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IMinMax));
            this.SetParam(1, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IMinMax minmax = (IMinMax)arguments[0];
            double precision = (double)arguments[1];

            return minmax.Minima(precision);
        }
    }
}
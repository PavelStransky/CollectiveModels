using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of the Pi number
    /// </summary>
    public class Pi: Fnc {
        public override string Help { get { return Messages.HelpPi; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return System.Math.PI;
        }
    }
}

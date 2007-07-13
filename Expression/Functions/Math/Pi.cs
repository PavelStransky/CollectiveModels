using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Value of the Pi number
    /// </summary>
    public class Pi: FunctionDefinition {
        public override string Help { get { return Messages.HelpPi; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return System.Math.PI;
        }
    }
}

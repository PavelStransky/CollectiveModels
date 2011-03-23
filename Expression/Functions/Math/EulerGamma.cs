using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value of the Euler-Mascheroni gamma constant
    /// </summary>
    public class EulerGamma: Fnc {
        public override string Help { get { return Messages.HelpEulerGamma; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return gamma;
        }

        private const double gamma = 0.5772156649015328606065120900824024310421;
    }
}

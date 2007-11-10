using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value with Wigner GUE distribution
    /// </summary>
    public class RandomGUE: Fnc {
        public override string Help { get { return Messages.HelpRandomGUE; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return RMTDistribution.GetGUE();
        }
    }
}

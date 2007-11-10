using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value with Wigner GSE distribution
    /// </summary>
    public class RandomGSE: Fnc {
        public override string Help { get { return Messages.HelpRandomGSE; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return RMTDistribution.GetGSE();
        }
    }
}

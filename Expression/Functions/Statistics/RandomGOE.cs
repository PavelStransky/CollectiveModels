using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Value with Wigner GOE distribution
    /// </summary>
    public class RandomGOE: Fnc {
        public override string Help { get { return Messages.HelpRandomGOE; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return RMTDistribution.GetGOE();
        }
    }
}

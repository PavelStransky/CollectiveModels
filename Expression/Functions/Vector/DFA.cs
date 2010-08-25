using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Detrended fluctuation analysis
    /// </summary>
    /// <remarks>PRE 49, 1685 (1994)</remarks>
    public class DFA: Fnc {
        public override string Help { get { return Messages.HelpDFA; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as Vector).DFA();
        }
    }
}

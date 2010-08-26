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
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, false, true, false, Messages.PAllPoints, Messages.PAllPointsDescription, false, typeof(bool)); 
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            bool allPoints = (bool)arguments[1];

            return (arguments[0] as Vector).DFA(allPoints);
        }
    }
}

using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates a partial trace for an eigenstate (returns it as a density matrix)
    /// </summary>
    public class PartialTrace: Fnc {
        public override string Help { get { return Messages.HelpPartialTrace; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PCombinedSystem, Messages.PCombinedSystemDescription, null, typeof(IEntanglement));
            this.SetParam(1, true, true, false, Messages.PEigenValueIndex, Messages.PEigenValueIndexDescription, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IEntanglement system = arguments[0] as IEntanglement;
            int n = (int)arguments[1];
            return system.PartialTrace(n);
        }
    }
}
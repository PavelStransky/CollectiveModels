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
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PCombinedSystem, Messages.PCombinedSystemDescription, null, typeof(LipkinOne), typeof(LipkinTwo));
            this.SetParam(1, true, true, false, Messages.PEigenValueIndex, Messages.PEigenValueIndexDescription, null, typeof(int));
            this.SetParam(2, false, true, false, Messages.PType, Messages.PTypeDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int n = (int)arguments[1];
            int type = (int)arguments[2];

            if(arguments[0] is LipkinOne)
                return ((LipkinOne)arguments[0]).PartialTrace(n, type);
            else
                return ((LipkinTwo)arguments[0]).PartialTrace(n, type);

        }
    }
}
using System;
using System.Collections;

using PavelStransky.Systems;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the Peres invariant of a quantum system
    /// </summary>
    public class PeresInvariant: Fnc {
        public override string Help { get { return Messages.HelpPeresInvariant; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, false, true, false, Messages.PPeresOperatorType, Messages.PPeresOperatorTypeDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int type = (int)arguments[1];
            return (arguments[0] as IQuantumSystem).PeresInvariant(type);
        }
    }
}

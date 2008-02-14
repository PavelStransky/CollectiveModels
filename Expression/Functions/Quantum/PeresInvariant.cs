using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the Peres invariant of a quantum system
    /// </summary>
    public class PeresInvariant: Fnc {
        public override string Help { get { return Messages.HelpPeresInvariant; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PLHOQuantumGCM, Messages.PLHOQuantumGCMDescription, typeof(LHOQuantumGCM));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as LHOQuantumGCM).GetPeresInvariant();
        }
    }
}

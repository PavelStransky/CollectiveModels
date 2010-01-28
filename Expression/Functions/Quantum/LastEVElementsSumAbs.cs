using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns absolute value of the sum of last elements of components of eigenvectors
    /// </summary>
    public class LastEVElementsSumAbs: Fnc {
        public override string Help { get { return Messages.HelpLastEVElementsSumAbs; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PLHOQuantumGCM, Messages.PLHOQuantumGCMDescription, typeof(LHOQuantumGCM));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as LHOQuantumGCM).LastEVElementsSumAbs();
        }
    }
}

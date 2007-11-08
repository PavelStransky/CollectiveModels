using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the second invariant of a quantum system
    /// </summary>
    public class SecondInvariant: Fnc {
        public override string Help { get { return Messages.HelpSecondInvariant; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PLHOQuantumGCM, Messages.PLHOQuantumGCMDescription, typeof(LHOQuantumGCMIR), typeof(LHOQuantumGCMAR));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item as LHOQuantumGCMAR != null)
                return (item as LHOQuantumGCMAR).GetSecondInvariant();

            else if(item as LHOQuantumGCMIR != null)
                return (item as LHOQuantumGCMIR).GetSecondInvariant();

            else
                return null;
        }
    }
}

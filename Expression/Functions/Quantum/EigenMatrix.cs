using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns a matrix of components of eigenvectors arranged in matrix by indexes
    /// </summary>
    public class EigenMatrix: Fnc {
        public override string Help { get { return Messages.HelpEigenMatrix; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PLHOQuantumGCM, Messages.PLHOQuantumGCMDescription, null, typeof(LHOQuantumGCM), typeof(QuantumDicke));
            this.SetParam(1, true, true, false, Messages.PEVIndex, Messages.PEVIndexDescription, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int n = (int)arguments[1];

            if(arguments[0] is LHOQuantumGCM)
                return (arguments[0] as LHOQuantumGCM).EigenMatrix(n);
            else if(arguments[0] is QuantumDicke)
                return (arguments[0] as QuantumDicke).EigenMatrix(n);

            return null;
        }
    }
}

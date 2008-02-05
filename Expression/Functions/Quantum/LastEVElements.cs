using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns last elements of components of eigenvectors; all quantum numbers are taken into account
    /// </summary>
    public class LastEVElements: Fnc {
        public override string Help { get { return Messages.HelpLastEVElements; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PLHOQuantumGCM, Messages.PLHOQuantumGCMDescription, null, typeof(LHOQuantumGCM));
            this.SetParam(1, true, true, false, Messages.PEVIndex, Messages.PEVIndexDescription, null, typeof(int));
            this.SetParam(2, false, true, false, Messages.P3LastEVElements, Messages.P3LastEVElementsDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LHOQuantumGCM q = arguments[0] as LHOQuantumGCM;
            int n = (int)arguments[1];
            int index = (int)arguments[2];

            return q.LastEVElements(n, index);
        }
    }
}

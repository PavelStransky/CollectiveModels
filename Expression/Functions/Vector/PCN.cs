using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Number of principal components of eigenvector
    /// </summary>
    /// <remarks>PRE 71, 046116 (2005)</remarks>
    public class PCN: Fnc {
        public override string Help { get { return Messages.HelpPCN; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as Vector).NumPrincipalComponents();
        }
    }
}

using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates classical ExtensiblePendulum class
    /// </summary>
    public class CEP: Fnc {
        public override string Help { get { return Messages.HelpClassicalEP; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, false, true, true, Messages.PExtensiblePendulumNu, Messages.PExtensiblePendulumNuDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double nu = (double)arguments[0];
            return new ClassicalEP(nu);
        }
    }
}
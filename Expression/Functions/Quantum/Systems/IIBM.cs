using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates the intrinsic IBM class
    /// </summary>
    public class IIBM : Fnc {
        public override string Help { get { return Messages.HelpIIBM; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, false, true, true, Messages.PBeta0, Messages.PBeta0Description, System.Math.Sqrt(2), typeof(double));
            this.SetParam(1, false, true, true, Messages.PRho, Messages.PRhoDescription, 1.0 / System.Math.Sqrt(2), typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double beta0 = (double)arguments[0];
            double rho = (double)arguments[1];

            return new IntrinsicIBM(beta0, rho);
        }
    }
}
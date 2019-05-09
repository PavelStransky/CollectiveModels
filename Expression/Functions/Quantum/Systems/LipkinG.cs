using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a class of the Lipkin model with all possible two-body terms
    /// </summary>
    public class LipkinG : Fnc {
        public override string Help { get { return Messages.HelpLipkinG; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);
            this.SetParam(0, false, true, true, Messages.PJx, Messages.PJxDescription, 0.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PJz, Messages.PJzDescription, 0.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PJx2, Messages.PJx2Description, 0.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PJz2, Messages.PJz2Description, 0.0, typeof(double));
            this.SetParam(4, false, true, true, Messages.PJxz, Messages.PJxzDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double jx = (double)arguments[0];
            double jz = (double)arguments[1];

            double jx2 = (double)arguments[2];
            double jz2 = (double)arguments[3];
            double jxz = (double)arguments[4];

            return new LipkinGeneral(jx, jz, jx2, jz2, jxz);
        }
    }
}
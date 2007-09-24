using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Trace of a given matrix
    /// </summary>
    public class Trace : Fnc {
        public override string Help { get { return Messages.HelpTrace; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as Matrix).Trace();
        }
    }
}

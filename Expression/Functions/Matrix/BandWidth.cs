using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Size of the band of a band matrix
    /// </summary>
    public class BandWidth : FunctionDefinition {
        public override string Help { get { return Messages.HelpBandWidth; } }

        protected override void CreateParameters() {
            this.NumParams(1);
            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as Matrix).BandWidth();
        }
    }
}

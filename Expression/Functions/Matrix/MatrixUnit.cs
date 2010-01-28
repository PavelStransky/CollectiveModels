using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a unit matrix
    /// </summary>
    public class MatrixUnit: Fnc {
        public override string Help { get { return Messages.HelpMatrixUnit; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PMatrixSize, Messages.PMatrixSizeDescription, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return Matrix.Unit((int)arguments[0]);
       }
    }
}
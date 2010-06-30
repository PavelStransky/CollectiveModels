using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the volume of a simplex given by the vectors of the matrix
    /// </summary>
    public class SimplexVolume: Fnc {
        public override string Help { get { return Messages.HelpSimplexVolume; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = arguments[0] as Matrix;
            return m.Determinant() / Math.SpecialFunctions.FactorialI(m.Length);
        }
    }
}
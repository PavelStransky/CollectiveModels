using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Parent for all sum functions
    /// </summary>
    public class FncSum: Fnc {
        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PMultiDimensions, Messages.PMultiDimensionsDescription, null,
                typeof(Vector), typeof(double), typeof(int), typeof(Matrix));
        }
    }
}
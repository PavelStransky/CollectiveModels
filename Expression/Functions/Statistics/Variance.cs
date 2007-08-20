using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Calculates variance of a vector
    /// </summary>
    public class Variance: FunctionDefinition {
        public override string Help { get { return Messages.HelpVariance; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector item = arguments[0] as Vector;
            return item.Variance();
        }
    }
}

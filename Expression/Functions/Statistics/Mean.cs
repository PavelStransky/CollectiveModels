using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Calculates mean of a vector
	/// </summary>
    public class Mean: FunctionDefinition {
        public override string Help { get { return Messages.HelpMean; } }

        protected override void CreateParameters() {
            this.NumParams(1);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector item = arguments[0] as Vector;
            return item.Mean();
        }
    }
}

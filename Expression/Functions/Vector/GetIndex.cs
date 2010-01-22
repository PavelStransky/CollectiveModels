using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns indices of items which are in given relation with given number
    /// </summary>
    public class GetIndex: Fnc {
        public override string Help { get { return Messages.HelpGetIndex; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, true, true, true, Messages.PValue, Messages.PValueDescription, 0.0, typeof(double));
            this.SetParam(2, false, true, false, Messages.PComparisonOperator, Messages.PComparisonOperatorDescription, "==", typeof(string));
        }	

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector item = (Vector)arguments[0];
            double x = (double)arguments[1];
            ComparisonOperator o = new ComparisonOperator((string)arguments[2]);

            return new TArray(item.GetIndex(x, o));
        }
    }
}

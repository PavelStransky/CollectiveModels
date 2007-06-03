using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Polynomial regression of data
	/// </summary>
	public class FnRegression: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return Messages.RegressionHelp;}}

        protected override void CreateParameters() {
            this.NumParams(2);

            this.SetParam(0, true, true, false, Messages.RegressionP1, Messages.RegressionP1Description, null, typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.POrder, Messages.POrderDetail, null, typeof(int));
        }

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			if(arguments[0] is PointVector) {
				PointVector pointVector = arguments[0] as PointVector;
				int order = (int)arguments[1];
				return Regression.Compute(pointVector, order);
			}

            return null;
		}

		private const string name = "regression";
	}
}
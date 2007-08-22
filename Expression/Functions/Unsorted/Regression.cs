using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Polynomial regression of data
	/// </summary>
	public class FnRegression: Fnc {
		public override string Name {get {return name;}}
		public override string Help {get {return Messages.RegressionHelp;}}

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.P1Regression, Messages.P1RegressionDescription, null, typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.POrder, Messages.POrderDetail, null, typeof(int));
        }

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			PointVector pointVector = arguments[0] as PointVector;
			int order = (int)arguments[1];
			return Regression.Compute(pointVector, order);
		}

		private const string name = "regression";
	}
}
using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Calculates an integral under given curve
	/// </summary>
	public class Integrate: FunctionDefinition {
		public override string Help {get {return Messages.HelpIntegrate;}}

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.P1Integrate, Messages.P1IntegrateDescription, null, typeof(PointVector));
        }

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			return (arguments[0] as PointVector).Integrate();
		}
	}
}

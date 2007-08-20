using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Norm of the vector
	/// </summary>
	public class Norm: FunctionDefinition {
		public override string Help {get {return Messages.HelpNorm;}}

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.P1Norm, Messages.P1NormDescription, null, typeof(Vector));
            this.SetParam(1, false, true, true, Messages.P2Norm, Messages.P2NormDescription, 2.0, typeof(double));
        }	

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as Vector).Norm((double)arguments[1]);
		}
	}
}

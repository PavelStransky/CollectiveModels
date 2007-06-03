using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Norm of the vector
	/// </summary>
	public class Norm: FunctionDefinition {
		public override string Help {get {return Messages.NormHelp;}}

        protected override void CreateParameters() {
            this.NumParams(2);

            this.SetParam(0, true, true, false, Messages.NormP1, Messages.NormP1Description, null, typeof(Vector));
            this.SetParam(1, false, true, true, Messages.NormP2, Messages.NormP2Description, 2.0, typeof(double));
        }	

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as Vector).Norm((double)arguments[1]);
		}
	}
}

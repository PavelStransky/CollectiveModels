using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Creates a point from two given numbers
	/// </summary>
	public class FnPoint: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return Messages.HelpPoint;}}

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, true, Messages.PX, Messages.PXDetail, null, typeof(double));
            this.SetParam(1, true, true, true, Messages.PY, Messages.PYDescription, null, typeof(double));
        }

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			return new PointD((double)arguments[0], (double)arguments[1]);
		}

		private const string name = "point";
	}
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z bodu nebo vektoru bodù vybere souøadnice X
	/// </summary>
	public class GetX: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string ParametersHelp {get {return parameters;}}

		protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(PointVector), typeof(PointD));
		}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments[0] is PointVector)
                return (arguments[0] as PointVector).VectorX;
            else 
                return (arguments[0] as PointD).X;
		}

		private const string help = "Z bodu nebo vektoru bodù vybere souøadnice X";
		private const string parameters = "PointD | PointVector";
	}
}

using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí kumulovaný histogram vektoru
	/// </summary>
	public class CumulHistogram: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string ParametersHelp {get {return parameters;}}

		protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 2);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 4);

            this.ConvertInt2Double(evaluatedArguments, 2);
            this.ConvertInt2Double(evaluatedArguments, 3);

            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(Vector));
            this.CheckArgumentsType(evaluatedArguments, 1, evaluateArray, typeof(int));
            this.CheckArgumentsType(evaluatedArguments, 2, evaluateArray, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 3, evaluateArray, typeof(double));
		}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = (Vector)arguments[0];

            double min = (arguments.Count > 2) ? (double)arguments[2] : v.Min();
            double max = (arguments.Count > 3) ? (double)arguments[3] : v.Max();

		    return v.CumulativeHistogram((int)arguments[1], min, max);
		}

		private const string help = "Vytvoøí kumulovaný histogram vektoru pro zadaný poèet intervalù";
		private const string parameters = "Vector; poèet intervalù (int) [;minimální hodnota (double) [;maximální hodnota (double)]]";
	}
}

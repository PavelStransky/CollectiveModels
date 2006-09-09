using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí histogram vektoru
	/// </summary>
	public class Histogram: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 2);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 4);
			this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));

			if(evaluatedArguments.Count > 2) {
				if(evaluatedArguments[2] is int)
					evaluatedArguments[2] = (double)(int)evaluatedArguments[2];
				this.CheckArgumentsType(evaluatedArguments, 2, typeof(double));
			}
			if(evaluatedArguments.Count > 3) {
				if(evaluatedArguments[3] is int)
					evaluatedArguments[3] = (double)(int)evaluatedArguments[3];
				this.CheckArgumentsType(evaluatedArguments, 3, typeof(double));
			}

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Vector) {
				Vector vector = item as Vector;

				double min = vector.Min();
				double max = vector.Max();
				if(arguments.Count > 2)
					min = (double)arguments[2];
				if(arguments.Count > 3)
					max = (double)arguments[3];

				return vector.Histogram((int)arguments[1], min, max);
			}
			else if(item is Array) 
				return this.EvaluateArray(depth, item as Array, arguments);
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Vytvoøí histogram vektoru pro zadaný poèet intervalù";
		private const string parameters = "Vector; poèet intervalù (int) [;minimální hodnota (double) [;maximální hodnota (double)]]";
	}
}

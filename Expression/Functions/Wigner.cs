using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vypoèítá Wignerovu funkci v danem bode
	/// </summary>
	public class Wigner: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is int)
				return this.WignerFunction((int)item);
			if(item is double)
				return this.WignerFunction((double)item);
			else if(item is Vector)
				return (item as Vector).Transform(new RealFunction(this.WignerFunction));
			else if(item is Matrix)
				return (item as Matrix).Transform(new RealFunction(this.WignerFunction));
			else if(item is TArray) 
				return this.EvaluateArray(depth, item as TArray, arguments);
			else
				return this.BadTypeError(item, 0);			
		}

		private double WignerFunction(double x) {
			return System.Math.PI / 2.0 * x * System.Math.Exp(-System.Math.PI / 4.0 * x * x);
		}

		private const string help = "Vypoèítá Wignerovu distribuèní funkci v daném bodì";
		private const string parameters = "int | double | Vector | Matrix";
	}
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vypoèítá Hodnotu Wignerova rozdìlení v daném bodì
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
				return SpecialFunctions.Wigner((int)item);
			if(item is double)
                return SpecialFunctions.Wigner((double)item);
			else if(item is Vector)
                return (item as Vector).Transform(SpecialFunctions.Wigner);
			else if(item is Matrix)
                return (item as Matrix).Transform(SpecialFunctions.Wigner);
			else if(item is TArray) 
				return this.EvaluateArray(depth, item as TArray, arguments);
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Vypoèítá hodnotu Wignerovy distribuèní funkce v daném bodì";
		private const string parameters = "int | double | Vector | Matrix";
	}
}

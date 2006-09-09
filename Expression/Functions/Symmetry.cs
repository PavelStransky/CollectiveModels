using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vypoèítá symmetry parameter vektoru
	/// </summary>
	public class Symmetry: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}
	
		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Vector)
				return (item as Vector).Symmetry();
			else if(item is Array) 
				return this.EvaluateArray(depth, item as Array, arguments);
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Vrací parametr symetrie vektoru";
		private const string parameters = "Vector";
	}
}

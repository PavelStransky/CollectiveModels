using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vypo��t� normu vektoru
	/// </summary>
	public class Norm: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}
	
		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

			// Druh� argument ur�uje, kolik�t� mocniny s��t�me (standardn� druh�)
			if(evaluatedArguments.Count > 1)
				this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
			else
				evaluatedArguments.Add(2);

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Vector)
				return (item as Vector).Norm((double)(int)arguments[1]);
			else if(item is Array) 
				return this.EvaluateArray(depth, item as Array, arguments);
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Vrac� normu vektoru";
		private const string parameters = "Vector [;mocnina slo�ek vektor�]";
	}
}

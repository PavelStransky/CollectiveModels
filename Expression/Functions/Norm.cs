using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vypoèítá normu vektoru
	/// </summary>
	public class Norm: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}
	
		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

			// Druhý argument urèuje, kolikáté mocniny sèítáme (standardnì druhé)
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

		private const string help = "Vrací normu vektoru";
		private const string parameters = "Vector [;mocnina složek vektorù]";
	}
}

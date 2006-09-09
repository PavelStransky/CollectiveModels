using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vypoèítá mocninu prvkù
	/// </summary>
	public class Power: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Use {get {return use;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 2);
			if(evaluatedArguments[1] is int)
				evaluatedArguments[1] = (double)(int)evaluatedArguments[1];
			this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			double power = (double)arguments[1];

			if(item is Vector) {
				Vector v = item as Vector;
				Vector result = new Vector(v.Length);
				for(int i = 0; i < result.Length; i++)
					result[i] = System.Math.Pow(v[i], power);
				return result;
			}
			else if(item is double) 
				return System.Math.Pow((double)item, power);
			else if(item is int)
				return System.Math.Pow((int)item, power);
			else if(item is Array) 
				return this.EvaluateArray(depth, item as Array, arguments);
			else
				return this.BadTypeError(item, 0);			
		}

		private const string help = "Vypoèítá mocninu prvkù";
		private const string use = "power(cokoliv, mocnina);";	
	}
}

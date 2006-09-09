using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z øady v prvním argumentu vybere prvek o indexu z druhého argumentu,
	/// pøípadnì jde do hloubky podle tøetího argumentu
	/// </summary>
	public class Item: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Use {get {return use;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 2);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

			if(!(evaluatedArguments[1] is int) && 
				!(evaluatedArguments[1] is PavelStransky.Expression.Array && (evaluatedArguments[1] as PavelStransky.Expression.Array).Count == 2))
				this.BadTypeError(evaluatedArguments[1], 1);

			// Pøidáme požadovanou hloubku
			if(evaluatedArguments.Count > 2) 
				this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));
			else
				evaluatedArguments.Add(0);

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			object i = arguments[1];
			int requiredDepth = (int)(arguments[2]);

			if(depth < requiredDepth) {
				if(item is PavelStransky.Expression.Array)
					return this.EvaluateArray(depth, item as PavelStransky.Expression.Array, arguments);
				else
					return this.BadTypeError(item, 0);
			}
			else {
				if(item is PavelStransky.Math.Vector) 
					return (item as PavelStransky.Math.Vector)[(int)i];
				else if(item is PavelStransky.Expression.Array)
					return (item as PavelStransky.Expression.Array)[(int)i];
				else if(item is Matrix)
					return (item as Matrix)[(i as int [])[0], (i as int [])[1]];
				else
					return this.BadTypeError(item, 0);
			}
		}

		private const string help = "Z øady v prvním argumentu vybere prvek o indexu z druhého argumentu, pøípadnì jde do hloubky podle tøetího argumentu";
		private const string use = "item(Vector | Array | Matrix, index [,hloubka])";
	}
}

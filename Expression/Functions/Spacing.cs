using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Pro vektor vypoèítá spacing - v_{i+j} - v_{i}
	/// </summary>
	public class Spacing: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

			if(evaluatedArguments.Count == 2)
				this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
			else
				evaluatedArguments.Add(defaultSpacing);

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Vector) {
				int spacing = (int)arguments[1];
				Vector v = item as Vector;
				Vector result = new Vector(v.Length - spacing);
				
				for(int i = spacing; i < v.Length; i++)
					result[i - spacing] = v[i] - v[i - spacing];

				return result;
			}
			else if(item is Array)
				return this.EvaluateArray(depth, item as Array, arguments);
			else
				return this.BadTypeError(item, 0);
		}

		private const int defaultSpacing = 1;

		private const string help = "Pro vektor vypoèítá rozdíly sousedních prvkù v[i + j] - v[i]";
		private const string parameters = "Vector [; diference j (int)]";
	}
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Ze dvou èísel funkce vytvoøí bod
	/// </summary>
	public class FnPoint: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}
		
		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 2);
			if(evaluatedArguments[0] is int)
				evaluatedArguments[0] = (double)(int)evaluatedArguments[0];
			if(evaluatedArguments[1] is int)
				evaluatedArguments[1] = (double)(int)evaluatedArguments[1];
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(double));
			this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			return new PointD((double)arguments[0], (double)arguments[1]);
		}

		private const string name = "point";
		private const string help = "Vytvoøí bod (PointD)";
		private const string parameters = "x (double); y (double)";
	}
}

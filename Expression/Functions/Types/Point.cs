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
		
		protected override void CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 2);

            this.ConvertInt2Double(evaluatedArguments, 0);
            this.ConvertInt2Double(evaluatedArguments, 1);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(double));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(double));
		}

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			return new PointD((double)arguments[0], (double)arguments[1]);
		}

		private const string name = "point";
		private const string help = "Vytvoøí bod (PointD)";
		private const string parameters = "x (double); y (double)";
	}
}

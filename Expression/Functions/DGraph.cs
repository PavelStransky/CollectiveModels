using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí graf hustot
	/// </summary>
	/// <param name="args">Argumenty funkce
	/// 0 ... data k vykreslení (matice)
	/// 1 ... parametry (string)
	/// 2 ... popisky X (array of string)
	/// 3 ... popisky Y (array of string)
	/// </param>
	public class DGraph: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 4);
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(Matrix));
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			string graphParams = null;
			if(arguments.Count > 1 && arguments[1] != null) {
				this.CheckArgumentsType(arguments, 1, typeof(string));
				graphParams = arguments[1] as string;
			}

			Array labelsX = null;
			if(arguments.Count > 3 && arguments[2] != null) {
				this.CheckArgumentsType(arguments, 2, typeof(Array));
				if((arguments[2] as Array).ItemType == typeof(string))
					labelsX = arguments[2] as Array;
				else
					this.BadTypeError(arguments[2], 2);
			}

			Array labelsY = null;
			if(arguments.Count > 3 && arguments[3] != null) {
				this.CheckArgumentsType(arguments, 3, typeof(Array));
				if((arguments[3] as Array).ItemType == typeof(string))
					labelsY = arguments[3] as Array;
				else
					this.BadTypeError(arguments[3], 3);
			}

			// Matice - graf hustot
			return new DensityGraph(item, graphParams, labelsX, labelsY);
		}

		private const string help = "Vytvoøí graf hustot";
		private const string parameters = "data k vykreslení (Matrix) [;parametry (string) [;popisky X (Array of string) [;popisky Y (Array of string)]]]";
	}
}

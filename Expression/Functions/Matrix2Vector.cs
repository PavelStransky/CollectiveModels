using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z prvkù matice udìlá vektor
	/// </summary>
	public class Matrix2Vector: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Matrix) {
				Matrix m = item as Matrix;
				Vector result = new Vector(m.LengthX * m.LengthY);
				int index = 0;
				for(int i = 0; i < m.LengthX; i++)
					for(int j = 0; j < m.LengthY; j++)
						result[index++] = m[i, j];
				return result;
			}
			else if(item is Array)
				return this.EvaluateArray(depth, item as Array, arguments);
			else
				return this.BadTypeError(item, 0);
		}

		private const string name = "m2v";
		private const string help = "Z prvkù matice udìlá vektor";
		private const string parameters = "Matrix";
	}
}

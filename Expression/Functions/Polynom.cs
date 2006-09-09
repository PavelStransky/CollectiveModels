using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí polynomiální regresi
	/// </summary>
	public class FnPolynom: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 2);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(arguments[1] is Vector) {
				Vector koefs = arguments[1] as Vector;

				if(item is int)
					return Polynom.GetValue(koefs, (int)item);
				else if(item is double) 
					return Polynom.GetValue(koefs, (double)item);
				else if(item is Vector) {
					Vector v = item as Vector;

					Vector result = new Vector(v.Length);
					for(int i = 0; i < result.Length; i++)
						result[i] = Polynom.GetValue(koefs, v[i]);

					return result;
				}
				else if(item is Matrix) {
					Matrix m = item as Matrix;

					Matrix result = new Matrix(m.LengthX, m.LengthY);
					for(int i = 0; i < result.LengthX; i++)
						for(int j = 0; j < result.LengthY; j++)
							result[i, j] = Polynom.GetValue(koefs, m[i, j]);

					return result;
				}
				else
					return this.BadTypeError(item, 0);
			}
			else if(arguments[1] is Array) {
				if(item is Array) 
					return this.EvaluateArray(depth, item as Array, arguments[1] as Array, arguments);
				else
					return this.BadTypeError(item, 0);
			}
			else
				return this.BadTypeError(arguments[1], 1);			
		}

		private const string name = "polynom";
		private const string help = "Vypoèítá hodnoty polynomu v daných bodech";
		private const string parameters = "int | double | Vector | Matrix; koeficienty polynomu (Vector)";
	}
}
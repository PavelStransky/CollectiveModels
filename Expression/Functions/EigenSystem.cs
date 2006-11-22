using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vypoèítá systém vlastních hodnot a vektorù
	/// </summary>
	public class EigenSystem: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);			
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			if(item is Matrix) {
				Jacobi jacobi = new Jacobi(item as Matrix);
				jacobi.SortAsc();
				Array result = new Array();

				result.Add(new Vector(jacobi.EigenValue));
				for(int i = 0; i < jacobi.EigenVector.Length; i++)
					result.Add(jacobi.EigenVector[i]);

				return result;
			}

            else if(item is IQuantumSystem) {
                IQuantumSystem qs = item as IQuantumSystem;
                Array result = new Array();

                result.Add(new Vector(qs.EigenValue));
                for(int i = 0; i < qs.EigenVector.Length; i++)
                    result.Add(qs.EigenVector[i]);

                return result;
            }

            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);
		}

		private const string help = "Vypoèítá systém vlastních hodnot a vektorù, vrací øadu vektorù, jejíž první prvek obsahuje vlastní hodnoty, zbytek prvkù vlastní vektory";
		private const string parameters = "Matrix | QuantumSystem";
	}
}

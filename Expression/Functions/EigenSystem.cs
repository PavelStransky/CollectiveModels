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
				TArray result = new TArray();

				result.Add(new Vector(jacobi.EigenValue));
				for(int i = 0; i < jacobi.EigenVector.Length; i++)
					result.Add(jacobi.EigenVector[i]);

				return result;
			}

            else if(item is IQuantumSystem) {
                IQuantumSystem qs = item as IQuantumSystem;
                TArray result = new TArray();

                result.Add(new Vector(qs.EigenValue));
                for(int i = 0; i < qs.EigenValue.Length; i++)
                    if(qs.EigenVector == null)
                        result.Add(qs.EigenVector[i]);
                    else
                        result.Add(new Vector(qs.EigenValue));

                return result;
            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);
            else
                return this.BadTypeError(item, 0);
		}

		private const string help = "Vypoèítá systém vlastních hodnot a vektorù, vrací øadu vektorù, jejíž první prvek obsahuje vlastní hodnoty, zbytek prvkù vlastní vektory";
		private const string parameters = "Matrix | QuantumSystem";
	}
}

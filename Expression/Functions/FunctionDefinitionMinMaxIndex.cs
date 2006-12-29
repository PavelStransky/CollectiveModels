using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Definice funkce, která hledá index nejmenší / nejvìtší hodnoty
	/// </summary>
	public abstract class FunctionDefinitionMinMaxIndex: FunctionDefinitionMinMax {
		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			int pdepth = (int)arguments[1];
			if(depth < pdepth) {
				if(item is TArray)
					return this.EvaluateArray(depth, item as TArray, arguments);
				else
					return this.BadTypeError(item, 0);
			}
			else {
				object result = this.EvaluateGoodDepth(depth, item, arguments);
				if(result is double) 
					return 0;
				else 
					return (result as object [])[0];
			}
		}

		/// <summary>
		/// Vypoèítá zadanou funkci pro každý prvek øady
		/// </summary>
		/// <param name="depth">Aktuální hloubka výpoètu</param>
		/// <param name="array">Vstupní øada</param>
		/// <param name="arguments">Parametry funkce</param>
		protected TArray EvaluateGoodDepthArray(int depth, TArray array, ArrayList arguments) {
			TArray result = new TArray();
			depth++;

			for(int i = 0; i < array.Count; i++) {
				object o = this.EvaluateGoodDepth(depth, array[i], arguments);
				if(o is double) 
					result.Add(this.FullResult(new TArray(), o));
				else
					result.Add(o);
			}

			return result;
		}

		/// <summary>
		/// Vrátí plný výsledek (Hodnotu i index)
		/// </summary>
		/// <param name="array">Øada s indexy</param>
		/// <param name="o">Hodnota</param>
		protected object [] FullResult(TArray array, object o) {
			object [] result = new object[2];
			result[0] = array;
			result[1] = o;
			return result;
		}
	}
}

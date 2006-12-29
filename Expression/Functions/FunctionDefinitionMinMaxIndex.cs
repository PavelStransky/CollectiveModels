using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Definice funkce, kter� hled� index nejmen�� / nejv�t�� hodnoty
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
		/// Vypo��t� zadanou funkci pro ka�d� prvek �ady
		/// </summary>
		/// <param name="depth">Aktu�ln� hloubka v�po�tu</param>
		/// <param name="array">Vstupn� �ada</param>
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
		/// Vr�t� pln� v�sledek (Hodnotu i index)
		/// </summary>
		/// <param name="array">�ada s indexy</param>
		/// <param name="o">Hodnota</param>
		protected object [] FullResult(TArray array, object o) {
			object [] result = new object[2];
			result[0] = array;
			result[1] = o;
			return result;
		}
	}
}

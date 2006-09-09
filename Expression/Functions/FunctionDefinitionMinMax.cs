using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Definice funkce, kter� hled� nejmen�� / nejv�t�� hodnotu
	/// </summary>
	public abstract class FunctionDefinitionMinMax: FunctionDefinition {
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

			if(evaluatedArguments.Count == 2)
				this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
			else {
				evaluatedArguments.Add(0);
			}

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			int pdepth = (int)arguments[1];
			if(depth < pdepth) {
				if(item is Array)
					return this.EvaluateArray(depth, item as Array, arguments);
				else
					return this.BadTypeError(item, 0);
			}
			else 
				return this.EvaluateGoodDepth(depth, item, arguments);
		}

		/// <summary>
		/// Po��t� u� jen maximum (jsme v po�adovan� hloubce)
		/// </summary>
		protected abstract object EvaluateGoodDepth(int depth, object item, ArrayList arguments);

		/// <summary>
		/// Zkontroluje, zda �ada vr�cen� z p�edchoz� �rovn� obsahuje n�jak� data
		/// </summary>
		/// <param name="result">Vr�cen� data</param>
		/// <param name="depth">Hloubka</param>
		protected void CheckResultLength(ArrayList result, int depth) {
			if(result.Count == 0)
				throw new FunctionDefinitionException(string.Format(errorMessageNoData, this.Name),
					string.Format(errorMessageNoDataDetail, depth)); 
		}

		private const string errorMessageNoData = "Funkci '{0}' nelze pou��t, proto�e d�lka �ady je nulov�.";
		private const string errorMessageNoDataDetail = "Hloubka: {0}";

		private const string parameters = "int | double |Vector | Matrix [; hloubka (int implicitn� 0)]";
	}
}

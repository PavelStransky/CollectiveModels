using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Definice funkce, kter� prov�d� import / export
	/// </summary>
	public class FncIE: Fnc {
		/// <summary>
		/// True, pokud �ten� / z�pis se bude prov�d�t bin�rn�
		/// </summary>
		/// <param name="arguments">Argumenty funkce</param>
		/// <param name="index">Index, na kter�m m� b�t hodnota o typu Binary</param>
		protected bool Binary(ArrayList arguments, int index) {
			bool binary = defaultBinary;

			if(arguments.Count > index) {
				if(arguments[index] as string == paramBinary)
					binary = true;
				else if(arguments[index] as string == paramText)
					binary = false;
				else
					throw new FncException(
                        this,
                        string.Format(errorMessageBadParameter, arguments[index] as string, index, this.Name));
			}

			return binary;
		}

		public override object Evaluate(Guider guider, ArrayList arguments) {
			ArrayList evaluatedArguments = this.EvaluateArguments(guider, arguments);
			this.CheckArguments(evaluatedArguments, guider.ArrayEvaluation);
			this.AddPath(guider.Context, evaluatedArguments);

            if(guider.ArrayEvaluation)
                return this.EvaluateArray(guider, evaluatedArguments);
            else
                return this.EvaluateFn(guider, evaluatedArguments);
        }

        /// <summary>
        /// Nastav� cestu (z kontextu vyt�hne p��slu�n� adres��)
        /// </summary>
        /// <param name="context">Kontext</param>
        /// <param name="evaluatedArguments">Argumenty</param>
        private void AddPath(Context context, ArrayList evaluatedArguments) {
            evaluatedArguments[0] = this.AddPath(context, evaluatedArguments[0] as string);
        }

		private const bool defaultBinary = true;
		private const string paramBinary = "binary";
		private const string paramText = "text";

		private const string errorMessageBadParameter = "Nezn�m� hodnota '{0}' {1}. parametru ve funkci '{2}'.";
	}
}
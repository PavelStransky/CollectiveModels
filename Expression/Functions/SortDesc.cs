using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Set��d� �adu / vektor sestupn�
	/// </summary>
	public class SortDesc: Sort {
		public override string Help {get {return help;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

			// Intern� d�me do druh�ho argumentu true pro ASC, false pro DESC
			if(evaluatedArguments.Count == 1)
				evaluatedArguments.Add(false);
			else {
				// a p��padn� druh� argument p�esuneme na t�et� m�sto
				evaluatedArguments.Add(evaluatedArguments[1]);
				evaluatedArguments[1] = false;
			}

			return evaluatedArguments;
		}

		private const string help = "Set��d� vektor nebo �adu sestupn�";
	}
}

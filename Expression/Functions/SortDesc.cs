using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Setøídí øadu / vektor sestupnì
	/// </summary>
	public class SortDesc: Sort {
		public override string Help {get {return help;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

			// Internì dáme do druhého argumentu true pro ASC, false pro DESC
			if(evaluatedArguments.Count == 1)
				evaluatedArguments.Add(false);
			else {
				// a pøípadný druhý argument pøesuneme na tøetí místo
				evaluatedArguments.Add(evaluatedArguments[1]);
				evaluatedArguments[1] = false;
			}

			return evaluatedArguments;
		}

		private const string help = "Setøídí vektor nebo øadu sestupnì";
	}
}

using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Generuje øadu n prvkù (podle druhého argumentu) 
	/// rozkopírováním jednoho prvku (v prvním argumentu)
	/// </summary>
	public class GenArray: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 2);
			this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));			
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			int count = (int)arguments[1];
			TArray result = new TArray();
			for(int i = 0; i < count; i++)
				result.Add(item);

			return result;			
		}

		private const string help = "Generuje øadu n prvkù rozkopírováním jednoho prvku";
		private const string parameters = "prvek (cokoliv); délka generované øady (int)";
	}
}

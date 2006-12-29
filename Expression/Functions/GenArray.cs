using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Generuje �adu n prvk� (podle druh�ho argumentu) 
	/// rozkop�rov�n�m jednoho prvku (v prvn�m argumentu)
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

		private const string help = "Generuje �adu n prvk� rozkop�rov�n�m jednoho prvku";
		private const string parameters = "prvek (cokoliv); d�lka generovan� �ady (int)";
	}
}

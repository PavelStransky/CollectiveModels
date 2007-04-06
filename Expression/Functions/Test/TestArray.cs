using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí testovací øadu
	/// </summary>
	public class TestArray: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);

            int count = evaluatedArguments.Count;
            for(int i = 0; i < count; i++)
                this.CheckArgumentsType(evaluatedArguments, i, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;
            int []index = new int[count];

            for(int i = 0; i < count; i++)
                index[i] = (int)arguments[i];
     
            return TArray.TestArray(index);
   		}

		private const string name = "testarray";
		private const string help = "Vytvoøí testovací øadu ve formátu (000, 001, ...)";
		private const string parameters = "dimenze1 (int)[; dimenze2 (int) ...]";
	}
}

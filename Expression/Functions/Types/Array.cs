using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Z argument� funkce vytvo�� �adu
	/// </summary>
	public class FnArray: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string ParametersHelp {get {return parameters;}}

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);

            int count = evaluatedArguments.Count;
            Type t = evaluatedArguments[0].GetType();

            for(int i = 1; i < count; i++)
                this.CheckArgumentsType(evaluatedArguments, i, evaluateArray, t);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;

            Type t = arguments[0].GetType();
            TArray result = new TArray(t, count);

            for(int i = 0; i < count; i++)
                result[i] = arguments[i];

            return result;
   		}

        private const string name = "array";
		private const string help = "Z argument� funkce vytvo�� �adu (Array)";
		private const string parameters = "prvky �ady";
	}
}

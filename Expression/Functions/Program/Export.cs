using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Ulo�� jednu prom�nnou do souboru
	/// </summary>
	public class FnExport: FunctionDefinitionIE {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
		public override string Parameters {get {return parameters;}}

		protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 2);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 3);

            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(string));
            this.CheckArgumentsType(evaluatedArguments, 2, evaluateArray, typeof(string));
		}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Export export = new Export((string)arguments[0], this.Binary(arguments, 2));
            export.Write(arguments[1]);
            export.Close();

			return null;
		}

        private const string name = "export";
		private const string help = "Ulo�� jednu prom�nnou do souboru (implicitn� bin�rn�)";
		private const string parameters = "n�zev souboru (string); prom�nn�; [;\"binary\" | \"text\"]";
	}
}

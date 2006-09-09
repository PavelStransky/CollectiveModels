using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Uloží context do souboru v argumentu 0
	/// </summary>
	public class Save: FunctionDefinitionIE {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            ArrayList evaluatedArguments = this.EvaluateArguments(context, arguments, writer);
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(string));

			this.SetPath(context, evaluatedArguments);

			if(evaluatedArguments.Count == 2)
				this.CheckArgumentsType(evaluatedArguments, 1, typeof(string));

			context.Export(evaluatedArguments[0] as string, this.Binary(evaluatedArguments, 1));

			return null;
		}

		private const string help = "Uloží context do souboru";
		private const string parameters = "soubor kontextu (string) [;\"binary\" | \"text\"]";
	}
}

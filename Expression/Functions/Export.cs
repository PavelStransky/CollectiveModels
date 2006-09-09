using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Uloží jednu promìnnou do souboru
	/// </summary>
	public class Export: FunctionDefinitionIE {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 2);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 3);
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(string));

			if(evaluatedArguments.Count == 3)
				this.CheckArgumentsType(evaluatedArguments, 2, typeof(string));

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			IExportable iExportable = arguments[1] as IExportable;
			if(iExportable != null)
				iExportable.Export(item as string, this.Binary(arguments, 2));
			else
				this.BadTypeError(arguments[1], 0);			

			return null;
		}
			
		private const string help = "Uloží jednu promìnnou do souboru (implicitnì binárnì)";
		private const string parameters = "název souboru (string); promìnná; [;\"binary\" | \"text\"]";
	}
}

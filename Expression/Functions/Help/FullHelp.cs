using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
    /// Full help for the given function (including names and types of the parameters)
	/// Vr�t� n�pov�du k zadan� funkci (v�etn� u�it�)
	/// </summary>
	public class FullHelp: FunctionDefinition {
		private FunctionDefinitions functions;
		
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="functions">Slovn�k zaregistrovan�ch funkc�</param>
		public FullHelp(FunctionDefinitions functions) : base() {
			this.functions = functions;
		}

		public override string Help {get {return Messages.HelpFullHelp;}}

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, false, false, Messages.PFnName, Messages.PFnNameDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			return functions[arguments[0] as string].FullHelp;
		}

		private const string help = "Vr�t� n�pov�du k zadan� funkci (v�etn� u�it�)";
		private const string parameters = "n�zev funkce";
	}
}

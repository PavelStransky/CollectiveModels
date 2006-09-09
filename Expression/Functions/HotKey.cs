using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvo�� horkou kl�vesu na dan� p��kaz
	/// </summary>
	public class FnHotKey: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 2);

			object str = Atom.EvaluateAtomObject(context, arguments[1]);
			if(!(str is string) || (str as string).Length != 1)
				this.BadTypeError(str, 1);

			if(arguments[0] is Atom)
				return new HotKey(arguments[0] as Atom, (str as string)[0]);
			else
				return this.BadTypeError(arguments[0], 0);
		}

		private const string name = "hotkey";
		private const string help = "Vytvo�� horkou kl�vesu na zadan� p��kaz (pou��v� se CTRL + zadan� kl�vesa)";
		private const string parameters = "p��kaz; znak (string d�lky 1)";
	}
}

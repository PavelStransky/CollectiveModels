using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvo�� �adu s pou�it�m v�ech zaregistrovan�ch funkc�
	/// </summary>
	public class Use: FunctionDefinition {
		private FunctionDefinitions functions;
		
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="functions">Slovn�k zaregistrovan�ch funkc�</param>
		public Use(FunctionDefinitions functions) : base() {
			this.functions = functions;
		}

		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsMaxNumber(arguments, 1);

			if(arguments.Count > 0) {
				if(arguments[0] is string)
					return functions[arguments[0] as string].Use;
				else
					return this.BadTypeError(arguments[0], 0);
			}
			else {
				Array result = new Array();

				foreach(FunctionDefinition f in functions.Values)
					result.Add(f.Use);

				return result;
			}
		}

		private const string help = "Pokud je zad�n n�zev funkce, vr�t� jej� pou�it�, jinak vytvo�� �adu (Array) s pou�it�m v�ech zaregistrovan�ch funkc�";
		private const string parameters = "[n�zev funkce]";
	}
}

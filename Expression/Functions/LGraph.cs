using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí graf
	/// </summary>
	/// <param name="args">Argumenty funkce
	/// 0 ... data k vykreslení (vektor, øada)
	/// 1 ... parametry (string)
	/// 2 ... parametry jednotlivých køivek (array of string)
	/// 3 ... chyby ke køivkám
	/// </param>
	public class CreateGraph: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 4);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			string graphParams = null;
			if(arguments.Count > 1 && arguments[1] != null) {
				this.CheckArgumentsType(arguments, 1, typeof(string));
				graphParams = arguments[1] as string;
			}

			// Ve 2. parametru jsou vlastnosti jednotlivých køivek grafu. Pokud je zadán jen string,
			// pak se použijí vlastnosti pro všechny køivky
			Array itemParams = null;
			if(arguments.Count > 2 && arguments[2] != null) {
				if(arguments[2] is string) {
					int length = 1;
					if(item is Array)
						length = (item as Array).Count;

					itemParams = new Array();
					for(int i = 0; i < length; i++)
						itemParams.Add(arguments[2]);
				}
				else if(arguments[2] is Array && (arguments[2] as Array).ItemType == typeof(string))
					itemParams = arguments[2] as Array;
				else
					this.BadTypeError(arguments[2], 2);
			}

			object errors = null;
			if(arguments.Count > 3 && arguments[3] != null) {
				if((arguments[3] is Array && item is Array) || (arguments[3] is Vector && (item is Vector || item is PointVector)))
					errors = arguments[3];
				else
					this.BadTypeError(arguments[3], 3);
			}

			return new Graph(item, graphParams, itemParams, errors);
		}

		private const string help = "Vytvoøí graf";
		private const string parameters = "data k vykreslení (Array | Vector | PointVector) [;parametry (string) [;parametry jednotlivých køivek (Array of string | string) [;chyby k bodùm (Array | Vector)]]]";	
	}
}

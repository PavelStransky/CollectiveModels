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

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsMaxNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments.Count > 0)
				return functions[arguments[0] as string].Use;
			else {
				TArray result = new TArray(typeof(string), functions.Count);

                int i = 0;
				foreach(FunctionDefinition f in functions.Values)
					result[i++] = f.Use;

				return result;
			}
		}

		private const string help = "Pokud je zad�n n�zev funkce, vr�t� jej� pou�it�, jinak vytvo�� �adu (Array) s pou�it�m v�ech zaregistrovan�ch funkc�";
		private const string parameters = "[n�zev funkce]";
	}
}

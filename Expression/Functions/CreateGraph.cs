using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvo�� graf
	/// </summary>
	/// <param name="args">Argumenty funkce
	/// 0 ... data k vykreslen� (vektor, �ada, �ada �ad)
    /// 1 ... pozad�
	/// 2 ... parametry (string)
	/// 3 ... parametry jednotliv�ch k�ivek (array of string)
    /// 4 ... parametry pozad�
	/// 5 ... chyby ke k�ivk�m
    /// Mo�nosti zad�n� (data, pozad�):
    /// (1, 1)
    /// (Array(N), 1) - k jednomu pozad� v�ce k�ivek najednou
    /// (Array(N, 1), 1) - k jednomu pozad� v�ce k�ivek, ale postupn� (animace)
    /// (Array(N), Array(N)) - jedna k�ivka k jednomu pozad�
    /// (Array(N, M), Array(N)) - M k�ivek ke ka�d�mu pozad�
	/// </param>
	public class CreateGraph: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 6);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item != null) {
                Type t = item.GetType();
                if(t == typeof(TArray))
                    t = (t as TArray).ItemType;

                if(t == typeof(TArray))
                    t = (t as TArray).ItemType;

                if(t != typeof(Vector) || t != typeof(PointVector))
                    this.BadTypeError(item, 0);
            }

            object background = null;
            if(arguments.Count > 1 && arguments[1] != null) {
                if((arguments[1] is TArray && (arguments[1] as TArray).ItemType == typeof(Matrix))
                    || (arguments[1] is Matrix))
                    background = arguments[1];
                else
                    this.BadTypeError(arguments[1], 1);
            }

			string graphParams = null;
			if(arguments.Count > 2 && arguments[2] != null) {
				this.CheckArgumentsType(arguments, 2, typeof(string));
				graphParams = arguments[2] as string;
			}

			// Ve 3. parametru jsou vlastnosti jednotliv�ch k�ivek grafu
            object itemParams = null;
			if(arguments.Count > 3 && arguments[3] != null) {
                Type t = arguments[3].GetType;

                if(t == typeof(TArray))
                    t = (t as TArray).ItemType;

                if(t == typeof(TArray))
                    t = (t as TArray).ItemType;

                if(t != typeof(string))
                    this.BadTypeError(item, 3);

                itemParams = arguments[3];
            }

            // Ve 4. parametru jsou vlastnosti pozad� grafu
            object backgroundParams = null;
            if(arguments.Count > 4 && arguments[4] != null) {
                Type t = arguments[4].GetType;

                if(t == typeof(TArray))
                    t = (t as TArray).ItemType;

                if(t == typeof(TArray))
                    t = (t as TArray).ItemType;

                if(t != typeof(string))
                    this.BadTypeError(item, 4);

                backgroundParams = arguments[4];
            }
            
            object errors = null;
			if(arguments.Count > 5 && arguments[5] != null) {
                Type t = arguments[5].GetType;
                if(t == typeof(TArray))
                    t = (t as TArray).ItemType;

                if(t == typeof(TArray))
                    t = (t as TArray).ItemType;

                if(t != typeof(Vector))
                    this.BadTypeError(item, 0);

                errors = arguments[5];
            }

			return new Graph(item, background, graphParams, itemParams, backgroundParams, errors);
		}

		private const string help = "Vytvo�� graf";
		private const string parameters = "data k vykreslen� (Array | Vector | PointVector)[; pozad� (Array | Matrix) [;parametry (string) [;parametry jednotliv�ch k�ivek (Array of string | string) [;parametry pozad� (Array of string | string) [;chyby k bod�m (Array | Vector)]]]]]";	
	}
}

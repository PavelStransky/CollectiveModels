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
	public class FnGraph: FunctionDefinition {
        public override string Name { get { return name; } }
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 6);

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
            // Prvn� parametr - data
            if(item != null) {
                this.CheckArgumentsType(arguments, 0, typeof(TArray), typeof(Vector), typeof(PointVector));

                Type t = item.GetType();

                if(t == typeof(TArray))
                    t = (item as TArray).ItemType;

                if(t == typeof(TArray))
                    t = ((item as TArray)[0] as TArray).ItemType;

                if(t != typeof(Vector) && t != typeof(PointVector))
                    this.BadTypeError(item, 0);
            }

            // Druh� parametr - pozad�
            object background = null;
            if(arguments.Count > 1 && arguments[1] != null) {
                this.CheckArgumentsType(arguments, 1, typeof(TArray), typeof(Matrix));

                if((arguments[1] is TArray && (arguments[1] as TArray).ItemType == typeof(Matrix))
                    || (arguments[1] is Matrix))
                    background = arguments[1];
                else
                    this.BadTypeError(arguments[1], 1);
            }

            // T�et� parametr - chybov� �se�ky
            object errors = null;
			if(arguments.Count > 2 && arguments[2] != null) {
                this.CheckArgumentsType(arguments, 2, typeof(TArray), typeof(Vector));

                Type t = arguments[2].GetType();

                if(t == typeof(TArray))
                    t = (arguments[2] as TArray).ItemType;

                if(t == typeof(TArray))
                    t = ((arguments[2] as TArray)[0] as TArray).ItemType; 
                
                if(t != typeof(Vector))
                    this.BadTypeError(item, 0);

                errors = arguments[2];
            }

            // 4. parametr - vlastnosti grafu (string nebo Context)
			object graphParams = null;
			if(arguments.Count > 3 && arguments[3] != null) {
				this.CheckArgumentsType(arguments, 3, typeof(string), typeof(Context));
				graphParams = arguments[3];
			}

			// 5. parametr - vlastnosti jednotliv�ch k�ivek grafu
            // (string nebo Context nebo Array of Context)
            object itemParams = null;
			if(arguments.Count > 4 && arguments[4] != null) {
                this.CheckArgumentsType(arguments, 4, typeof(TArray), typeof(Context));
                
                Type t = arguments[4].GetType();

                if(t == typeof(TArray))
                    t = (arguments[4] as TArray).ItemType;

                if(t == typeof(TArray))
                    t = ((arguments[4] as TArray)[0] as TArray).ItemType;

                if(t != typeof(string) && t != typeof(Context))
                    this.BadTypeError(item, 4);

                itemParams = arguments[4];
            }

            // 6. parametr - vlastnosti pozad� grafu
            object backgroundParams = null;
            if(arguments.Count > 5 && arguments[5] != null) {
                this.CheckArgumentsType(arguments, 5, typeof(TArray), typeof(Context));

                Type t = arguments[5].GetType();

                if(t == typeof(TArray))
                    t = (arguments[5] as TArray).ItemType;

                if(t != typeof(string))
                    this.BadTypeError(item, 5);

                backgroundParams = arguments[5];
            }

            return new Graph(item, background, errors, graphParams, itemParams, backgroundParams);
		}

        private const string name = "graph";
		private const string help = "Vytvo�� graf";
        private const string parameters = "data k vykreslen� (Array | Vector | PointVector)[; pozad� (Array | Matrix)[; chyby k bod�m (Array | Vector) [;parametry (string | Context) [;parametry jednotliv�ch k�ivek (string | Array | Context) [;parametry pozad� (string | Array | Context)]]]]]";	
	}
}

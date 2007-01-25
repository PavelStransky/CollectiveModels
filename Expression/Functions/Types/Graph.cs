using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí graf
	/// </summary>
	/// <param name="args">Argumenty funkce
	/// 0 ... data k vykreslení (vektor, øada, øada øad)
    /// 1 ... pozadí
	/// 2 ... parametry (string)
	/// 3 ... parametry jednotlivých køivek (array of string)
    /// 4 ... parametry pozadí
	/// 5 ... chyby ke køivkám
    /// Možnosti zadání (data, pozadí):
    /// (1, 1)
    /// (Array(N), 1) - k jednomu pozadí více køivek najednou
    /// (Array(N, 1), 1) - k jednomu pozadí více køivek, ale postupnì (animace)
    /// (Array(N), Array(N)) - jedna køivka k jednomu pozadí
    /// (Array(N, M), Array(N)) - M køivek ke každému pozadí
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
            // První parametr - data
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

            // Druhý parametr - pozadí
            object background = null;
            if(arguments.Count > 1 && arguments[1] != null) {
                this.CheckArgumentsType(arguments, 1, typeof(TArray), typeof(Matrix));

                if((arguments[1] is TArray && (arguments[1] as TArray).ItemType == typeof(Matrix))
                    || (arguments[1] is Matrix))
                    background = arguments[1];
                else
                    this.BadTypeError(arguments[1], 1);
            }

            // Tøetí parametr - chybové úseèky
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

			// 5. parametr - vlastnosti jednotlivých køivek grafu
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

            // 6. parametr - vlastnosti pozadí grafu
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
		private const string help = "Vytvoøí graf";
        private const string parameters = "data k vykreslení (Array | Vector | PointVector)[; pozadí (Array | Matrix)[; chyby k bodùm (Array | Vector) [;parametry (string | Context) [;parametry jednotlivých køivek (string | Array | Context) [;parametry pozadí (string | Array | Context)]]]]]";	
	}
}

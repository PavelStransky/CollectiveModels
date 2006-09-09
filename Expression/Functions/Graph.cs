using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvo�� graf
	/// </summary>
	/// <param name="args">Argumenty funkce
	/// 0 ... data k vykreslen� (matice, vektor, �ada)
	/// 1 ... parametry (string)
	/// 2 ... popisky X (array of string)
	/// 3 ... popisky Y (array of string)
	/// 4 ... parametry jednotliv�ch k�ivek (array of string)
	/// </param>
	public class FnGraph: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 5);
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			object retValue = null;

			string graphParams = null;
			if(arguments.Count > 1 && arguments[1] != null) {
				this.CheckArgumentsType(arguments, 1, typeof(string));
				graphParams = arguments[1] as string;
			}

			// Ve 2. parametru jsou vlastnosti jednotliv�ch k�ivek grafu. Pokud je zad�n jen string,
			// pak se pou�ij� vlastnosti pro v�echny k�ivky
			Array itemParams = null;
			if(arguments.Count > 2 && arguments[2] != null)
				if(arguments[2] is string) {
					int length = 1;
					if(item is Array)
						length = (item as Array).Count;

					itemParams = new Array();
					for(int i = 0; i < length; i++)
						itemParams.Add(arguments[2]);
				}
				else if(arguments[2] is Array && (arguments[2] as Array).ItemTypeName == typeof(string).FullName)
					itemParams = arguments[2] as Array;
				else
					this.BadTypeError(arguments[2], 2);

			Array labelsX = null;
			if(arguments.Count > 3 && arguments[3] != null) {
				this.CheckArgumentsType(arguments, 3, typeof(Array));
				if((arguments[3] as Array).ItemTypeName == typeof(string).FullName)
					labelsX = arguments[3] as Array;
				else
					this.BadTypeError(arguments[3], 3);
			}

			Array labelsY = null;
			if(arguments.Count > 4 && arguments[4] != null) {
				this.CheckArgumentsType(arguments, 4, typeof(Array));
				if((arguments[4] as Array).ItemTypeName == typeof(string).FullName)
					labelsY = arguments[4] as Array;
				else
					this.BadTypeError(arguments[4], 4);
			}

			// Matice - graf hustot
			if(item is Matrix) 
				retValue = new Graph(PavelStransky.Expression.Graph.GraphTypes.Density, item, graphParams, itemParams);
			else if(item is Array)
				retValue = new Graph(PavelStransky.Expression.Graph.GraphTypes.LineArray, item, graphParams, itemParams);
			else if(item is Vector || item is PointVector)
				retValue = new Graph(PavelStransky.Expression.Graph.GraphTypes.Line, item, graphParams, itemParams);

			return retValue;			
		}

		private const string name = "graph";
		private const string help = "Vytvo�� graf";
		private const string parameters = "data k vykreslen� (Matrix | Array | Vector | PointVector) [;parametry (string) [;parametry jednotliv�ch k�ivek (Array of string | string) [;popisky X (Array of string) [;popisky Y (Array of string)]]]]";	
	}
}

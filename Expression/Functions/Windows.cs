using System;
using System.Collections;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Rozd�l� data do ok�nek
	/// </summary>
	public class Windows: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 5);
			this.CheckArgumentsType(evaluatedArguments, 1, typeof(int));
			this.CheckArgumentsType(evaluatedArguments, 2, typeof(int));
			this.CheckArgumentsType(evaluatedArguments, 3, typeof(int));
			this.CheckArgumentsType(evaluatedArguments, 4, typeof(int));

			evaluatedArguments[4] = (int)evaluatedArguments[4] + 1;

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			int length = (int)arguments[1];
			int interval = (int)arguments[2];
			int outDepth = (int)arguments[3];
			int inDepth = (int)arguments[4];

			if(outDepth > depth) {
				if(item is Array)
					return this.EvaluateArray(depth, item as Array, arguments);
				else
					return this.BadTypeError(item, 0);
			}
			else if(outDepth == depth) {
				int numWindows = 0;
				object o = item;

				for(int i = 0; i < inDepth - depth - 1; i++) {
					if(o is Array && (o as Array).Count > 0)
						o = (o as Array)[0];
					else
						this.BadTypeError(o, 0);
				}

				if(o is Array)
					numWindows = ((o as Array).Count - length) / interval + 1;
				else if(o is Vector)
					numWindows = ((o as Vector).Length - length) / interval + 1;
				else
					this.BadTypeError(o, 0);

				if(arguments.Count <= 5)
					arguments.Add(null);

				Array result = new Array();
				for(int i = 0; i < numWindows; i++) {
					arguments[5] = i;
					result.Add(this.Evaluate(depth + 1, item, arguments));
				}

				return result;
			}
			else {
				int window = (int)arguments[5];

				if(inDepth > depth) {
					if(item is Array)
						return this.EvaluateArray(depth, item as Array, arguments);
					else
						return this.BadTypeError(item, 0);
				}
				else {
					if(item is Array) {
						Array aIn = item as Array;
						Array aOut = new Array();

						for(int k = 0; k < length; k++)
							aOut.Add(aIn[k + window * interval]);

						return aOut;
					}
					else if(item is Vector) {	
						Vector vIn = item as Vector;
						Vector vOut = new Vector(length);
						for(int k = 0; k < length; k++)
							vOut[k] = vIn[k + window * interval];

						return vOut;
					}
					else
						return this.BadTypeError(item, 0);
				}
			}
		}

		private const string help = "Rozd�l� data do ok�nek - data vezme ze zadan� hloubky a vlo�� na jinou hloubku";
		private const string parameters = "Vector | Array; d�lka ok�nka (int); posun ok�nka (int); v�stupn� hloubka (int); vstupn� hloubka (int)";
	}
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Prvky øady uspoøádá do vektoru
	/// </summary>
	public class Array2Vector: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Name {get {return name;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(Array));
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			Array array = item as Array;

			if(array.ItemTypeName == typeof(double).FullName) {
				Vector result = new Vector(array.Count);
				for(int i = 0; i < array.Count; i++)
					result[i] = (double)array[i];
				return result;
			}
			else if(array.ItemTypeName == typeof(int).FullName) {
				Vector result = new Vector(array.Count);
				for(int i = 0; i < array.Count; i++)
					result[i] = (int)array[i];
				return result;
			}
			else if(array.ItemTypeName == typeof(Array).FullName) 
				return this.EvaluateArray(depth, array, arguments);
			else
				return this.BadTypeError(item, 0);
		}

		private const string name = "a2v";
		private const string help = "Prvky øady uspoøádá do vektoru";
		private const string parameters = "Array";
	}
}

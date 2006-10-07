using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrátí hodnoty jako øetìzec
	/// </summary>
	public class String: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

			if(evaluatedArguments.Count > 1) 
				this.CheckArgumentsType(evaluatedArguments, 1, typeof(string));
			else
				evaluatedArguments.Add(string.Empty);
			
			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			string format = arguments[1] as string;

            if(item is int)
                return ((int)item).ToString(format);
            else if(item is double)
                return ((double)item).ToString(format);
            else if(item is DateTime)
                return ((DateTime)item).ToString(format);
            else if(item is PointD || item is Vector || item is PointVector || item is Matrix)
                return item.ToString();
            else if(item is string)
                return item as string;
            else if(item is Array)
                return this.EvaluateArray(depth, item as Array, arguments);
            else
                return this.BadTypeError(item, 0);			
		}

		private const string help = "Vrátí hodnoty jako øetìzec";
		private const string parameters = "int | double | Point | Vector | PointVector | Matrix [; formát (string)]";
	}
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vrátí hodnoty jako øetìzec
	/// </summary>
	public class FnString: FunctionDefinition {
        public override string Name { get { return name; } }
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override void CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(int), typeof(double), typeof(DateTime), typeof(PointD),
                typeof(PointVector), typeof(Vector), typeof(Matrix), typeof(string));
			this.CheckArgumentsType(evaluatedArguments, 1, typeof(string));
		}

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];
            string format = arguments.Count > 1 ? arguments[1] as string : string.Empty;

            if(item is int)
                return ((int)item).ToString(format);
            else if(item is double)
                return ((double)item).ToString(format);
            else if(item is DateTime)
                return ((DateTime)item).ToString(format);
            else if(item is string)
                return item as string;
            else
                return item.ToString();
		}

        private const string name = "string";
		private const string help = "Vrátí hodnoty jako øetìzec";
		private const string parameters = "int | double | Point | Vector | PointVector | Matrix | DateTime | string [; formát (string)]";
	}
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Ascending sort
	/// </summary>
	public class Sort: FunctionDefinition {
		public override string Help {get {return Messages.SortHelp;}}
		public override string Parameters {get {return Messages.SortParams;}}

		protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(ISortable));
            this.CheckArgumentsType(evaluatedArguments, 1, evaluateArray, typeof(ISortable));
		}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            ISortable item = arguments[0] as ISortable;

            if(arguments.Count > 1)
                return item.Sort(arguments[1] as ISortable);
            else
                return item.Sort();
        }
	}
}

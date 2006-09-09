using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Permutuje øadu argumentu 1 podle poøadí øady argumentu 2
	/// </summary>
	public class Permute: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 2);
			this.CheckArgumentsMaxNumber(evaluatedArguments, 2);
			this.CheckArgumentsType(evaluatedArguments, 0, typeof(Array));
			this.CheckArgumentsType(evaluatedArguments, 1, typeof(Array));			

			return evaluatedArguments;
		}

		protected override object Evaluate(int depth, object item, ArrayList arguments) {
			Array aIn = arguments[0] as Array;
			Array aOrder = arguments[1] as Array;
			Array result = new Array();

			if(aIn.Count != aOrder.Count)
				throw new FunctionDefinitionException(string.Format(errorMessageBadLength, this.Name),
					string.Format(errorMessageBadLengthDetail, aIn.Count, aOrder.Count));
			if(aOrder.ItemTypeName != typeof(int).FullName)
				this.BadTypeError(aOrder, 2);

			for(int i = 0; i < aOrder.Count; i++)
				result.Add(aIn[(int)aOrder[i]]);

			return result;			
		}

		private const string errorMessageBadLength = "Chybná délka øady ve funkci '{0}'. Øady musí být stejnì dlouhé.";
		private const string errorMessageBadLengthDetail = "Délka prní øady: {0}\nDélka druhé øady: {1}";

		private const string help = "Permutuje øadu argumentu 1 podle poøadí øady argumentu 2";
		private const string parameters = "Array; poøadí permutací (Array)";
	}
}

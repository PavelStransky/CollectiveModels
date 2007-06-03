using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions
{
	/// <summary>
	/// Returns length(s) of given object as an array
	/// </summary>
	public class Length: FunctionDefinition {
		public override string Help {get {return Messages.LengthHelp;}}
		public override string Parameters {get {return Messages.LengthParameters;}}

		protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, 
                typeof(Vector), typeof(PointVector), typeof(Matrix), typeof(TArray), typeof(List));
		}

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector)
                return new TArray((item as Vector).Length);

            else if(item is PointVector)
                return new TArray((item as PointVector).Length);

            else if(item is Matrix)
                return new TArray((item as Matrix).LengthX, (item as Matrix).LengthY);

            else if(item is List)
                return new TArray((item as List).Count);

            else if(item is TArray)
                return new TArray((item as TArray).Lengths);

            return null;
		}
	}
}

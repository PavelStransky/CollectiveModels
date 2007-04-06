using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions
{
	/// <summary>
	/// Vrac� rozm�ry dan�ho prvku jako �adu
	/// </summary>
	public class Length: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override void CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(Vector), typeof(PointVector), typeof(Matrix), typeof(TArray));
		}

        protected override object EvaluateArray(Guider guider, ArrayList arguments) {
            return this.EvaluateFn(guider, arguments);
        }

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector)
                return new TArray((item as Vector).Length);

            else if(item is PointVector)
                return new TArray((item as PointVector).Length);

            else if(item is Matrix)
                return new TArray((item as Matrix).LengthX, (item as Matrix).LengthY);

            else 
                return new TArray((item as TArray).Lengths);
		}

		private const string help = "Vrac� rozm�ry objektu (po�ty prvk� v jednotliv�ch dimenz�ch) jako �adu";
		private const string parameters = "Vector | PointVector | Matrix | TArray";
	}

}

using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def
{
	/// <summary>
	/// Returns length(s) of given object as an array
	/// </summary>
	public class Length: Fnc {
		public override string Help {get {return Messages.HelpLength;}}

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PMultiDimensions, Messages.PMultiDimensionsDescription, null,
                typeof(Vector), typeof(TArray), typeof(Matrix), typeof(List), typeof(PointVector));
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

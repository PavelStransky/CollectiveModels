using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Calculates neighbour spacing v_{i+j} - v_{i}
	/// </summary>
	public class Spacing: Fnc {
		public override string Help {get {return Messages.HelpSpacing;}}

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, false, true, false, Messages.PSpacing, Messages.PSpacingDescription, 1, typeof(int));
        }

		protected override object EvaluateFn(Guider guider, ArrayList arguments) {
			Vector v = arguments[0] as Vector;
			int spacing = (int)arguments[1];
			Vector result = new Vector(v.Length - spacing);
				
            int length = v.Length;
			for(int i = spacing; i < length; i++)
				result[i - spacing] = v[i] - v[i - spacing];

			return result;
		}
	}
}

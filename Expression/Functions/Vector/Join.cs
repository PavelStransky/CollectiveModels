using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Øadu vektorù nebo vektorù bodù slouèí do jednoho vektoru
	/// </summary>
	public class Join: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override void CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(Vector), typeof(PointVector));
		}

        protected override object EvaluateArray(Guider guider, ArrayList arguments) {
            return this.EvaluateFn(guider, arguments);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector || item is PointVector)
                return item;

            TArray ta = item as TArray;

            if(ta.GetItemType() == typeof(Vector)) {
                int length = ta.GetNumElements();
                Vector[]vector = new Vector[length];

                ta.ResetEnumerator();

                int i = 0;
                foreach(Vector v in ta)
                    vector[i++] = v;

                return Vector.Join(vector);
            }

            else {
                int length = ta.GetNumElements();
                PointVector[] vector = new PointVector[length];

                ta.ResetEnumerator();

                int i = 0;
                foreach(PointVector v in ta)
                    vector[i++] = v;

                return PointVector.Join(vector);
            }
        }

		private const string help = "Øadu vektorù nebo vektory slouèí do jednoho vektoru";
		private const string parameters = "Vector | PointVector";
	}
}

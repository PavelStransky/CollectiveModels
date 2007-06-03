using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
    /// Array of Vectors or Array of PointVectors joins into one Vector
	/// </summary>
	public class Join: FunctionDefinition {
		public override string Help {get {return Messages.JoinHelp;}}

        protected override void CreateParameters() {
            this.NumParams(1, true);

            this.SetParam(0, true, true, false, Messages.JoinP1, Messages.JoinP1Description, null, typeof(Vector), typeof(PointVector), typeof(TArray));
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

            else if(ta.GetItemType() == typeof(PointVector)) {
                int length = ta.GetNumElements();
                PointVector[] vector = new PointVector[length];

                ta.ResetEnumerator();

                int i = 0;
                foreach(PointVector v in ta)
                    vector[i++] = v;

                return PointVector.Join(vector);
            }

            return null;
        }
	}
}

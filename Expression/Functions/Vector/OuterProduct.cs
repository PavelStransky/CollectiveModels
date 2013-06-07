using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Outer product of two vectors
    /// </summary>
    public class OuterProduct : Fnc {
        public override string Help { get { return Messages.HelpOuterProduct; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v1 = arguments[0] as Vector;
            Vector v2 = arguments[1] as Vector;

            int lengthX = v1.Length;
            int lengthY = v2.Length;

            Matrix result = new Matrix(lengthX, lengthY);
            for(int i = 0; i < lengthX; i++)
                for(int j = 0; j < lengthY; j++)
                    result[i, j] = v1[i] * v2[j];

            return result;
        }
    }
}

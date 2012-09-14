using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Put a given vector at each of the given times
    /// </summary>
    public class CreateByCopy: Fnc {
        public override string Help { get { return Messages.HelpCreateByCopy; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.PIndex, Messages.PIndexDescription, null, typeof(TArray));
            this.SetParam(2, false, true, false, Messages.PLength, Messages.PLengthDescription, -1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = (Vector)arguments[0];
            TArray ind = (TArray)arguments[1];
            int length = (int)arguments[2];

            int vLength = v.Length;
            int num = ind.Length;

            if(length < 0)
                length = v.Length + (int)ind[num - 1];

            Vector result = new Vector(length);
            for(int i = 0; i < num; i++) {
                int k = (int)ind[i];
                for(int j = 0; j < vLength; j++)
                    if(k + j < length)
                        result[k + j] += v[j];
            }
            return result;
        }
    }
}

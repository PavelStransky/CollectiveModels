using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns a vector of cumulations sum_{i=0}^{k} s_{k}, where the first element is 0;
    /// </summary>
    public class Cumul0: Fnc {
        public override string Help { get { return Messages.HelpCumul0; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = arguments[0] as Vector;
            Vector result = new Vector(v.Length + 1);

            int length = v.Length;
            result[0] = 0;
            for(int i = 0; i < length; i++)
                result[i + 1] = result[i] + v[i];

            return result;
        }
    }
}

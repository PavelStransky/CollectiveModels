using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns a vector of cumulations sum_{i=0}^{k} s_{k}
    /// </summary>
    public class Cumul: Fnc {
        public override string Help { get { return Messages.HelpCumul; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = arguments[0] as Vector;
            Vector result = new Vector(v.Length);

            int length = v.Length;
            result[0] = v[0];
            for(int i = 1; i < length; i++)
                result[i] = result[i - 1] + v[i];

            return result;
        }
    }
}

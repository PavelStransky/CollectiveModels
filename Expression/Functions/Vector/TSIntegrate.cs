using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Integrates a time series S_{i}=sum_{j=1}^{i}(s_{j}-<s>)
    /// </summary>
    public class TSIntegrate: Fnc {
        public override string Help { get { return Messages.HelpTSIntegrate; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = arguments[0] as Vector;
            double mean = v.Mean();

            int length = v.Length;
            Vector result = new Vector(length);

            result[0] = v[0] - mean;

            for(int i = 1; i < length; i++)
                result[i] = result[i - 1] - mean;

            return result;
        }
    }
}

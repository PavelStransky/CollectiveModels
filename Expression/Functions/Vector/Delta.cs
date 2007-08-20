using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Calculates statistics v_{i} - v_{0} - i
    /// </summary>
    public class Delta: FunctionDefinition {
        public override string Help { get { return Messages.HelpDelta; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = arguments[0] as Vector;


            int length = v.Length;
            Vector result = new Vector(length);

            double v0 = v[0];

            for(int i = 0; i < length; i++)
                result[i] = v[i] - v0 - i;

            return result;
        }
    }
}

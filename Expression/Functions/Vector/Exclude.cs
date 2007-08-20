using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// From the first vector excludes values contained in the second vector
    /// </summary>
    public class Exclude: FunctionDefinition {
        public override string Help { get { return Messages.HelpExclude; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.P1Exclude, Messages.P1ExcludeDescription, null,
                typeof(Vector), typeof(int));
            this.SetParam(2, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 1.0E-6, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v = (Vector)arguments[0];
            Vector exc = (Vector)arguments[1];
            double precision = (double)arguments[2];

            int li = v.Length;
            int le = exc.Length;
            int l = 0;

            Vector result = new Vector(li);

            for(int i = 0; i < li; i++) {
                bool found = false;

                for(int j = 0; j < le; j++)
                    if(v[i] - precision <= exc[j] && v[i] + precision >= exc[j]) {
                        found = true;
                        break;
                    }

                if(!found)
                    result[l++] = v[i];
            }
                    

            result.Length = l;

            return result;
        }
    }
}

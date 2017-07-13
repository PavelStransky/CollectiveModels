using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Finds all intersection points of lines given in the form y = y0 + k x
    /// </summary>
    public class EPIntersection : Fnc {
        public override string Help { get { return Messages.HelpEPIntersection; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PY, Messages.PYDescription, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.PK, Messages.PKDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector y = arguments[0] as Vector;
            Vector k = arguments[1] as Vector;

            int length = y.Length;

            PointVector result = new PointVector(length * (length - 1) / 2);
            int l = 0;
            for(int i = 0; i < length; i++)
                for(int j = i + 1; j < length; j++) {
                    double x = (y[i] - y[j]) / (k[j] - k[i]);
                    result[l++] = new PointD(x, y[i] + x * k[i]);
                }

            return result;
        }
    }
}
using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Phase between two numbers or vectors
    /// </summary>
    public class PhaseFn: Fnc {
        public override string Help { get { return Messages.HelpPhase; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, true, Messages.PReal, Messages.PRealDescription, null, typeof(double), typeof(Vector));
            this.SetParam(1, true, true, true, Messages.PImmaginary, Messages.PImmaginaryDescription, null, typeof(double), typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments[0] is double) {
                double d1 = (double)arguments[0];
                double d2 = (double)arguments[1];

                return this.Phase(d1, d2);
            }

            if(arguments[0] is Vector) {
                Vector v1 = (Vector)arguments[0];
                Vector v2 = (Vector)arguments[1];

                int length = v1.Length;

                if(length != v2.Length)
                    throw new FncException(
                        this,
                        Messages.EMNotEqualLength,
                        string.Format(Messages.EMNotEqualLengthDetail, length, v2.Length));

                Vector result = new Vector(length);
                for(int i = 0; i < length; i++)
                    result[i] = this.Phase(v1[i], v2[i]);

                return result;
            }

            return null;
        }

        private double Phase(double re, double im) {
            return System.Math.Atan(im / re);
        }

        private const string name = "phase";
    }
}

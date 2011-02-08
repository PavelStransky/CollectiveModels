using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Data of input pointvector interprets as mean values and standard deviations of a set of Gaussian functions
    /// </summary>
    public class GaussianSmooth: FncMathD {
        public override string Help { get { return Messages.HelpGaussianSmooth; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetXParam();
            this.SetParam(1, true, true, false, Messages.PGaussianCoef, Messages.PGaussianCoefDescription, null, typeof(PointVector), typeof(Vector));
            this.SetParam(2, false, true, false, Messages.PWeight, Messages.PWeightDescription, null, typeof(Vector));
        }

        protected override double FnDouble(double x, params object[] p) {
            double result = 0.0;

            Vector weight = null;
            if(p.Length > 1)
                weight = (Vector)p[1];

            if(p[0] is Vector) {
                Vector gv = (Vector)p[0];
                int length = gv.Length;

                for(int i = 0; i < length; i++)
                    result += (weight == null ? 1.0 : weight[i]) * SpecialFunctions.Gaussian(x, gv[i], 1.0);
            }
            else if(p[0] is PointVector) {
                PointVector gpv = (PointVector)p[0];
                int length = gpv.Length;

                for(int i = 0; i < length; i++)
                    result += (weight == null ? 1.0 : weight[i]) * SpecialFunctions.Gaussian(x, gpv[i].X, gpv[i].Y);
            }

            return result;
        }
    }
}
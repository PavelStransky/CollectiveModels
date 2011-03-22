using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Unfolds given data
    /// </summary>
    public class Unfolding: Fnc {
        public override string Help { get { return Messages.HelpUnfolding; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PEnergies, Messages.PEnergiesDescription, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.PParam, Messages.PParamDescription, null, typeof(int));
            this.SetParam(2, false, true, false, Messages.PUnfoldingType, Messages.PUnfoldingTypeDescription, "cpolynom", typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector e = arguments[0] as Vector;
            int order = (int)arguments[1];

            PointVector histogram = e.CumulativeHistogramStep(); // / (new PointD(1.0, e.Length));
            Vector regression = Regression.Compute(histogram, order);

            // Approximation of the histogram curve
            int length = histogram.Length / 2;
            PointVector approx = new PointVector(length);
            double error = 0.0;
            for(int i = 0; i < length; i++) {
                double x = histogram[2 * i].X;
                double y = Polynom.GetValue(regression, x);
                approx[i] = new PointD(x, y);

                double ye = (y - histogram[2 * i].Y);
                error += ye * ye;
                ye = (y - histogram[2 * i + 1].Y);
                error += ye * ye;
            }

            // Unfolding
            length = e.Length;
            Vector unfolded = new Vector(length);
            for(int i = 0; i < length; i++)
                unfolded[i] = Polynom.GetValue(regression, e[i]);

            List result = new List();
            result.Add(histogram);
            result.Add(regression);
            result.Add(approx);
            result.Add(error);
            result.Add(unfolded);

            return result;
        }
    }
}
using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Finds all intersection points of two pointvectors
    /// </summary>
    public class Intersection: Fnc {
        public override string Help { get { return Messages.HelpIntersection; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector pv1 = arguments[0] as PointVector;
            PointVector pv2 = arguments[1] as PointVector;

            int length1 = pv1.Length;
            int length2 = pv2.Length;

            ArrayList r = new ArrayList();

            for(int i = 1; i < length1; i++) {
                double x1min = pv1[i - 1].X;
                double y1min = pv1[i - 1].Y;
                double x1max = pv1[i].X;
                double y1max = pv1[i].Y;

                for(int j = 1; j < length2; j++) {
                    double x2min = pv2[j - 1].X;
                    double y2min = pv2[j - 1].Y;
                    double x2max = pv2[j].X;
                    double y2max = pv2[j].Y;

                    double d = (x1max - x1min) * (y2max - y2min) - (y1max - y1min) * (x2max - x2min);
                    double x = ((y1min * x1max - y1max * x1min) * (x2max - x2min) + (y2max * x2min - y2min * x2max) * (x1max - x1min));
                    x /= d;
                    double y = ((y1min * x1max - y1max * x1min) * (y2max - y2min) + (y2max * x2min - y2min * x2max) * (y1max - y1min));
                    y /= d;

                    // Existuje prùnik
                    if(((x >= x1min && x <= x1max) || (x >= x1max && x <= x1min)) && ((y >= y1min && y <= y1max) || (y >= y1max && y <= y1min))
                        && ((x >= x2min && x <= x2max) || (x >= x2max && x <= x2min)) && ((y >= y2min && y <= y2max) || (y >= y2max && y <= y2min)))
                        r.Add(new PointD(x, y));
                }
            }

            int count = r.Count;
            PointVector result = new PointVector(count);
            for(int i = 0; i < count; i++)
                result[i] = (PointD)r[i];

            return result;
        }
    }
}
//((bc-ad)(h-g)+(d-c)(fg-eh))/((d-c)(f-e)-(b-a)(h-g))
//((ad-bc)(f-e)+(b-a)(fg-eh))/((d-c)(f-e)-(b-a)(h-g))

//(a d (g - h) + b c (-g + h) + (c - d) (f g - e h))/(
//d (e - f) + c (-e + f) + (a - b) (g - h)), y -> (
// a (d (e - f) + f g - e h) + b (c (-e + f) - f g + e h))/(
// d (e - f) + c (-e + f) + (a - b) (g - h))

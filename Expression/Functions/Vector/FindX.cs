using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// From the Y value finds the best X value (using linear interpolation and multiplicity)
    /// </summary>
    public class FindX : Fnc {
        public override string Help { get { return Messages.HelpD1; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVector, null, typeof(PointVector));
            this.SetParam(1, true, true, true, Messages.PValue, Messages.PValueDescription, 0.0, typeof(PointD), typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector pv = arguments[0] as PointVector;

            ArrayList a = new ArrayList();
            int num = 0;
            int i = 0;

            if (arguments[1] is PointD) {
                PointD p = (PointD)arguments[1];
                num = this.Find(a, pv, p);
            }
            else {
                PointVector v = (PointVector)arguments[1];
                int length = v.Length;

                for (i = 0; i < length; i++)
                    num = System.Math.Max(num, this.Find(a, pv, v[i]));
            }

            PointVector result = new PointVector(a.Count);
            i = 0;
            foreach (PointD p in a)
                result[i++] = p;

            if (guider != null)
                guider.Write(num);

            return result;
        }

        private int Find(ArrayList a, PointVector pv, PointD p) {
            int length = pv.Length;
            int num = 0;

            double x = p.X;
            double y = p.Y;

            for (int i = 1; i < length; i++)
                if ((pv[i - 1].Y > x && pv[i].Y <= x) || (pv[i - 1].Y < x && pv[i].Y >= x)) {
                    double koef = (pv[i].X - pv[i - 1].X) / (pv[i].Y - pv[i - 1].Y);
                    a.Add(new PointD((pv[i].Y - x) * koef + pv[i].X, y));
                    num++;
                }

            return num;
        }
    }
}
using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def
{
    /// <summary>
    /// Calculates complex level density on a grid
    /// </summary>
    public class ComplexLevelDensity : Fnc
    {
        public override string Help { get { return Messages.HelpComplexLevelDensity; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PResonances, Messages.PResonancesDescription, null, typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.PIntervalX, Messages.PIntervalXDescription, null, typeof(double), typeof(Vector));
            this.SetParam(2, true, true, false, Messages.PIntervalY, Messages.PIntervalYDescription, null, typeof(double), typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector resonances = arguments[0] as PointVector;

            List result = new List();

            if (arguments[1] is double) {
                double e = (double)arguments[1];

                if (arguments[2] is double) {
                    PointD p = this.LD(resonances, new PointD(e, (double)arguments[2]));
                    result.Add(p.X);
                    result.Add(p.Y);
                }
                else {
                    Vector vg = (Vector)arguments[2];

                    Vector re = new Vector(vg.Length);
                    Vector ri = new Vector(vg.Length);

                    for (int i = 0; i < vg.Length; i++) {
                        PointD p = this.LD(resonances, new PointD(e, vg[i]));
                        re[i] = p.X;
                        ri[i] = p.Y;
                    }

                    result.Add(re);
                    result.Add(ri);
                }
            }
            else {
                Vector ve = (Vector)arguments[1];

                if(arguments[2] is double) {
                    double g = (double)arguments[2];

                    Vector re = new Vector(ve.Length);
                    Vector ri = new Vector(ve.Length);

                    for (int i = 0; i < ve.Length; i++) {
                        PointD p = this.LD(resonances, new PointD(ve[i], g));
                        re[i] = p.X;
                        ri[i] = p.Y;
                    }

                    result.Add(re);
                    result.Add(ri);
                }
                else {
                    Vector vg = (Vector)arguments[2];

                    Matrix re = new Matrix(ve.Length, vg.Length);
                    Matrix ri = new Matrix(ve.Length, vg.Length);

                    for (int i = 0; i < ve.Length; i++) {
                        for (int j = 0; j < vg.Length; j++) {
                            PointD p = this.LD(resonances, new PointD(ve[i], vg[j]));
                            re[i, j] = p.X;
                            ri[i, j] = p.Y;
                        }
                    }

                    result.Add(re);
                    result.Add(ri);
                }
            }

            return result;
        }

        private PointD LD(PointVector resonances, PointD p) {
            PointD result = new PointD();
            for(int i = 0; i < resonances.Length; i++) {
                double de = p.X - resonances[i].X;
                double dg = p.Y - resonances[i].Y;
                double denominator = de * de + dg * dg;
                result.X += dg / denominator;
                result.Y += de / denominator;
            }
            result.X *= 1.0 / System.Math.PI;
            result.Y *= 1.0 / System.Math.PI;

            return result;
        }
    }
}

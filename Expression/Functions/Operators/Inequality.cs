using System.Collections;
using System.Text;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Operator of inequality
    /// </summary>
    public class Inequality: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpInequality; } }
        public override OperatorPriority Priority { get { return OperatorPriority.ComparePriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, true, true, false, Messages.PValue, Messages.PValueDescription, null,
                typeof(int), typeof(double), typeof(Vector), typeof(Matrix), typeof(PointD), typeof(PointVector), typeof(string));

            this.AddCompatibility(typeof(int), typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int[] lengths = this.GetTypesLength(arguments, false);
            int count = arguments.Count;

            object item = arguments[0];

            if(lengths[0] > 0 && lengths[2] > 0) {  // Double i int hodnoty
                for(int k = 1; k < count; k++) {
                    item = arguments[k];
                    double d = (item is int) ? (int)item : (double)item;

                    for(int l = 0; l < k; l++) {
                        item = arguments[l];
                        if(item is double) {
                            if((double)item == d)
                                return false;
                        }
                        else if(item is int) {
                            if((int)item == d)
                                return false;
                        }
                    }
                }
                return true;
            }

            else if(item is int) {
                for(int k = 1; k < count; k++) {
                    int i = (int)arguments[k];
                    for(int l = 0; l < k; l++)
                        if(i == (int)arguments[l])
                            return false;
                }
                return true;
            }

            else if(item is double) {
                for(int k = 1; k < count; k++) {
                    double d = (double)arguments[k];
                    for(int l = 0; l < k; l++)
                        if(d == (double)arguments[l])
                            return false;
                }
                return true;
            }

            else if(item is Vector) {
                for(int k = 1; k < count; k++) {
                    Vector v = arguments[k] as Vector;
                    int length = v.Length;

                    for(int l = 0; l < k; l++) {
                        Vector vl = arguments[l] as Vector;

                        if(length != vl.Length)
                            continue;

                        bool equal = true;
                        for(int i = 0; i < length; i++)
                            if(v[i] != vl[i]) {
                                equal = false;
                                break;
                            }
                        if(equal)
                            return false;
                    }
                }
                return true;
            }

            else if(item is Matrix) {
                for(int k = 1; k < count; k++) {
                    Matrix m = arguments[k] as Matrix;
                    int lengthx = m.LengthX;
                    int lengthy = m.LengthY;

                    for(int l = 0; l < k; l++) {
                        Matrix ml = arguments[l] as Matrix;

                        if(lengthx != ml.LengthX || lengthy != ml.LengthY)
                            continue;

                        bool equal = true;
                        for(int i = 0; i < lengthx; i++)
                            for(int j = 0; j < lengthy; j++)
                                if(m[i, j] != ml[i, j]) {
                                    equal = false;
                                    break;
                                }
                        if(equal)
                            return false;
                    }
                }
                return true;
            }

            else if(item is PointD) {
                for(int k = 1; k < count; k++) {
                    PointD p = (PointD)arguments[k];

                    for(int l = 0; l < k; l++) {
                        PointD pl = (PointD)arguments[l];

                        if(p.X == pl.X && p.Y == pl.Y)
                            return false;
                    }
                }
                return true;
            }

            else if(item is PointVector) {
                for(int k = 1; k < count; k++) {
                    PointVector pv = arguments[k] as PointVector;
                    int length = pv.Length;

                    for(int l = 0; l < k; l++) {
                        PointVector pvl = arguments[l] as PointVector;

                        if(length != pvl.Length)
                            continue;

                        bool equal = true;
                        for(int i = 0; i < length; i++)
                            if(pv[i].X != pvl[i].X || pv[i].Y != pvl[i].Y) {
                                equal = false;
                                break;
                            }
                        if(equal)
                            return false;
                    }
                }
                return true;
            }

            else if(item is string) {
                for(int k = 1; k < count; k++) {
                    string s = (string)arguments[k];

                    for(int l = 0; l < k; l++)
                        if(s == (string)arguments[l])
                            return false;
                }
                return true;
            }

            return null;
        }

        private const string name = "!=";
    }
}

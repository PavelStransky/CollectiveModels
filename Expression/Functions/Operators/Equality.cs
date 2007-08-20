using System.Collections;
using System.Text;
using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Operator of equality
    /// </summary>
    public class Equality: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpEquality; } }
        public override OperatorPriority Priority { get { return OperatorPriority.ComparePriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, true, true, false, Messages.PValue, Messages.PValueDescription, null,
                typeof(int), typeof(double), typeof(Vector), typeof(Matrix), typeof(PointD), typeof(PointVector), typeof(string));

            this.AddCompatibility(typeof(int), typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int[] lengths = this.GetTypesLength(arguments, false);

            object item = arguments[0];
            arguments.RemoveAt(0);

            if(lengths[0] > 0 && lengths[2] > 0) {  // Double i int hodnoty
                double d = (item is int) ? (int)item : (double)item;

                    foreach(object o in arguments)
                        if(o is double) {
                            if((double)o != d)
                                return false;
                        }
                        else if(o is int) {
                            if((int)o != d)
                                return false;
                        }

                    return true;
            }

            else if(item is int) {
                int i = (int)item;
                foreach(int o in arguments)
                    if(o != i)
                        return false;
                return true;
            }

            else if(item is double) {
                double d = (double)item;
                foreach(double o in arguments)
                    if(o != d)
                        return false;
                return true;
            }

            else if(item is Vector) {
                Vector v = item as Vector;
                int length = v.Length;

                foreach(Vector o in arguments) {
                    if(o.Length != length)
                        return false;
                    for(int i = 0; i < length; i++)
                        if(o[i] != v[i])
                            return false;
                }

                return true;
            }

            else if(item is Matrix) {
                Matrix m = item as Matrix;
                int lengthx = m.LengthX;
                int lengthy = m.LengthY;

                foreach(Matrix o in arguments) {
                    if(o.LengthX != lengthx || o.LengthY != lengthy)
                        return false;
                    for(int i = 0; i < lengthx; i++)
                        for(int j = 0; j < lengthy; j++)
                            if(o[i, j] != m[i, j])
                                return false;
                }

                return true;
            }

            else if(item is PointD) {
                PointD p = (PointD)item;

                foreach(PointD o in arguments)
                    if(o.X != p.X || o.Y != p.Y)
                        return false;

                return true;
            }

            else if(item is PointVector) {
                PointVector pv = (PointVector)item;
                int length = pv.Length;

                foreach(PointVector o in arguments) {
                    if(o.Length != length)
                        return false;
                    for(int i = 0; i < length; i++)
                        if(o[i].X != pv[i].X || o[i].Y != pv[i].Y)
                            return false;
                }

                return true;
            }

            else if(item is string) {
                string s = (string)item;

                foreach(string o in arguments)
                    if(s != o)
                        return false;

                return true;
            }

            return null;
        }

        private const string name = "==";
    }
}

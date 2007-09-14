using System.Collections;
using System.Text;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Operator power, items of vectors and matrices is multiplied among one another
    /// </summary>
    public class PowerItem: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpPowerItem; } }
        public override OperatorPriority Priority { get { return OperatorPriority.PowerPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PRoot, Messages.PRootDescription, null,
                typeof(int), typeof(double), typeof(Vector), typeof(Matrix),
                typeof(LongNumber), typeof(LongFraction));
            this.SetParam(1, true, true, false, Messages.PExponent, Messages.PExponentDescription, null,
                typeof(int), typeof(double), typeof(Vector), typeof(Matrix));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            this.GetTypesLength(arguments, true);

            object left = arguments[0];
            object right = arguments[1];

            if(left is int) {
                if(right is int)
                    return (int)System.Math.Pow((int)left, (int)right);

                else if(right is double)
                    return System.Math.Pow((int)left, (double)right);

                else if(right is Vector) {
                    Vector v = (Vector)right;
                    int length = v.Length;

                    Vector result = new Vector(length);
                    for(int i = 0; i < length; i++)
                        result[i] = System.Math.Pow((int)left, v[i]);

                    return result;
                }

                else if(right is Matrix) {
                    Matrix m = (Matrix)right;
                    int lengthx = m.LengthX;
                    int lengthy = m.LengthY;

                    Matrix result = new Matrix(lengthx, lengthy);
                    for(int i = 0; i < lengthx; i++)
                        for(int j = 0; j < lengthy; j++)
                            result[i, j] = System.Math.Pow((int)left, m[i, j]);

                    return result;
                }
            }

            else if(left is double) {
                if(right is int)
                    return System.Math.Pow((double)left, (int)right);

                else if(right is double)
                    return System.Math.Pow((double)left, (double)right);

                else if(right is Vector) {
                    Vector v = (Vector)right;
                    int length = v.Length;

                    Vector result = new Vector(length);
                    for(int i = 0; i < length; i++)
                        result[i] = System.Math.Pow((double)left, v[i]);

                    return result;
                }
                else if(right is Matrix) {
                    Matrix m = (Matrix)right;
                    int lengthx = m.LengthX;
                    int lengthy = m.LengthY;

                    Matrix result = new Matrix(lengthx, lengthy);
                    for(int i = 0; i < lengthx; i++)
                        for(int j = 0; j < lengthy; j++)
                            result[i, j] = System.Math.Pow((double)left, m[i, j]);

                    return result;
                }
            }

            else if(left is LongNumber) {
                if(right is int)
                    return (left as LongNumber).Power((int)right);
            }

            else if(left is LongFraction) {
                if(right is int)
                    return (left as LongFraction).Power((int)right);
            }

            else if(left is Vector) {
                Vector v = (Vector)left;
                int length = v.Length;
                Vector result = new Vector(length);

                if(right is int) {
                    for(int i = 0; i < length; i++)
                        result[i] = System.Math.Pow(v[i], (int)right);
                    return result;
                }

                else if(right is double) {
                    for(int i = 0; i < length; i++)
                        result[i] = System.Math.Pow(v[i], (double)right);
                    return result;
                }

                else if(right is Vector) {
                    for(int i = 0; i < length; i++)
                        result[i] = System.Math.Pow(v[i], ((Vector)right)[i]);
                    return result;
                }
            }

            else if(left is Matrix) {
                Matrix m = (Matrix)right;
                int lengthx = m.LengthX;
                int lengthy = m.LengthY;
                Matrix result = new Matrix(lengthx, lengthy);

                if(right is int) {
                    for(int i = 0; i < lengthx; i++)
                        for(int j = 0; j < lengthy; j++)
                            result[i, j] = System.Math.Pow(m[i, j], (int)right);
                    return result;
                }

                else if(right is double) {
                    for(int i = 0; i < lengthx; i++)
                        for(int j = 0; j < lengthy; j++)
                            result[i, j] = System.Math.Pow(m[i, j], (double)right);
                    return result;
                }

                else if(right is Matrix) {
                    for(int i = 0; i < lengthx; i++)
                        for(int j = 0; j < lengthy; j++)
                            result[i, j] = System.Math.Pow(m[i, j], ((Matrix)right)[i, j]);
                    return result;
                }
            }

            this.BadTypeCompatibility(left, right);
            return null;
        }

        private const string name = "^^";
    }
}

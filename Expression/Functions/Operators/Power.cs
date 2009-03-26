using System.Collections;
using System.Text;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Operator power
    /// </summary>
    public class Power: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpPower; } }
        public override OperatorPriority Priority { get { return OperatorPriority.PowerPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PRoot, Messages.PRootDescription, null,
                typeof(int), typeof(double), typeof(Vector), typeof(Matrix), typeof(string), 
                typeof(LongNumber), typeof(LongFraction));
            this.SetParam(1, true, true, false, Messages.PExponent, Messages.PExponentDescription, null,
                typeof(int), typeof(double), typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object left = arguments[0];
            object right = arguments[1];

            if(left is int) {
                if(right is int)
                    return (int)System.Math.Pow((int)left, (int)right);
                else if(right is double)
                    return System.Math.Pow((int)left, (double)right);
                else if(right is Vector) {
                    Vector v = right as Vector;
                    int length = v.Length;

                    Vector result = new Vector(length);
                    for(int i = 0; i < length; i++)
                        result[i] = System.Math.Pow((int)left, v[i]);

                    return result;
                }
            }

            else if(left is double) {
                if(right is int)
                    return System.Math.Pow((double)left, (int)right);
                else if(right is double)
                    return System.Math.Pow((double)left, (double)right);
                else if(right is Vector) {
                    Vector v = right as Vector;
                    int length = v.Length;

                    Vector result = new Vector(length);
                    for(int i = 0; i < length; i++)
                        result[i] = System.Math.Pow((double)left, v[i]);

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
                if(right is int) {
                    if(((int)right % 2) == 0)
                        return System.Math.Pow((Vector)left * (Vector)right, (int)right / 2);
                    else
                        return System.Math.Pow((Vector)left * (Vector)right, (int)right / 2) * (Vector)left;
                }
            }

            else if(left is Matrix) {
                if(right is int) {
                    Matrix m = (Matrix)left;
                    Matrix result = Matrix.Unit(m.Length);

                    for(int i = 0; i < (int)right; i++)
                        result *= m;

                    return result;
                }
            }

            else if(left is string) {
                if(right is int) {
                    StringBuilder s = new StringBuilder();
                    for(int i = 0; i < (int)right; i++)
                        s.Append(left);
                    return s.ToString();
                }
            }

            this.BadTypeCompatibility(left, right);
            return null;
        }

        private const string name = "^";
    }
}

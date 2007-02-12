using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Z vektoru èi øady odstraní hodnoty NaN, Infinity
    /// </summary>
    public class RemoveBadPoints: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            if(item is Vector) {
                Vector v = item as Vector;

                int index = 0;
                int length = v.Length;
                Vector result = new Vector(length);
                for(int i = 0; i < length; i++)
                    if(!double.IsNaN(v[i]) && !double.IsInfinity(v[i]))
                        result[index++] = v[i];
                result.Length = index;

                return result;
            }
            else if(item is PointVector) {
                PointVector pv = item as PointVector;

                int index = 0;
                int length = pv.Length;
                PointVector result = new PointVector(length);
                for(int i = 0; i < length; i++) {
                    PointD p = pv[i];
                    if(!double.IsNaN(p.X) && !double.IsInfinity(p.X)
                        && !double.IsNaN(p.Y) && !double.IsInfinity(p.Y))
                        result[index++] = p;
                }
                result.Length = index;

                return result;
            }

            else if(item is TArray)
                return this.EvaluateArray(depth, item as TArray, arguments);

            else
                return this.BadTypeError(item, 0);
        }

        private const string help = "Z vektoru nebo pointvectoru odstraní body NaN, Infinity";
        private const string parameters = "Vector | PointVector";
    }
}
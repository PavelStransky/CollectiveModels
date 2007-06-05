using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Z argument� funkce vytvo�� vektor
    /// </summary>
    public class FnVector: FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.VectorHelp; } }
        public override string Parameters { get { return Messages.VectorParameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            int count = evaluatedArguments.Count;
            for(int i = 0; i < count; i++) {
                this.CheckArgumentsType(evaluatedArguments, i, true, 
                    typeof(TArray), typeof(double), typeof(Vector), typeof(Matrix), typeof(List), typeof(int));
            }
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;
            int i = 0;

            ArrayList items = new ArrayList();

            for(i = 0; i < count; i++) {
                object item = arguments[i];

                if(item is int)
                    items.Add((double)(int)item);

                else if(item is double)
                    items.Add(item);

                else if(item is Vector) {
                    Vector v = item as Vector;
                    int length = v.Length;

                    for(int j = 0; j < length; j++)
                        items.Add(v[j]);
                }

                else if(item is Matrix) {
                    Matrix m = item as Matrix;
                    int lengthX = m.LengthX;
                    int lengthY = m.LengthY;

                    for(int j = 0; j < lengthX; j++)
                        for(int k = 0; k < lengthY; k++)
                            items.Add(m[j, k]);
                }

                else if(item is TArray) {
                    TArray a = item as TArray;
                    a.ResetEnumerator();

                    foreach(object o in a) {
                        ArrayList newarg = new ArrayList();
                        newarg.Add(o);
                        Vector v = this.EvaluateFn(guider, newarg) as Vector;
                        int length = v.Length;

                        for(int j = 0; j < length; j++)
                            items.Add(v[j]);
                    }
                }

                else if(item is List) {
                    List l = item as List;

                    foreach(object o in l) {
                        ArrayList newarg = new ArrayList();
                        newarg.Add(o);
                        Vector v = this.EvaluateFn(guider, newarg) as Vector;
                        int length = v.Length;

                        for(int j = 0; j < length; j++)
                            items.Add(v[j]);
                    }
                }
            }

            count = items.Count;
            Vector result = new Vector(count);
            i = 0;

            foreach(double d in items)
                result[i++] = d;

            return result;
        }

        private const string name = "vector";
    }
}

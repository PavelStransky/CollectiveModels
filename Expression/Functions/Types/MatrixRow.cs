using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Given vectors put onto rows of a matrix
    /// </summary>
    public class MatrixRow : Fnc {
        public override string Help { get { return Messages.HelpMatrixRow; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);

            this.SetParam(0, true, true, false, Messages.PMatrixRow, Messages.PMatrixRowDescription, null,
                typeof(TArray), typeof(Vector), typeof(List));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;
            int i = 0;

            ArrayList items = new ArrayList();
            int length = -1;

            for(i = 0; i < count; i++) {
                object item = arguments[i];

                if(item is Vector) {
                    Vector v = item as Vector;
                    int l = v.Length;

                    if(length < 0)
                        length = l;

                    if(length != l)
                        throw new FunctionDefinitionException(Messages.EMNotEqualLength);

                    items.Add(v);
                }

                else if(item is TArray) {
                    TArray a = item as TArray;
                    int l = a.Length;
                    Type t = a.GetItemType();

                    a.ResetEnumerator();

                    if(t == typeof(Vector)) {
                        foreach(Vector v in a) {
                            l = v.Length;

                            if(length < 0)
                                length = l;

                            if(length != l)
                                throw new FunctionDefinitionException(Messages.EMNotEqualLength);

                            items.Add(v);
                        }
                    }
                    else {
                        if(length < 0)
                            length = l;

                        if(length != l)
                            throw new FunctionDefinitionException(Messages.EMNotEqualLength);

                        Vector v = new Vector(length);
                        int j = 0;

                        if(t == typeof(int))
                            foreach(int x in a)
                                v[j++] = x;
                        else if(t == typeof(double))
                            foreach(double d in a)
                                v[j++] = d;
                        else
                            this.BadTypeError(a[0], i);

                        items.Add(v);
                    }
                }

                else if(item is List) {
                    List li = item as List;
                    int l = li.Count;

                    Type t = li.CheckOneType();

                    if(t == typeof(Vector)) {
                        foreach(Vector v in li) {
                            l = v.Length;

                            if(length < 0)
                                length = l;

                            if(length != l)
                                throw new FunctionDefinitionException(Messages.EMNotEqualLength);

                            items.Add(v);
                        }
                    }
                    else {
                        if(length < 0)
                            length = l;

                        if(length != l)
                            throw new FunctionDefinitionException(Messages.EMNotEqualLength);

                        Vector v = new Vector(length);
                        int j = 0;

                        if(t == typeof(int))
                            foreach(int x in li)
                                v[j++] = x;
                        else if(t == typeof(double))
                            foreach(double d in li)
                                v[j++] = d;
                        else
                            this.BadTypeError(li[0], i);

                        items.Add(v);
                    }
                }
            }

            count = items.Count;
            Matrix result = new Matrix(count, length);

            i = 0;
            foreach(Vector d in items)
                result.SetRowVector(i++, d);

            return result;
        }
    }
}

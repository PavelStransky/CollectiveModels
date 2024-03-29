using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates new object of a given type
    /// </summary>
    public class FnNew: Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpNew; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, true, true, false, Messages.PType, Messages.PTypeDescription, null);
        }
        
        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return this.EvaluateOne(guider, arguments, 0);
        }

        private object EvaluateOne(Guider guider, ArrayList arguments, int start) {
            Type t;

            try {
                t = Atom.ParseType(arguments[start] as string);
            }
            catch(Exception e) {
                // Nepoda�ilo se ur�it typ prom�nn�
                throw new FncException(this, e.Message);
            }

            if(t == typeof(int)) {
                int i = 0;
                this.CheckArgumentsMaxNumber(arguments, start + 2);
                return i;
            }

            else if(t == typeof(double)) {
                double d = 0.0;
                this.CheckArgumentsMaxNumber(arguments, start + 1);
                return d;
            }

            else if(t == typeof(List)) {
                this.CheckArgumentsMaxNumber(arguments, start + 1);
                return new List();
            }

            else if(t == typeof(Vector)) {
                this.CheckArgumentsMaxNumber(arguments, start + 2);
                this.CheckArgumentsType(arguments, start + 1, false, typeof(int));
                return new Vector((int)arguments[start + 1]);
            }

            else if(t == typeof(PointVector)) {
                this.CheckArgumentsMaxNumber(arguments, start + 2);
                this.CheckArgumentsType(arguments, start + 1, false, typeof(int));
                return new PointVector((int)arguments[start + 1]);
            }

            else if(t == typeof(Matrix)) {
                this.CheckArgumentsMaxNumber(arguments, start + 3);
                this.CheckArgumentsType(arguments, start + 1, false, typeof(int));
                int lengthX = (int)arguments[start + 1];
                int lengthY = lengthX;

                if(arguments.Count > start + 2) {
                    this.CheckArgumentsType(arguments, start + 2, false, typeof(int));
                    lengthY = (int)arguments[start + 2];
                }

                return new Matrix(lengthX, lengthY);
            }

            else if(t == typeof(TArray)) {
                int count = arguments.Count;

                for(int i = start + 1; i < count; i++) {
                    this.CheckArgumentsType(arguments, i, false, typeof(string), typeof(int));
                    if(arguments[i] is string) {
                        count = i - start - 1;
                        break;
                    }
                }

                int[] index = new int[count];

                for(int i = 0; i < count; i++)
                    index[i] = (int)arguments[i + start + 1];

                object fo = this.EvaluateOne(guider, arguments, start + count + 1);

                TArray result = new TArray(fo.GetType(), index);
                int[] rindex = (int[])result.StartEnumIndex.Clone();

                do {
                    if(fo is ICloneable)
                        result[rindex] = (fo as ICloneable).Clone();
                    else
                        result[rindex] = fo;
                } while(TArray.MoveNext(count, rindex, result.StartEnumIndex, result.EndEnumIndex));

                return result;
            }

            else
                return null;
        }

        private const string name = "new";
    }
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvo�� nov� objekt podle typu
    /// </summary>
    public class FnNew: FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return this.EvaluateOne(guider, arguments, 0);
        }

        private object EvaluateOne(Guider guider, ArrayList arguments, int start) {
            Type t = Atom.ParseType(arguments[start] as string);

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

            else if(t == typeof(Vector)) {
                this.CheckArgumentsMaxNumber(arguments, start + 2);
                this.CheckArgumentsType(arguments, start + 1, typeof(int));
                return new Vector((int)arguments[start + 1]);
            }

            else if(t == typeof(Matrix)) {
                this.CheckArgumentsMaxNumber(arguments, start + 3);
                this.CheckArgumentsType(arguments, start + 1, typeof(int));
                int lengthX = (int)arguments[start + 1];
                int lengthY = lengthX;

                if(arguments.Count > start + 2) {
                    this.CheckArgumentsType(arguments, start + 2, typeof(int));
                    lengthY = (int)arguments[start + 2];
                }

                return new Matrix(lengthX, lengthY);
            }

            else if(t == typeof(TArray)) {
                int count = arguments.Count;

                for(int i = start + 1; i < count; i++) {
                    this.CheckArgumentsType(arguments, i, typeof(string), typeof(int));
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
                    result[rindex] = (fo as ICloneable).Clone();
                } while(TArray.MoveNext(count, rindex, result.StartEnumIndex, result.EndEnumIndex));

                return result;
            }

            else
                return null;
        }

        private const string name = "new";
        private const string help = "Z argument� funkce vytvo�� �adu (Array)";
        private const string parameters = "dimenze1 (int)[; dimenze2 (int) ...]";
    }
}

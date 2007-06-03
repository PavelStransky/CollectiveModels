using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Sum all elements
    /// </summary>
    public class Sum : SumFn {
        public override string Help { get { return Messages.SumHelp; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is int || item is double)
                return item;

            else if(item is Vector)
                return (item as Vector).Sum();

            else if(item is Matrix)
                return (item as Matrix).Sum();

            else if(item is TArray) {
                TArray a = item as TArray;
                int resultI = 0;
                double resultD = 0.0;

                a.ResetEnumerator();
                foreach(object o in a) {
                    ArrayList newarg = new ArrayList();
                    newarg.Add(o);

                    object r = this.EvaluateFn(guider, newarg);
                    if(r is int)
                        resultI += (int)r;
                    else if(r is double)
                        resultD += (double)r;
                }

                if(resultD == 0)
                    return resultI;
                else
                    return resultI + resultD;
            }

            return 0;
        }
    }
}

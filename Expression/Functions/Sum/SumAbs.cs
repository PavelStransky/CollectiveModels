using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Sum absolute values of all elements
    /// </summary>
    public class SumAbs: SumFn {
        public override string Help { get { return Messages.HelpSumAbs; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is int)
                return System.Math.Abs((int)item);
                
            else if(item is double)
                return System.Math.Abs((double)item);

            else if(item is Vector)
                return (item as Vector).SumAbs();

            else if(item is Matrix)
                return (item as Matrix).SumAbs();

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

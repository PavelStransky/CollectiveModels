using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Maximal common divisor of numbers
    /// </summary>
    public class MCD: Fnc {
        public override string Help { get { return Messages.HelpMCD; } }

        protected override void CreateParameters() {
            this.SetNumParams(2, true);
            this.SetParam(0, true, true, false, Messages.PValue, Messages.PValueDescription, null, typeof(int), typeof(LongNumber));
            this.SetParam(1, true, true, false, Messages.PValue, Messages.PValueDescription, null, typeof(int), typeof(LongNumber));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object o1 = arguments[0];
            object o2 = arguments[1];

            object o = this.CalculateOne(o1, o2);

            int count = arguments.Count;
            int i = 2;

            while(i < count) {
                o1 = arguments[i];
                o2 = o;
                o = this.CalculateOne(o1, o2);
                i++;
            }

            return o;
        }

        /// <summary>
        /// Calculates one MCD using right types
        /// </summary>
        private object CalculateOne(object o1, object o2) {
            if(o1 is int) {
                if(o2 is int)
                    return this.MCDI((int)o1, (int)o2);
                else
                    return LongNumber.MCD((int)o1, (LongNumber)o2);
            }
            else {
                if(o2 is int)
                    return LongNumber.MCD((LongNumber)o1, (int)o2);
                else
                    return LongNumber.MCD((LongNumber)o1, (LongNumber)o2);
            }
        }

        /// <summary>
        /// Maximal common divisor of two integer numbers
        /// </summary>
        private int MCDI(int i1, int i2) {
            i1 = System.Math.Abs(i1);
            i2 = System.Math.Abs(i2);

            if(i2 > i1) {
                int i = i2;
                i2 = i1;
                i1 = i;
            }

            while(i2 != 0) {
                int r = i1 % i2;

                i1 = i2;
                i2 = r;
            }

            return i1;
        }
    }
}

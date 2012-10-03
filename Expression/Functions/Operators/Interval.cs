using System.Collections;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Interval operator
    /// </summary>
    public class Interval: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpTimes; } }
        public override OperatorPriority Priority { get { return OperatorPriority.IntervalPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PStartingPoint, Messages.PStartingPointDetail, null, typeof(int));
            this.SetParam(1, true, true, false, Messages.PEndingPoint, Messages.PEndingPointDetail, null, typeof(int));
            this.SetParam(2, false, true, false, Messages.PStep, Messages.PStepDescription, 1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int start = (int)arguments[0];
            int end = (int)arguments[1];
            int step = (int)arguments[2];

            if(step > 0) {
                int num = System.Math.Max(0, (end - start)) / step + 1;
                TArray result = new TArray(typeof(int), num);

                int i = 0;
                while(start <= end) {
                    result[i++] = start;
                    start += step;
                }

                return result;
            }
            else if(step < 0) {
                int num = System.Math.Max(0, (start - end)) / (-step) + 1;
                TArray result = new TArray(typeof(int), num);

                int i = 0;
                while(start >= end) {
                    result[i++] = start;
                    start += step;
                }

                return result;
            }

            return null;
        }

        private const string name = ":";
    }
}

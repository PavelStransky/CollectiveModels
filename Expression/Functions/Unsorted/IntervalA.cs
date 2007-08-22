using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates points for interval (as array)
    /// </summary>
    public class IntervalA: IntervalV {
        public override string Help { get { return Messages.HelpIntervalA; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int num = (int)arguments[2];
            DiscreteInterval di = new DiscreteInterval((double)arguments[0], (double)arguments[1], num);
            TArray a = new TArray(typeof(double), num);

            for(int i = 0; i < num; i++)
                a[i] = di.GetX(i);

            return a;
        }
    }
}

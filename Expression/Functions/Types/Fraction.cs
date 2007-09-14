using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Exact fraction
    /// </summary>
    public class FnFraction: Fnc {
        public override string Help { get { return Messages.HelpFraction; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PNumerator, Messages.PNumeratorDescription, null,
                typeof(int), typeof(LongNumber));
            this.SetParam(1, false, true, false, Messages.PDenominator, Messages.PDenominatorDescription, new LongNumber(1),
                typeof(int), typeof(LongNumber));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LongNumber n = (arguments[0] is int) ? new LongNumber((int)arguments[0]) : (arguments[0] as LongNumber);
            LongNumber d = (arguments[1] is int) ? new LongNumber((int)arguments[1]) : (arguments[1] as LongNumber);

            return new LongFraction(n, d);
        }

        private const string name = "fraction";
    }
}

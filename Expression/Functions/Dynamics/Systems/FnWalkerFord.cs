using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates WalkerFord class
    /// </summary>
    public class FnWalkerFord : Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpWalkerFord; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, false, true, true, Messages.PA, Messages.PADescription, 0.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PB, Messages.PBDescription, 0.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PMaxJ, Messages.PMaxJDescription, 5.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double a = (double)arguments[0];
            double b = (double)arguments[1];
            double maxJ = (double)arguments[2];
            return new WalkerFord(a, b, maxJ);
        }

        private const string name = "walkerford";
    }
}
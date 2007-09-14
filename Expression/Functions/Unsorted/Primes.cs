using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Primes
    /// </summary>
    public class FnPrimes: Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpPrimes; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PCount, Messages.PCountDescription, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return new TArray(Primes.GetPrimes((int)arguments[0]));
        }

        private const string name = "primes";
    }
}
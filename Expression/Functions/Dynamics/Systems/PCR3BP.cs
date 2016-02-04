using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates ThreeBody class
    /// </summary>
    public class FnPCR3BP : Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpPCR3BP; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, false, true, true, Messages.PMass, Messages.PMassDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double mu = (double)arguments[0];
            return new PCR3BP(mu);
        }

        private const string name = "pcr3bp";
    }
}
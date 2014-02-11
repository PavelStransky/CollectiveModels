using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For given dynamical system returns all local extreemes and energy at them
    /// </summary>
    public class Extreemes: Fnc {
        public override string Help { get { return Messages.HelpExtreemes; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PCW, Messages.PCWDescription, null, typeof(ClassicalCW));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            ClassicalCW cw = arguments[0] as ClassicalCW;
            return cw.Extreemes();
        }
    }
}

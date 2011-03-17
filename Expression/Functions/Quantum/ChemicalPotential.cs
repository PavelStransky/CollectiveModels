using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the chemical potential of the Strutinsky method for a given mass number
    /// </summary>
    public class ChemicalPotential: Fnc {
        public override string Help { get { return Messages.HelpChemicalPotential; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PStrutinsky, Messages.PStrutinskyDescription, null, typeof(Strutinsky));
            this.SetParam(1, true, true, false, Messages.PMassNumber, Messages.PMassNumberDescription, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Strutinsky s = arguments[0] as Strutinsky;
            return s.ChemicalPotential((int)arguments[1]);
        }
    }
}
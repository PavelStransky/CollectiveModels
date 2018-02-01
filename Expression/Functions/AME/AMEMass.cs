using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns a matrix with masses of known isotopes
    /// </summary>
    public class AMEMass : Fnc {
        public override string Help { get { return Messages.HelpAMEMass; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PAME, Messages.PAMEDescription, null, typeof(AME));
            this.SetParam(1, false, true, false, Messages.PAMEExperiment, Messages.PAMEExperimentDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            AME ame = arguments[0] as AME;
            bool exp = (bool)arguments[1];

            List result = new List();
            Matrix m =  ame.Mass(exp);
            result.Add(m);
            result.Add(ame.BindingEnergy(exp));
            result.Add(ame.MassExcess(exp));

            if(guider != null)
                guider.WriteLine("Number of elements: " + m.NumNonzeroItems());

            return result;
        }
    }
}

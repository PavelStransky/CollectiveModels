using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.PT;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns sum of logarithms of differences between E_i and other energies
    /// </summary>
    public class PTSumLn: Fnc {
        public override string Help { get { return Messages.HelpPTSumLn; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PPT, Messages.PPTDescription, null, typeof(PT1));
            this.SetParam(1, false, true, false, Messages.PEVIndex, Messages.PEVIndexDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PT1 pt = arguments[0] as PT1;
            int i = (int)arguments[1];

            return pt.SumLn(i);
        }
    }
}
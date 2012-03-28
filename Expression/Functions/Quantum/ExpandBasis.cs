using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Expand a specified eigenvector in basis components
    /// </summary>
    public class ExpandBasis: Fnc {
        public override string Help { get { return Messages.HelpExpandBasis; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PLipkin, Messages.PLipkinDescription, null, typeof(LipkinFactorized));
            this.SetParam(1, true, true, false, Messages.PEVIndex, Messages.PEVIndexDescription, null, typeof(int));
            this.SetParam(2, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LipkinFactorized lipkin = arguments[0] as LipkinFactorized;
            int i = (int)arguments[1];
            double precision = (double)arguments[2];
            return lipkin.ExpandBasis(i, precision);
        }
    }
}
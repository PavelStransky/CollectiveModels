using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.IBM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Creates a ClassicalIBM class
    /// </summary>
    public class CIBM : FunctionDefinition {
        public override string Help { get { return Messages.CIBMHelp; } }

        protected override void CreateParameters() {
            this.NumParams(2);

            this.SetParam(0, true, true, true, Messages.PIBMEta, Messages.PIBMEtaDescription, null, typeof(double));
            this.SetParam(1, true, true, true, Messages.PIBMChi, Messages.PIBMChiDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
                double eta = (double)arguments[0];
                double chi = (double)arguments[1];

                return new ClassicalIBM(eta, chi);
        }
    }
}
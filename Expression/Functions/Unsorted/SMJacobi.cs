using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Jacobi matrix of the Standard Mapping
    /// </summary>
    public class SMJacobi : Fnc {
        public override string Help { get { return Messages.HelpSMJacobi; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, true, Messages.P1StandardMapping, Messages.P1StandardMappingDescription, null, typeof(double));
            this.SetParam(1, true, true, true, Messages.P3StandardMapping, Messages.P3StandardMappingDescription, null, typeof(double));
            this.SetParam(2, true, true, true, Messages.P4StandardMapping, Messages.P4StandardMappingDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double k = (double)arguments[0];
            double r = (double)arguments[1];
            double theta = (double)arguments[2];

            StandardMapping1 sm = new StandardMapping1(k);
            return sm.Jacobi(r, theta);
        }

        private const string name = "standardmapping";
    }
}
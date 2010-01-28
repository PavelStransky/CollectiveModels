using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For GCM system solves the equation Potential == Given energy
    /// </summary>
    public class PotentialRoots : Fnc {
        public override string Help { get { return Messages.HelpPotentialRoots; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(GCM));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, false, true, true, Messages.PGamma, Messages.PGammaDetail, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            GCM gcm = arguments[0] as GCM;
            double e = (double)arguments[1];
            double gamma = (double)arguments[2];
            return gcm.Roots(e, gamma);
        }
    }
}

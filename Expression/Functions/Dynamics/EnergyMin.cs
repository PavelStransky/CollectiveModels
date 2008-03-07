using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For given dynamical system returns the minimum possible energy
    /// </summary>
    public class EnergyMin: Fnc {
        public override string Help { get { return Messages.HelpEnergyMin; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(ClassicalGCM));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            ClassicalGCM cgcm = arguments[0] as ClassicalGCM;
            return cgcm.VBG(cgcm.ExtremalBeta(0).Min(), 0.0);
        }
    }
}

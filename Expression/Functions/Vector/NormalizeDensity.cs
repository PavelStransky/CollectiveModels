using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Normalizes the level density of a given vector into the form (0, ..., length - 1)
    /// (not unfolding)
    /// </summary>
    public class NormalizeDensity: Fnc {
        public override string Help { get { return Messages.HelpNormalizeDensity; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PEnergies, Messages.PEnergiesDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector e = (arguments[0] as Vector).Sort() as Vector;
            e -= e[0];
            e *= (e.Length - 1) / e.LastItem;
            return e;
        }
    }
}
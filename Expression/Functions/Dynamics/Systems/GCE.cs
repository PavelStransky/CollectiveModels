using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a system to check the Geometrical curvature method
    /// </summary>
    public class GCE : Fnc {
        public override string Help { get { return Messages.HelpGCE; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return new GeometricCounterExample((int)arguments[0]);
        }
    }
}
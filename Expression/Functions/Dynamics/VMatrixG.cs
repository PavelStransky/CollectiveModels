using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates matrix with value of the eigenvalue of cal{V} matrix
    /// </summary>
    public class VMatrixG: Fnc {
        public override string Help { get { return Messages.HelpSALIG; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(PavelStransky.GCM.GCM));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, true, true, false, Messages.PIntervalX, Messages.PIntervalXDescription, null, typeof(Vector));
            this.SetParam(3, true, true, false, Messages.PIntervalY, Messages.PIntervalYDescription, null, typeof(Vector));
            this.SetParam(4, false, true, false, Messages.PEigenValueIndex, Messages.PEigenValueIndexDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PavelStransky.GCM.GCM gcm = arguments[0] as PavelStransky.GCM.GCM;

            double e = (double)arguments[1];
            Vector vx = (Vector)arguments[2];
            Vector vy = (Vector)arguments[3];
            int ei = (int)arguments[4];

            return gcm.VMatrixG(e, vx, vy, ei);
        }
    }
}

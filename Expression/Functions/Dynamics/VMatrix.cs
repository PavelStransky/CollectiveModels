using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// cal_{V} matrix according to
    /// PRL 98, 234301 (2007), expression (27)
    /// </summary>
    public class VMatrix: Fnc {
        public override string Help { get { return Messages.HelpVMatrix; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(GCM));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, true, true, true, Messages.PX, Messages.PXDetail, null, typeof(double));
            this.SetParam(3, true, true, true, Messages.PY, Messages.PYDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            GCM gcm = arguments[0] as GCM;
            double e = (double)arguments[1];
            double x = (double)arguments[2];
            double y = (double)arguments[3];

            return gcm.VMatrix(e, x, y);
        }
    }
}

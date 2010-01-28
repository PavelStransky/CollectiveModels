using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Finds points of all avoided crossings for PT3
    /// </summary>
    public class AvoidedCrossings: Fnc {
        public override string Help { get { return Messages.HelpAvoidedCrossings; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PPT, Messages.PPTDescription, null, typeof(PT3));
            this.SetParam(1, true, true, false, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, null, typeof(int));
            this.SetParam(2, false, true, false, Messages.PPrecision, Messages.PPrecisionDescription, 10, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PT3 pt3 = (PT3)arguments[0];
            int maxE = (int)arguments[1];
            int precision = (int)arguments[2];

            PointVector[] crossings = pt3.AvoidedCrossings(maxE, precision, guider);
            return new TArray(crossings);
        }
    }
}
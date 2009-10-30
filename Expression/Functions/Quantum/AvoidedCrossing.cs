using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.PT;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Finds precise position of the avoided crossing of two levels (minimum of their distance)
    /// </summary>
    public class AvoidedCrossing: Fnc {
        public override string Help { get { return Messages.HelpAvoidedCrossing; } }

        protected override void CreateParameters() {
            this.SetNumParams(7);

            this.SetParam(0, true, true, false, Messages.PPT, Messages.PPTDescription, null, typeof(PT3));
            this.SetParam(1, true, true, false, Messages.P2AvoidedCrossing, Messages.P2AvoidedCrossingDescription, null, typeof(int));
            this.SetParam(2, true, true, false, Messages.P3AvoidedCrossing, Messages.P3AvoidedCrossingDescription, null, typeof(int));
            this.SetParam(3, true, true, true, Messages.P4AvoidedCrossing, Messages.P4AvoidedCrossingDescription, null, typeof(double));
            this.SetParam(4, true, true, true, Messages.P5AvoidedCrossing, Messages.P5AvoidedCrossingDescription, null, typeof(double));
            this.SetParam(5, true, true, false, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, null, typeof(int));
            this.SetParam(6, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 1E-14, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PT3 pt3 = (PT3)arguments[0];
            int n1 = (int)arguments[1];
            int n2 = (int)arguments[2];
            double b1 = (double)arguments[3];
            double b2 = (double)arguments[4];

            int maxE = (int)arguments[5];
            double precision = (double)arguments[6];

            return pt3.AvoidedCrossing(n1, n2, b1, b2, maxE, precision);
        }
    }
}
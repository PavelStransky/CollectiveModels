using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a time series of the standard mapping with given initial conditions
    /// </summary>
    public class FnStandardMapping: Fnc {
        public override string Help { get { return Messages.HelpStandardMapping; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, true, Messages.P1StandardMapping, Messages.P1StandardMappingDescription, null, typeof(double));
            this.SetParam(1, true, true, false, Messages.PTime, Messages.PTimeDescription, null, typeof(int));
            this.SetParam(2, true, true, true, Messages.P3StandardMapping, Messages.P3StandardMappingDescription, null, typeof(double));
            this.SetParam(3, true, true, true, Messages.P4StandardMapping, Messages.P4StandardMappingDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double k = (double)arguments[0];
            int time = (int)arguments[1];
            double r = (double)arguments[2];
            double theta = (double)arguments[3];

            StandardMapping sm = new StandardMapping(k);
            return sm.Compute(r, theta, time);
        }

        private const string name = "standardmapping";
    }
}
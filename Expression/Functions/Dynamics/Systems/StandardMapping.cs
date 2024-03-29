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
            this.SetNumParams(5);

            this.SetParam(0, true, true, true, Messages.P1StandardMapping, Messages.P1StandardMappingDescription, null, typeof(double));
            this.SetParam(1, true, true, true, Messages.P3StandardMapping, Messages.P3StandardMappingDescription, null, typeof(double));
            this.SetParam(2, true, true, true, Messages.P4StandardMapping, Messages.P4StandardMappingDescription, null, typeof(double));
            this.SetParam(3, true, true, false, Messages.PTime, Messages.PTimeDescription, null, typeof(int));
            this.SetParam(4, false, true, false, Messages.PModulo, Messages.PModuloDescription, true, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double k = (double)arguments[0];
            double r = (double)arguments[1];
            double theta = (double)arguments[2];
            int time = (int)arguments[3];
            bool modulo = (bool)arguments[4];

            StandardMapping1 sm = new StandardMapping1(k);
            return sm.Compute(r, theta, time, modulo);
        }

        private const string name = "standardmapping";
    }
}
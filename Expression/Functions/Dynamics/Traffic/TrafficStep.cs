using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Makes steps of the traffic system without returning the results
    /// </summary>
    public class TrafficStep: Fnc {
        public override string Help { get { return Messages.HelpTrafficStep; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PTraffic, Messages.PTrafficDescription, null, typeof(Traffic));
            this.SetParam(1, false, true, false, Messages.PTime, Messages.PTimeDescription, 1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Traffic t = arguments[0] as Traffic;
            int time = (int)arguments[1];
            t.Step(time);
            return t;
        }
    }
}
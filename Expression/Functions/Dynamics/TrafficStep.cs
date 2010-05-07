using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Makes a step of the traffic system
    /// </summary>
    public class TrafficStep: Fnc {
        public override string Help { get { return Messages.HelpTrafficStep; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PTraffic, Messages.PTrafficDescription, null, typeof(Traffic));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Traffic t = arguments[0] as Traffic;
            t.Step();
            return t.GetMatrix();
        }
    }
}
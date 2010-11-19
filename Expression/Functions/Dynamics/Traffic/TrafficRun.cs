using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Runs the traffic system for several time steps
    /// </summary>
    public class TrafficRun: Fnc {
        public override string Help { get { return Messages.HelpTrafficRun; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PTraffic, Messages.PTrafficDescription, null, typeof(Traffic));
            this.SetParam(1, false, true, false, Messages.PTime, Messages.PTimeDescription, 1, typeof(int));
            this.SetParam(2, false, true, false, Messages.PBoundaryCut, Messages.PBoundaryCutDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Traffic t = arguments[0] as Traffic;
            int time = (int)arguments[1];
            int boundary = (int)arguments[2];

            ArrayList r = t.Run(time, boundary);

            List result = new List();
            foreach(object o in r)
                result.Add(o);

            return result;
        }
    }
}
using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Sets the parameters of the traffic system
    /// </summary>
    public class TrafficParams: Fnc {
        public override string Help { get { return Messages.HelpTrafficParams; } }

        protected override void CreateParameters() {
            this.SetNumParams(7);
            this.SetParam(0, true, true, false, Messages.PTraffic, Messages.PTrafficDescription, null, typeof(Traffic));
            this.SetParam(1, false, true, false, Messages.P1TrafficParams, Messages.P1TrafficParamsDescription, -1, typeof(int));
            this.SetParam(2, false, true, false, Messages.P2TrafficParams, Messages.P2TrafficParamsDescription, -1, typeof(int));
            this.SetParam(3, false, true, false, Messages.P3TrafficParams, Messages.P3TrafficParamsDescription, -1, typeof(int));
            this.SetParam(4, false, true, false, Messages.P4TrafficParams, Messages.P4TrafficParamsDescription, -1, typeof(int));
            this.SetParam(5, false, true, false, Messages.P5TrafficParams, Messages.P5TrafficParamsDescription, -1, typeof(int));
            this.SetParam(5, false, true, false, Messages.P6TrafficParams, Messages.P6TrafficParamsDescription, -1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Traffic t = arguments[0] as Traffic;

            int sensorDistance = (int)arguments[1];
            int shortDistance = (int)arguments[2];
            int shortDistanceStopped = (int)arguments[3];
            int minGreen = (int)arguments[4];
            int maxTolerance = (int)arguments[5];
            int cutPlatoon = (int)arguments[6];

            t.SetParams(sensorDistance, shortDistance, shortDistanceStopped, minGreen, maxTolerance, cutPlatoon);
            return t;
        }
    }
}
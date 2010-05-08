using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Generates initial conditions for the traffic system
    /// </summary>
    public class TrafficIC: Fnc {
        public override string Help { get { return Messages.HelpTrafficIC; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PTraffic, Messages.PTrafficDescription, null, typeof(Traffic));
            this.SetParam(1, true, true, true, Messages.P1TrafficIC, Messages.P1TrafficICDescription, null, typeof(double), typeof(Vector));
            this.SetParam(2, false, true, false, Messages.P2TrafficIC, Messages.P2TrafficICDescription, "probability", typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Traffic t = arguments[0] as Traffic;

            double x = -1.0;
            double y = -1.0;
            if(arguments[2] is Vector) {
                Vector v = arguments[2] as Vector;
                x = v[0];
                if(v.Length > 1)
                    y = v[1];
            }
            else
                x = (double)arguments[1];

            if(y < 0.0)
                y = x;

            Traffic.ICType icType = (Traffic.ICType)Enum.Parse(typeof(Traffic.ICType), (string)arguments[2], true);
            
            t.InitialCondition(x, y, icType);
            return t;
        }
    }
}
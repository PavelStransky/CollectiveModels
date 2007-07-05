using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Calculates SALI dependence on time for given trajectory
    /// </summary>
    public class FnSALI: FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpSALI; } }

        protected override void CreateParameters() {
            this.NumParams(4);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, true, Messages.P2Poincare, Messages.P2PoincareDescription, null, typeof(Vector), typeof(double));
            this.SetParam(2, true, true, true, Messages.PTime, Messages.PTimeDescription, null, typeof(int));
            this.SetParam(3, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;

            Vector ic;

            // Trajectory is generated randomly
            if(arguments[1] is double) {
                double e = (double)arguments[1];
                ic = dynamicalSystem.IC(e);
            }
            else if(arguments[1] is Vector && (arguments[1] as Vector).Length / 2 == dynamicalSystem.DegreesOfFreedom)
                ic = (Vector)arguments[1];
            else
                return this.BadTypeError(arguments[1], 1);

            double time = (double)arguments[2];
            double precision = (double)arguments[3];

            SALI sali = new SALI(dynamicalSystem, precision);
            return sali.TimeDependence(ic, time);
        }

        private const string name = "sali";
    }
}

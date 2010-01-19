using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For given dynamical system and energy generates initial condition of a trajectory and returns it as Vector
    /// </summary>
    public class InitialCondition: Fnc {
        public override string Help { get { return Messages.HelpInitialCondition; } }

        protected override void CreateParameters() {
            this.SetNumParams(7);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, false, true, false, Messages.P4Poincare, Messages.P4PoincareDescription, null, typeof(Vector));
            this.SetParam(3, false, true, false, Messages.P5Poincare, Messages.P5PoincareDescription, null, typeof(int));
            this.SetParam(4, false, true, false, Messages.P6Poincare, Messages.P6PoincareDescription, null, typeof(int));
            this.SetParam(5, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, "normal", typeof(string));
            this.SetParam(6, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;
            double e = (double)arguments[1];

            if(arguments[2] == null)
                return dynamicalSystem.IC(e);

            else {
                Vector plane = (Vector)arguments[2];
                int i1 = (int)arguments[3];
                int i2 = (int)arguments[4];

                RungeKuttaMethods rkMethod = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[5], true);
                double precision = (double)arguments[6];

                PoincareSection ps = new PoincareSection(dynamicalSystem, precision, rkMethod);

                for(int i = 0; i < maxNumAttemps; i++ ) {
                    Vector ic = dynamicalSystem.IC(e);
                    if(ps.CrossPlane(plane, i1, i2, ic))
                        return ic;
                }

                throw new FncException(this, Messages.EMMaxIterationIC);
            }
        }

        // Maximální poèet pokusù, než bude vyvolána výjimka
        private const int maxNumAttemps = 20;
    }
}

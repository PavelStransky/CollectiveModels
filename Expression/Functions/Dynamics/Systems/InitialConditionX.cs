using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For given dynamical system and energy generates missing coordinate / momentum in the initial condition
    /// </summary>
    public class InitialConditionX: Fnc {
        public override string Help { get { return Messages.HelpInitialConditionX; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, false, true, false, Messages.PInitialCondition, Messages.PInitialConditionDescription, null, typeof(Vector));
            this.SetParam(3, false, true, false, Messages.PComponentIndex, Messages.POneOrientationDescription, false, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;
            double e = (double)arguments[1];
            Vector ic = (Vector)arguments[2];
            int i = (int)arguments[3];

            ic = ic.Clone() as Vector;
            ic[i] = double.NaN;
            if(dynamicalSystem.IC(ic, e))
                return ic;
            else
                return new Vector(0);
        }
    }
}

using System;
using System.Collections;

using PavelStransky.Systems;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Thermodynamical mean value of a Peres operator
    /// </summary>
    public class TDMeanOperator: Fnc {
        public override string Help { get { return Messages.HelpTDMeanOperator; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, true, Messages.PTemperature, Messages.PTemperatureDescription, null, typeof(double));
            this.SetParam(2, false, true, false, Messages.PPeresOperatorType, Messages.PPeresOperatorTypeDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double T = (double)arguments[1];
            int type = (int)arguments[2];
            return (arguments[0] as IQuantumSystem).EigenSystem.MeanOperator(T, type);
        }
    }
}

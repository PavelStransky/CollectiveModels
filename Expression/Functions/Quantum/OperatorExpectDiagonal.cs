using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Expectation value of the diagonal elements <m1 m2|L|m1 m2>
    /// </summary>
    public class OperatorExpectDiagonal: Fnc {
        public override string Help { get { return Messages.HelpOperatorExpectDiagonal; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PDoublePendulum, Messages.PDoublePendulumDescription, null, typeof(QuantumDP));
            this.SetParam(1, true, true, false, Messages.PEVIndex, Messages.PEVIndexDescription, null, typeof(Vector));
            this.SetParam(2, false, true, false, Messages.POperatorType, Messages.POperatorTypeDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            QuantumDP system = (QuantumDP)arguments[0];
            Vector basisIndex = (Vector)arguments[1];
            int operatorType = (int)arguments[2];

            return system.OperatorExpectDiagonal(basisIndex, operatorType);
        }
    }
}
using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the expectation value of the q operator in the Dicke model
    /// </summary>
    public class DickeQmn : Fnc {
        public override string Help { get { return Messages.HelpDickeQmn; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(QuantumDicke));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            QuantumDicke qd = arguments[0] as QuantumDicke;
            return qd.ExpectationValuePositionOperator(guider);
        }
    }
}
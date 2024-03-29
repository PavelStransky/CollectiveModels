using System.Collections;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Array evaluation of functions
    /// </summary>
    public class EvalArray: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpEvalArray; } }
        public override OperatorPriority Priority { get { return OperatorPriority.MaxPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, false, false, Messages.PCommands, Messages.PCommandsDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];
            bool oldArrayEvaluation = guider.ArrayEvaluation;
            guider.ArrayEvaluation = true;
            object result = this.EvaluateAtomObject(guider, item);
            guider.ArrayEvaluation = oldArrayEvaluation;

            return result;
        }

        private const string name = "#";
    }
}

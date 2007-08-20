using System.Collections;
using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Array evaluation of functions
    /// </summary>
    public class EvalArray: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpEvalArray; } }
        public override OperatorPriority Priority { get { return OperatorPriority.MaxPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, false, false, Messages.PFnName, Messages.PFnNameDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];
            guider.ArrayEvaluation = true;
            object result = Atom.EvaluateAtomObject(guider, item);
            guider.ArrayEvaluation = false;

            return result;
        }

        private const string name = "#";
    }
}

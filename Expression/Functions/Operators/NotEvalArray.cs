using System;
using System.Collections;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Disable the array evaluation of the following expression
    /// </summary>
    public class NotEvalArray: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpNotEvalArray; } }
        public override FncTypes FncType { get { return FncTypes.Operator; } }
        public override OperatorPriority Priority { get { return OperatorPriority.MaxPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, false, false, Messages.PCommands, Messages.PCommandsDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];
            bool oldArrayEvaluation = guider.ArrayEvaluation;
            guider.ArrayEvaluation = false;
            object result = this.EvaluateAtomObject(guider, item);
            guider.ArrayEvaluation = oldArrayEvaluation;

            return result;
        }

        protected override void CreateExamples() {
            StringBuilder s = new StringBuilder();
            s.AppendLine("a = new(\"array\"; 5; 5; \"double\");");
            s.AppendLine("#sin('deflate(a));");
            this.examples.Add(s.ToString());
        }

        private const string name = "'";
    }
}

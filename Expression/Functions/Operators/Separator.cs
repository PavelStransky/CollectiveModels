using System.Collections;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Separator operator
    /// </summary>
    public class Separator: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpSeparator; } }
        public override OperatorPriority Priority { get { return OperatorPriority.SeparatorPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, false, true, false, Messages.PCommands, Messages.PCommandsDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;

            if(count > 0)
                return arguments[count - 1];
            else
                return null;
        }

        private const string name = ";";
    }
}

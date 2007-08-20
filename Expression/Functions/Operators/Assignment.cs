using System.Collections;
using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Operátor pøiøazení
    /// </summary>
    public class Assignment: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpAssignment; } }
        public override OperatorPriority Priority { get { return OperatorPriority.AssignmentPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, false, false, Messages.P1Assignment, Messages.P1AssignmentDescription, null);
            this.SetParam(1, true, true, false, Messages.P2AssignmentDescription, Messages.P2Assignment, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            guider.Context.SetVariable(arguments[0] as string, arguments[1]);
            return arguments[1];
        }

        private const string name = "=";
    }
}

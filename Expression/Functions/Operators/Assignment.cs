using System.Collections;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
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
            object leftSide = arguments[0];
            this.result = arguments[1];

            if(this.result == null)
                throw new FncException(Messages.EMNullValue);

            if(this.result as ICloneable != null)
                this.result = (this.result as ICloneable).Clone();

            if(leftSide is Indexer) {
                (leftSide as Indexer).Evaluate(guider, this.AssignFn);
                return this.result;
            }
            else if(leftSide is string) {
                guider.Context.SetVariable(leftSide as string, this.result);
                return arguments[1];
            }
            else
                throw new FncException(Messages.EMBadAssignment, leftSide.GetType().FullName);
        }

        private object result;

        /// <summary>
        /// Pøiøazovací funkce
        /// </summary>
        /// <param name="o">Objekt pro pøiøazení</param>
        private object AssignFn(object o) {
            return this.result;
        }

        private const string name = "=";
    }
}

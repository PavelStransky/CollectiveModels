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
            this.SetNumParams(2, true);
            this.SetParam(0, true, false, false, Messages.P1Assignment, Messages.P1AssignmentDescription, null);
            this.SetParam(1, true, false, false, Messages.P2Assignment, Messages.P2AssignmentDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count - 1;

            this.result = this.EvaluateAtomObject(guider, arguments[count]);
            if(this.result == null)
                throw new FncException(this, Messages.EMNullValue);

            if(this.result as ICloneable != null)
                this.result = (this.result as ICloneable).Clone();

            for(int i = 0; i < count; i++) {
                object leftSide = arguments[i];
                if(leftSide is Indexer) 
                    (leftSide as Indexer).Evaluate(guider, this.AssignFn);
                
                else if(leftSide is string) 
                    guider.Context.SetVariable(leftSide as string, this.result);
                
                else
                    throw new FncException(this, Messages.EMBadAssignment, leftSide.GetType().FullName);
            }

            return this.result;
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

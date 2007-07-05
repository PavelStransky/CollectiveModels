using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Duration of evaluating of the expression
    /// </summary>
    public class FnTime : FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpTime; } }
        public override string Parameters { get { return Messages.TimeParameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) { }

        public override object Evaluate(Guider guider, ArrayList arguments, bool evaluateArray) {
            this.CheckArgumentsNumber(arguments, 1);

            DateTime startTime = DateTime.Now;
            base.Evaluate(guider, arguments, evaluateArray);
            return DateTime.Now - startTime;
        }

        private const string name = "time";
    }
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Calculates values of specified function and returns it as pointvector
    /// </summary>
    public class FnVal: FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpTime; } }

        public override object Evaluate(Guider guider, ArrayList arguments, bool evaluateArray) {
            this.CheckArgumentsNumber(arguments, 1);

            DateTime startTime = DateTime.Now;
            base.Evaluate(guider, arguments, evaluateArray);
            return DateTime.Now - startTime;
        }

        private const string name = "fnval";
    }
}

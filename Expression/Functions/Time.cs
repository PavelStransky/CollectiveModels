using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrac� aktu�ln� �as
    /// </summary>
    public class FnTime : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }
        public override string Name { get { return name; } }

        protected override ArrayList CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 0);
            return evaluatedArguments;
        }

        protected override object Evaluate(int depth, object item, ArrayList arguments) {
            return DateTime.Now;
        }

        private const string name = "time";
        private const string help = "Vrac� aktu�ln� �as";
        private const string parameters = "";
    }
}

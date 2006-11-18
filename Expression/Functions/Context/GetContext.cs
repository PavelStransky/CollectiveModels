using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vr�t� aktu�ln� kontext
    /// </summary>
    public class GetContext : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 0);
            return context;
        }

        private const string help = "Vr�t� aktu�ln� kontext.";
        private const string parameters = "";
    }
}

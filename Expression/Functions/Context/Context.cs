using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvo�� nov� kontext
    /// </summary>
    public class NewContext : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsMaxNumber(arguments, 1);

            Context result = new Context();
            context.OnNewContextRequest(new ContextEventArgs(result));

            if(arguments.Count > 0)
                Atom.EvaluateAtomObject(result, arguments[0]);

            return result;
        }

        private const string help = "Vytvo�� nov� kontext.";
        private const string parameters = "P��kaz nov�ho kontextu";
    }
}

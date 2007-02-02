using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Nastaví nový globální kontext
    /// </summary>
    public class SetContext : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 1);
            ArrayList args = this.EvaluateArguments(context, arguments, writer);
            this.CheckArgumentsType(args, 0, typeof(Context));

            Context c = args[0] as Context;
            context.OnEvent(new ContextEventArgs(ContextEventType.SetContext, c));
            return c;
        }

        private const string help = "Nastaví nový globální kontext";
        private const string parameters = "Kontext";
    }
}

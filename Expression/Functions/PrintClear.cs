using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vymaže writer
    /// </summary>
    public class PrintClear : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 0);

            if(writer != null)
                writer.Clear();

            return null;
        }

        private const string help = "Vymaže všechny výstupy";
        private const string parameters = "";
    }
}

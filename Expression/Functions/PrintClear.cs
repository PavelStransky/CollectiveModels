using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vyma�e writer
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

        private const string help = "Vyma�e v�echny v�stupy";
        private const string parameters = "";
    }
}

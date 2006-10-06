using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vypíše text (promìnnou) do writeru
    /// </summary>
    public class PrintLine : FunctionDefinition {
        private int counter = 0;
        private int maxCounter = 1;

        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsMaxNumber(arguments, 2);

            if(arguments.Count > 1) {
                this.CheckArgumentsType(arguments, 0, typeof(int));
                this.maxCounter = (int)arguments[1];
            }

            this.counter++;

            if(this.counter >= this.maxCounter) {
                object toPrint = arguments.Count == 0 ? string.Empty : Atom.EvaluateAtomObject(context, arguments[0]);
                if(writer != null)
                    writer.WriteLine(toPrint.ToString());

                this.counter = 0;
            }

            return this.counter;
        }

        private const string name = "print";
        private const string help = "Vypíše výsledek výrazu a zalomí øádku";
        private const string parameters = "výraz [; poèet volání, po kolika dojde k výpisu (int)]";
    }
}

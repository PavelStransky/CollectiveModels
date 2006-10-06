using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vrátí dobu výpètu pøíkazu
    /// </summary>
    public class FnTime : FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 1);

            DateTime startTime = DateTime.Now;
            Atom.EvaluateAtomObject(context, arguments[0]);

            return DateTime.Now - startTime;
        }

        private const string name = "time";
        private const string help = "Vrátí dobu výpoètu";
        private const string parameters = "výraz";
    }
}

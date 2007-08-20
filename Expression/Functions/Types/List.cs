using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Z argumentù funkce vytvoøí seznam
    /// </summary>
    public class FnList: FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string ParametersHelp { get { return parameters; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            List result = new List();

            foreach(object o in arguments)
                result.Add(o);

            return result;
        }

        private const string name = "list";
        private const string help = "Z argumentù funkce vytvoøí seznam (List)";
        private const string parameters = "prvky seznamu";
    }
}

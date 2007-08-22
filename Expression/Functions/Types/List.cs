using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Z argument� funkce vytvo�� seznam
    /// </summary>
    public class FnList: Fnc {
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
        private const string help = "Z argument� funkce vytvo�� seznam (List)";
        private const string parameters = "prvky seznamu";
    }
}

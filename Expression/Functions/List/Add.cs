using System;
using System.Collections;

using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Na konec seznamu pøidá prvek
    /// </summary>
    public class FnAdd: FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsMinNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(List));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            List result = null;

            foreach(object o in arguments) {
                if(result == null)
                    result = o as List;
                else
                    result.Add(o);
            }

            return result;
        }

        private const string name = "add";
        private const string help = "Na konec seznamu (List) pøidá prvek";
        private const string parameters = "Seznam (List)[; pøidávané prvky]";
    }
}

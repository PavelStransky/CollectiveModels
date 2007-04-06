using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Z kontextu vyzvedne promìnnou
    /// </summary>
    public class GetVar : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 2);

            this.CheckArgumentsType(evaluatedArguments, 0, typeof(Context));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Context c = arguments[0] as Context;
            string name = arguments[1] as string;
            return c[name];
        }

        private const string help = "Z kontextu vrátí urèenou promìnnou.";
        private const string parameters = "Kontext; název promìnné (string)";
    }
}

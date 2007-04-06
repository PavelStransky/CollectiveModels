using System;
using System.IO;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Z globálního kontextu vyzvedne promìnnou
    /// </summary>
    public class GetGlobalVar: FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        protected override void CheckArguments(ArrayList evaluatedArguments) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Import import = new Import(Context.GlobalContextFileName, true);
            Context c = import.Read() as Context;
            import.Close();

            return c[arguments[0] as string];
        }

        private const string help = "Z globálního kontextu vrátí urèenou promìnnou.";
        private const string parameters = "Název promìnné (string)";
    }
}

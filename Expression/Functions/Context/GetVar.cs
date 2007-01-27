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

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsMinNumber(arguments, 2);

            Context result = Atom.EvaluateAtomObject(context, arguments[0]) as Context;
            ArrayList args = new ArrayList(); args.Add(result); args.Add(arguments[1]);
            this.CheckArgumentsType(args, 0, typeof(Context));
            this.CheckArgumentsType(args, 1, typeof(string));

            string name = arguments[1] as string;
            return result[name];
        }

        private const string help = "Z kontextu vrátí urèenou promìnnou.";
        private const string parameters = "Kontext; název promìnné";
    }
}

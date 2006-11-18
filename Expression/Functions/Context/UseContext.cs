using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Pou�ije kontext k v�po�t�m
    /// </summary>
    public class UseContext : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 2);

            Context result = Atom.EvaluateAtomObject(context, arguments[0]) as Context;
            ArrayList args = new ArrayList(); args.Add(result);
            this.CheckArgumentsType(args, 0, typeof(Context));

            Atom.EvaluateAtomObject(result, arguments[1]);
            return result;
        }

        private const string help = "Pou�ije zadan� kontext k v�po�t�m.";
        private const string parameters = "Kontext; P��kazy";
    }
}

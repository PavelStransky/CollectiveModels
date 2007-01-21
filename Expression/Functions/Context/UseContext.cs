using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Použije kontext k výpoètùm
    /// </summary>
    public class UseContext : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsMinNumber(arguments, 2);

            Context result = Atom.EvaluateAtomObject(context, arguments[0]) as Context;
            ArrayList args = new ArrayList(); args.Add(result);
            this.CheckArgumentsType(args, 0, typeof(Context));

            if(arguments[1] != null)
                Atom.EvaluateAtomObject(result, arguments[1]);

            int count = arguments.Count;
            if(count > 2)
                for(int i = 2; i < count; i++)
                    if(arguments[i] is string && context.Contains(arguments[i] as string)) {
                        Variable v = context[arguments[i] as string];
                        result.SetVariable(v.Name, v.Item);
                    }

            return result;
        }

        private const string help = "Použije zadaný kontext k výpoètùm.";
        private const string parameters = "Kontext; Pøíkazy[; Promìnné kopírované z aktuálního kontextu]";
    }
}

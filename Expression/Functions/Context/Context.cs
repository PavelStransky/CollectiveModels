using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí nový kontext
    /// </summary>
    public class FnContext : FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            Context result = new Context();
            context.OnNewContextRequest(new ContextEventArgs(result));

            int count = arguments.Count;

            if(count > 1)
                for(int i = 1; i < count; i++)
                    if(arguments[i] is string && context.Contains(arguments[i] as string)) {
                        Variable v = context[arguments[i] as string];
                        result.SetVariable(v.Name, v.Item);
                    }

            if(count > 0 && arguments[0] != null)
                Atom.EvaluateAtomObject(result, arguments[0]);

            return result;
        }

        private const string name = "context";
        private const string help = "Vytvoøí nový kontext.";
        private const string parameters = "[Pøíkaz nového kontextu[; Promìnné kopírované z aktuálního kontextu]]";
    }
}

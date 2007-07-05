using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Creates a new context
    /// </summary>
    public class FnContext : FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpContext; } }

        protected override void CreateParameters() {
            this.NumParams(2, true);
            this.SetParam(0, false, false, false, Messages.PCommands, Messages.PContext1Description, null, typeof(string));
            this.SetParam(1, false, false, false, Messages.PVarName, Messages.P2ContextDescription, null, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Context result = new Context(guider.Context.Directory);
            guider.Context.OnEvent(new ContextEventArgs(ContextEventType.NewContext, result));

            int count = arguments.Count;

            for(int i = 1; i < count; i++) {
                if(arguments[i] == null)
                    continue;

                Variable v = guider.Context[arguments[i] as string];
                result.SetVariable(v.Name, v.Item);
            }

            if(count > 0 && arguments[0] != null) {
                Expression e = new Expression(arguments[0] as string);
                e.Evaluate(new Guider(result, guider));
            }

            return result;
        }

        private const string name = "context";
    }
}

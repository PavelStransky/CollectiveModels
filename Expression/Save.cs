using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vyšle požadavek o uložení souboru
    /// </summary>
    public class Save : FunctionDefinition {
        public override string Help { get { return help; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            ArrayList evaluatedArguments = this.EvaluateArguments(context, arguments, writer);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 1);

            if(evaluatedArguments.Count > 0) {
                this.CheckArgumentsType(evaluatedArguments, 0, typeof(string));
                context.OnEvent(new ContextEventArgs(ContextEventType.Save, evaluatedArguments[0] as string));
            }
            else
                context.OnEvent(new ContextEventArgs(ContextEventType.Save));

            return null;
        }

        private const string help = "Vyšle požadavek o ukonèení programu";
    }
}

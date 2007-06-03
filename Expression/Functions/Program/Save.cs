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
        public override string Use { get { return use; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsMaxNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments.Count > 0) 
                guider.Context.OnEvent(new ContextEventArgs(ContextEventType.Save, arguments[0] as string));
            else
                guider.Context.OnEvent(new ContextEventArgs(ContextEventType.Save));
            return null;
        }

        private const string help = "Vyšle požadavek o ukonèení programu";
        private const string use = "Jméno souboru (string)";
    }
}

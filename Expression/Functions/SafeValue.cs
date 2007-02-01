using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Z vlastn�ho kontextu vyzvedne; pokud prom�nn� neexistuje, vr�t� defaultn� hodnotu
    /// </summary>
    public class SafeValue : FunctionDefinition {
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }

        public override object Evaluate(Context context, ArrayList arguments, IOutputWriter writer) {
            this.CheckArgumentsNumber(arguments, 2);

            if(context.Contains(arguments[0] as string))
                return context[arguments[0] as string];
            else
                return Atom.EvaluateAtomObject(context, arguments[1]);
        }

        private const string help = "Vr�t� hodnotu prom�nn�. Pokud prom�nn� neexistuje, vr�t� uvedenou default hodnotu.";
        private const string parameters = "N�zev prom�nn�; defaultn� hodnota";
    }
}

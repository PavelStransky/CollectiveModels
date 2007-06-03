using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vyp�e text (prom�nnou) do writeru
    /// </summary>
    public class FnPrint : FunctionDefinition {
        public override string Name { get { return name; } }
        public override string Help { get { return help; } }
        public override string Parameters { get { return parameters; } }


        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 1);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return guider.Write(arguments[0]);
        }

        private const string name = "print";
        private const string help = "Vyp�e v�sledek v�razu";
        private const string parameters = "v�raz [; po�et vol�n�, po kolika dojde k v�pisu (int)]";
    }
}

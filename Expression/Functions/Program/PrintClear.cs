using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vyma�e writer
    /// </summary>
    public class PrintClear : Fnc {
        public override string Help { get { return help; } }

        protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
            this.CheckArgumentsNumber(evaluatedArguments, 0);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            guider.Clear();
            return null;
        }

        private const string help = "Vyma�e v�echny v�stupy";
    }
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Duration of evaluating of the expression
    /// </summary>
    public class FnTime : Fnc {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpTime; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, false, false, Messages.PCommands, Messages.PCommandsDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            this.CheckArgumentsNumber(arguments, 1);

            DateTime startTime = DateTime.Now;
            Atom.EvaluateAtomObject(guider, arguments[0]);
            return DateTime.Now - startTime;
        }

        private const string name = "time";
    }
}
using System.Collections;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Mutes the output of a function
    /// </summary>
    public class Mute: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpMute; } }
        public override OperatorPriority Priority { get { return OperatorPriority.MaxPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, false, false, Messages.PFnName, Messages.PFnNameDescription, null);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];
            guider.Mute = true;
            object result = Atom.EvaluateAtomObject(guider, item);
            guider.Mute = false;

            return result;
        }

        private const string name = "@";
    }
}

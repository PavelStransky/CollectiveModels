using System.Collections;
using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Operator ||
    /// </summary>
    public class BoolOr: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpBoolOr; } }
        public override OperatorPriority Priority { get { return OperatorPriority.BoolOrPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, true, true, false, Messages.PBool, Messages.PBoolDescription, null, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            bool result = true;

            foreach(bool b in arguments)
                result = result || b;

            return result;
        }

        private const string name = "||";
    }
}

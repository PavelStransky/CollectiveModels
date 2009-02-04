using System.Collections;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Operator ! (boolean NOT)
    /// </summary>
    public class Negation: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpNegation; } }
        public override OperatorPriority Priority { get { return OperatorPriority.BoolNegPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PBool, Messages.PBoolDescription, null, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return !(bool)arguments[0];
        }

        private const string name = "!";
    }
}

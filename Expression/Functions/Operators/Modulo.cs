using System.Collections;
using System.Text;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Modulo power
    /// </summary>
    public class Modulo: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpModulo; } }
        public override OperatorPriority Priority { get { return OperatorPriority.IntervalPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PInteger, Messages.PIntegerDescription, null,
                typeof(int));
            this.SetParam(1, true, true, false, Messages.PInteger, Messages.PIntegerDescription, null,
                typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object left = arguments[0];
            object right = arguments[1];

            if(left is int) {
                if(right is int)
                    return (int)left % (int)right;
            }

            this.BadTypeCompatibility(left, right);
            return null;
        }

        private const string name = "%";
    }
}

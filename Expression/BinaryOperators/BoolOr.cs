using System;

using PavelStransky.Expression;

namespace PavelStransky.Expression.BinaryOperators {
    /// <summary>
    /// Booleovský souèet ||
    /// </summary>
    public class BoolOr: BinaryOperator {
        public override string OperatorName { get { return operatorName; } }
        public override int Priority { get { return multiplePriority; } }

        protected override object EvaluateBB(bool left, bool right) {
            return left || right;
        }

        private const string operatorName = "||";
    }
}

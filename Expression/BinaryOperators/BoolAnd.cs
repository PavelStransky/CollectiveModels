using System;

using PavelStransky.Expression;

namespace PavelStransky.Expression.BinaryOperators {
    /// <summary>
    /// Boolean multiplication &&
    /// </summary>
    public class BoolAnd: BinaryOperator {
        public override string OperatorName { get { return operatorName; } }
        public override OperatorPriority Priority { get { return OperatorPriority.BoolMultiplePriority; } }

        protected override object EvaluateBB(bool left, bool right) {
            return left && right;
        }

        private const string operatorName = "&&";
    }
}

using System;

using PavelStransky.Expression;

namespace PavelStransky.Expression.BinaryOperators {
    /// <summary>
    /// Booleovsk� sou�et ||
    /// </summary>
    public class BoolOr: BinaryOperator {
        public override string OperatorName { get { return operatorName; } }
        public override OperatorPriority Priority { get { return OperatorPriority.BoolAddPriority; } }

        protected override object EvaluateBB(bool left, bool right) {
            return left || right;
        }

        private const string operatorName = "||";
    }
}

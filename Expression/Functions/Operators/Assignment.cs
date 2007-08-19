using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.BinaryOperators {
    /// <summary>
    /// Operátor pøiøazení
    /// </summary>
    public class PlusX: BinaryOperator {
        public override string OperatorName { get { return operatorName; } }
        public override OperatorPriority Priority { get { return OperatorPriority.BoolAddPriority; } }

        public override object Evaluate(object left, object right) {
            return base.Evaluate(left, right);
        }

        private const string operatorName = "=";
    }
}

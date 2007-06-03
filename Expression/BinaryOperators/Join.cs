using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.BinaryOperators {
    /// <summary>
    /// Operator that joins items together
    /// </summary>
    public class Join: BinaryOperator {
        public override string OperatorName { get { return operatorName; } }
        public override OperatorPriority Priority { get { return OperatorPriority.JoinPriority; } }

        protected override object EvaluateDDx(double left, double right) {
            Vector result = new Vector(2);

            result[0] = left;
            result[1] = right;
            
            return result;
        }

        protected override object EvaluateDVx(double left, Vector right) {
            int length = right.Length;
            Vector result = new Vector(length + 1);

            result[0] = left;
            for(int i = 0; i < length; i++)
                result[i + 1] = right[i];

            return result;
        }

        protected override object EvaluatePPx(PointD left, PointD right) {
            PointVector result = new PointVector(2);

            result[0] = left;
            result[1] = right;

            return result;
        }

        protected override object EvaluateVDx(Vector left, double right) {
            int length = left.Length;
            Vector result = new Vector(length + 1);

            for(int i = 0; i < length; i++)
                result[i] = left[i];
            result[length] = right;

            return result;
        }

        protected override object EvaluateVVx(Vector left, Vector right) {
            return Vector.Join(left, right);
        }

        protected override object EvaluateSSx(string left, string right) {
            return left + right;
        }

        protected override object EvaluateLO(List left, object right) {
            List result = left.Clone() as List;
            result.Add(right);
            return result;
        }

        protected override object EvaluateOL(object left, List right) {
            List result = right.Clone() as List;
            result.Insert(0, left);
            return result;
        }

        protected override object EvaluateLL(List left, List right) {
            List result = new List();

            result.AddRange(left);
            result.AddRange(right);

            return result;
        }

        private const string operatorName = "~";
    }
}

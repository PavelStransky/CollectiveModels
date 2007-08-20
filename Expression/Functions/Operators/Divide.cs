using System.Collections;
using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Operator divide
    /// </summary>
    public class Divide: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpDivide; } }
        public override OperatorPriority Priority { get { return OperatorPriority.ProductPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PDividend, Messages.PDividendDescription, null,
                typeof(int), typeof(double), typeof(Vector), typeof(Matrix), typeof(PointD), typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.PDivisor, Messages.PDivisorDescription, null,
                typeof(int), typeof(double), typeof(Vector), typeof(Matrix), typeof(PointD), typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object left = arguments[0];
            object right = arguments[1];

            if(left is int) {
                if(right is int)
                    return (int)left / (int)right;
                else if(right is double)
                    return (int)left / (double)right;
                else if(right is Matrix)
                    return ((Matrix)right).Inv() * (int)left;
            }
            else if(left is double) {
                if(right is int)
                    return (double)left / (int)right;
                else if(right is double)
                    return (double)left / (double)right;
                else if(right is Matrix)
                    return ((Matrix)right).Inv() * (double)left;
            }
            else if(left is Vector) {
                if(right is int)
                    return (Vector)left / (int)right;
                else if(right is double)
                    return (Vector)left / (double)right;
                else if(right is Matrix)
                    return (Vector)left * ((Matrix)right).Inv();
            }
            else if(left is Matrix) {
                if(right is int)
                    return (Matrix)left / (int)right;
                else if(right is double)
                    return (Matrix)left / (double)right;
                else if(right is Matrix)
                    return (Matrix)left / (Matrix)right;
            }
            else if(left is PointD) {
                if(right is PointD)
                    return (PointD)left / (PointD)right;
            }
            else if(left is PointVector) {
                if(right is PointD)
                    return (PointVector)left / (PointD)right;
            }

            this.BadTypeCompatibility(left, right);
            return null;
        }

        private const string name = "/";
    }
}

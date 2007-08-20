using System.Collections;
using System;

using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Operator times
    /// </summary>
    public class Times: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpTimes; } }
        public override OperatorPriority Priority { get { return OperatorPriority.ProductPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PCoefficient, Messages.PCoefficient, null,
                typeof(int), typeof(double), typeof(Vector), typeof(Matrix), typeof(PointD), typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.PCoefficient, Messages.PCoefficient, null,
                typeof(int), typeof(double), typeof(Vector), typeof(Matrix), typeof(PointD), typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object left = arguments[0];
            object right = arguments[1];

            if(left is int) {
                if(right is int)
                    return (int)left * (int)right;
                else if(right is double)
                    return (int)left * (double)right;
                else if(right is Vector)
                    return (int)left * (Vector)right;
                else if(right is Matrix)
                    return (Matrix)right * (int)left;
            }
            else if(left is double) {
                if(right is int)
                    return (double)left * (int)right;
                else if(right is double)
                    return (double)left * (double)right;
                else if(right is Vector)
                    return (double)left * (Vector)right;
                else if(right is Matrix)
                    return (Matrix)right * (double)left;
            }
            else if(left is Vector) {
                if(right is int)
                    return (Vector)left * (int)right;
                else if(right is double)
                    return (Vector)left * (double)right;
                else if(right is Vector)
                    return (Vector)left * (Vector)right;
                else if(right is Matrix)
                    return (Vector)left * (Matrix)right;
            }
            else if(left is Matrix) {
                if(right is int)
                    return (Matrix)left * (int)right;
                else if(right is double)
                    return (Matrix)left * (double)right;
                else if(right is Vector)
                    return (Matrix)left * (Vector)right;
                else if(right is Matrix)
                    return (Matrix)left * (Matrix)right;
            }
            else if(left is PointD) {
                if(right is PointD)
                    return (PointD)left * (PointD)right;
                else if(right is PointVector)
                    return (PointVector)right * (PointD)left;
            }
            else if(left is PointVector) {
                if(right is PointD)
                    return (PointVector)left * (PointD)right;
            }

            this.BadTypeCompatibility(left, right);
            return null;
        }

        private const string name = "*";
    }
}

using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.PT;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Creates a DoubleQuadratic class
    /// </summary>
    public class TwoLHO: FunctionDefinition {
        public override string Help { get { return Messages.HelpTwoLHO; } }

        protected override void CreateParameters() {
            this.NumParams(5);

            this.SetParam(0, false, true, true, Messages.P1TwoLHO, Messages.P1TwoLHODescription, 1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.P2TwoLHO, Messages.P2TwoLHODescription, -0.5, typeof(double));
            this.SetParam(2, false, true, true, Messages.P3TwoLHO, Messages.P3TwoLHODescription, 0.5, typeof(double));
            this.SetParam(3, false, true, true, Messages.P4TwoLHO, Messages.P4TwoLHODescription, 1.0, typeof(double));
            this.SetParam(4, false, true, true, Messages.PHBar, Messages.PHBarDescription, 0.01, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double omega = (double)arguments[0];
            double x1 = (double)arguments[1];
            double x2 = (double)arguments[2];
            double m = (double)arguments[3];
            double hbar = (double)arguments[4];

            return new DoubleQuadratic(omega, x1, x2, m, hbar);
        }
    }
}
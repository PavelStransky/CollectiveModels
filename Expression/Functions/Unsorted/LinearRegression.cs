using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Linear regression of data (including uncertainities of estimated parameters)
    /// </summary>
    public class LinearRegression: FunctionDefinition {
        public override string Help { get { return Messages.RegressionHelp; } }

        protected override void CreateParameters() {
            this.NumParams(1);
            this.SetParam(0, true, true, false, Messages.P1Regression, Messages.P1RegressionDescription, null, typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PointVector pointVector = arguments[0] as PointVector;
            return Regression.ComputeLinear(pointVector);
        }
    }
}
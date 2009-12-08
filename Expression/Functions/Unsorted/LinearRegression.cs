using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Linear regression of data (including uncertainities of estimated parameters)
    /// </summary>
    public class LinearRegression: Fnc {
        public override string Help { get { return Messages.HelpRegression; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.P1Regression, Messages.P1RegressionDescription, null, typeof(PointVector), typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments[0] is Vector)
                return Regression.ComputeLinear(new PointVector(arguments[0] as Vector));
            else
                return Regression.ComputeLinear(arguments[0] as PointVector);
        }
    }
}
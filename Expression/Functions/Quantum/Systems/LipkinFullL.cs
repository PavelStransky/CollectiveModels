using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a class of the Lipkin model with control parameters as of PRE 2017, diagonalized in the full SU(2) basis
    /// </summary>
    public class LipkinFullL : Fnc {
        public override string Help { get { return Messages.HelpLipkinFullL; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);
            this.SetParam(0, false, true, true, Messages.PAlpha, Messages.PAlphaDescription, 0.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.POmega, Messages.POmegaDescription, 0.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PAlpha, Messages.PAlphaDescription, 0.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.POmega, Messages.POmegaDescription, 0.0, typeof(double));
            this.SetParam(4, false, true, false, Messages.PIsLinear, Messages.PIsLinearDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double alpha = (double)arguments[0];
            double omega = (double)arguments[1];

            double alphaIm = (double)arguments[2];
            double omegaIm = (double)arguments[3];

            bool isLinear = (bool)arguments[4];

            return new LipkinFullLambda(alpha, omega, alphaIm, omegaIm, isLinear);
        }
    }
}
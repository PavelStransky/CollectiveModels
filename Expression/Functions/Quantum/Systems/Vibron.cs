using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def
{
    /// <summary>
    /// Creates the quantum Vibron class
    /// </summary>
    public class Vibron : Fnc
    {
        public override string Help { get { return Messages.HelpVibron; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, false, true, true, Messages.PAlpha, Messages.PAlphaDescription, 1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PBeta, Messages.PBetaDescription, 1.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double alpha = (double)arguments[0];
            double beta = (double)arguments[1];
            
            return new PavelStransky.Systems.Vibron(alpha, beta);
        }
    }
}
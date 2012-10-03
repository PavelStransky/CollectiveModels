using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a Lipkin model class diagonalized in the full SU(2) basis, divided into two interacting subsystems consisting of the environment + spectator
    /// </summary>
    public class LipkinTwoSpectatorFnc: Fnc {
        public override string Help { get { return Messages.HelpLipkinTwoSpectator; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);
            this.SetParam(0, false, true, true, Messages.PAlpha, Messages.PAlphaDescription, 0.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.POmega, Messages.POmegaDescription, 0.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PAlphaS, Messages.PAlphaSDescription, 0.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.POmegaS, Messages.POmegaSDescription, 0.0, typeof(double));
            this.SetParam(4, false, true, true, Messages.PG, Messages.PGDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double alpha = (double)arguments[0];
            double omega = (double)arguments[1];
            double alphaS = (double)arguments[2];
            double omegaS = (double)arguments[3];
            double g = (double)arguments[4];

            return new LipkinTwoSpectator(alpha, omega, alphaS, omegaS, g);
        }

        const string name = "lipkintwospectator";
    }
}
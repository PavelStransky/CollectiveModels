using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a Lipkin model consisting of the environment diagonalized in the full SU(2) basis + one simply interacting spin
    /// </summary>
    public class LipkinTwoSpectatorSimpleFnc: Fnc {
        public override string Help { get { return Messages.HelpLipkinTwoSpectatorSimple; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);
            this.SetParam(0, false, true, true, Messages.PAlpha, Messages.PAlphaDescription, 0.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.POmega, Messages.POmegaDescription, 0.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PEpsilon, Messages.PEpsilonDescription, 0.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PG, Messages.PGDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double alpha = (double)arguments[0];
            double omega = (double)arguments[1];
            double epsilon = (double)arguments[2];
            double g = (double)arguments[3];

            return new LipkinTwoSpectatorSimple(alpha, omega, epsilon, g);
        }

        const string name = "lipkintwospectatorsimple";
    }
}
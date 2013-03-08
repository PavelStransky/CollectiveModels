using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates quantum Creagh-Whelan class
    /// </summary>
    public class QCW : Fnc {
        public override string Help { get { return Messages.HelpQuantumCW; } }

        protected override void CreateParameters() {
            this.SetNumParams(6);
            this.SetParam(0, false, true, true, Messages.PA, Messages.PADescription, 0.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PB, Messages.PBDescription, 0.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PC, Messages.PCDescription, 1.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PMu, Messages.PMuDescription, 1.0, typeof(double));
            this.SetParam(4, false, true, true, Messages.PHBar, Messages.PHBarDescription, 0.01, typeof(double));
            this.SetParam(5, false, true, false, Messages.PPower, Messages.PPowerDescription, 2, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double a = (double)arguments[0];
            double b = (double)arguments[1];
            double c = (double)arguments[2];
            double mu = (double)arguments[3];
            double hbar = (double)arguments[4];
            int power = (int)arguments[5];

            return new QuantumCW(a, b, c, mu, hbar, power);
        }
    }
}
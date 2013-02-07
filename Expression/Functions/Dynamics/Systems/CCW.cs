using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates classical Creagh-Whelan class
    /// </summary>
    public class CCW : Fnc {
        public override string Help { get { return Messages.HelpClassicalCW; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);
            this.SetParam(0, false, true, true, Messages.PA, Messages.PADescription, 0.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PB, Messages.PBDescription, 0.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PMu, Messages.PMuDescription, 1.0, typeof(double));
            this.SetParam(3, false, true, false, Messages.PPower, Messages.PPowerDescription, 2, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double a = (double)arguments[0];
            double b = (double)arguments[1];
            double mu = (double)arguments[2];
            int power = (int)arguments[3];
            
            return new ClassicalCW(a, b, mu, power);
        }
    }
}
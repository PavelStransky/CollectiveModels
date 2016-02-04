using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the zeros of the partition function of a 2D Spin system
    /// </summary>
    public class SpinZZero : Fnc {
        public override string Help { get { return Messages.HelpSpinZZero; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PIsing2D, Messages.PIsing2DDescription, null, typeof(Ising2D));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Ising2D ising = arguments[0] as Ising2D;
            return ising.GetZeros();
        }
    }
}
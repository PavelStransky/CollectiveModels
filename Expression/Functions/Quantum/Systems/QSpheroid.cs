using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates Quantum Spheroid class
    /// </summary>
    public class QSpheroid: Fnc {
        public override string Help { get { return Messages.HelpQuantumSpheroid; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, true, Messages.PDeformation, Messages.PDeformationDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double delta = (double)arguments[0];
            return new Spheroid(delta);
        }
    }
}
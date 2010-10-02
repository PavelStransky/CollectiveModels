using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns a basis vector
    /// </summary>
    public class BasisVector: Fnc {
        public override string Help { get { return Messages.HelpBasisVector; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PEVIndex, Messages.PEVIndexDescription, null, typeof(Vector), typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem system = (IQuantumSystem)arguments[0];
            int index = (arguments[1] is int) ? (int)arguments[1] : system.EigenSystem.BasisQuantumNumber((Vector)arguments[1]);
            return system.EigenSystem.BasisVector(index);
        }
    }
}
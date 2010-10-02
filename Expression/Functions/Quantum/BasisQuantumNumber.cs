using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Transforms the index of the basis vector into its quantum numbers and vice versa
    /// </summary>
    public class BasisQuantumNumber: Fnc {
        public override string Help { get { return Messages.HelpBasisQuantumNumber; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PEVIndex, Messages.PEVIndexDescription, null, typeof(Vector), typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem system = (IQuantumSystem)arguments[0];

            if(arguments[1] is int)
                return system.EigenSystem.BasisQuantumNumber((int)arguments[1]);
            else
                return system.EigenSystem.BasisQuantumNumber((Vector)arguments[1]);
        }
    }
}
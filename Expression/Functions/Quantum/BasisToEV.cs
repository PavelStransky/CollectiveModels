using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Transforms given state vector represented in basis components to a vector expressed in components of eigenvectors
    /// </summary>
    public class BasisToEV: Fnc {
        public override string Help { get { return Messages.HelpBasisToEV; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PKet, Messages.PKetDescription, null, typeof(PointVector), typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem system = (IQuantumSystem)arguments[0];
            if(arguments[1] is Vector)
                return system.EigenSystem.BasisToEV(new PointVector(arguments[1] as Vector, 0.0)).VectorX;
            else
                return system.EigenSystem.BasisToEV(arguments[1] as PointVector);
        }
    }
}
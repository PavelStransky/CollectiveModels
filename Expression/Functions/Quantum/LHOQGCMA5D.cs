using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates an object that calculates eigenenergies of QuantumGCM in 5D basis 
    /// preparing the Hamiltonian matrix by using algebraic relations
    /// </summary>
    public class LHOQGCMA5D: FncLHOQGCM {
        public override string Help { get { return Messages.HelpLHOQGCMA5D; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMA5D(a, b, c, k, a0, hbar);
        }
    }
}
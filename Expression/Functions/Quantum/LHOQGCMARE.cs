using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates an object that calculates eigenenergies of QuantumGCM in radial 2D basis even in angular coordinate
    /// preparing the Hamiltonian matrix by using algebraic relations
    /// </summary>
    public class LHOQGCMARE: FncLHOQGCM {
        public override string Help { get { return Messages.HelpLHOQGCMARE; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMARE(a, b, c, k, a0, hbar);
        }
    }
}
using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates an object that calculates eigenenergies of QuantumGCM in radial 2D basis in angular coordinate
    /// preparing the Hamiltonian matrix by integrating the basis functions in x-representation;
    /// both odd and even states are included
    /// </summary>
    public class LHOQGCMIR: FncLHOQGCM {
        public override string Help { get { return Messages.HelpLHOQGCMIR; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMIR(a, b, c, k, a0, hbar);
        }
    }
}
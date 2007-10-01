using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates an object that calculates eigenenergies of QuantumGCM in 2D radial basis
    /// preparing the Hamiltonian matrix by integrating the basis functions in x-representation;
    /// states with all possible (also nonfysical) angular momentum are included    
    /// </summary>
    public class LHOQGCMIRF: FncLHOQGCM {
        public override string Help { get { return Messages.HelpLHOQGCMIRF; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMIRFull(a, b, c, k, a0, hbar);
        }
    }
}
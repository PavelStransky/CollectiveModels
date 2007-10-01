using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates an object that calculates eigenenergies of QuantumGCM in radial 2D basis odd in angular coordinate
    /// preparing the Hamiltonian matrix by integrating the basis functions in x-representation
    /// </summary>
    public class LHOQGCMIRO: FncLHOQGCM {
        public override string Help { get { return Messages.HelpLHOQGCMIRO; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMIRO(a, b, c, k, a0, hbar);
        }
    }
}
using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates an object that calculates eigenenergies of QuantumGCM in radial 2D basis odd in angular coordinate
    /// preparing the Hamiltonian matrix by using algebraic relations
    /// </summary>
    public class LHOQGCMARO: FncLHOQGCM {
        public override string Help { get { return Messages.HelpLHOQGCMARO; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMARO(a, b, c, k, a0, hbar);
        }
    }
}
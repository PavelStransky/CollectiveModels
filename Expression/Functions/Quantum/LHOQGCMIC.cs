using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates an object that calculates eigenenergies of QuantumGCM in 2D cartesian basis (direct product of two 1D harmonic oscillators)
    /// preparing the Hamiltonian matrix by integrating the basis functions in x-representation;
    /// states with all possible (also nonfysical) angular momentum are included
    /// </summary>
    public class LHOQGCMIC: FncLHOQGCM {
        public override string Help { get { return Messages.HelpLHOQGCMIC; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMIC(a, b, c, k, a0, hbar);
        }
    }
}
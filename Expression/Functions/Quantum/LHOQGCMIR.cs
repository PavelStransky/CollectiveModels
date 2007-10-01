using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vytvo�� LHOQuantumGCMR t��du (po��tanou v radi�ln�ch sou�adnic�ch)
    /// </summary>
    public class LHOQGCMIR: FncLHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMIR(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvo�� LHOQuantumGCMC t��du (kvantov� GCM v pol�rn� b�zi LHO)";
    }
}
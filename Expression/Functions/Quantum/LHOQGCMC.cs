using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvo�� LHOQuantumGCMC t��du
    /// </summary>
    public class LHOQGCMC: LHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMC(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvo�� LHOQuantumGCMC t��du (kvantov� GCM v kart�zsk� b�zi LHO, jen m = 3k)";
    }
}
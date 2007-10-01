using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates LHOQuantumGCMI5D class
    /// </summary>
    public class LHOQGCMI5D: FncLHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMI5D(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvo�� LHOQuantumGCMI5D t��du (kvantov� GCM v kart�zsk� b�zi LHO, m = 3k, po��t�no pomoc� LAPACK)";
    }
}
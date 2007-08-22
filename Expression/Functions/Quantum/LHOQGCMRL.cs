using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vytvo�� LHOQuantumGCMR t��du (po��tanou v radi�ln�ch sou�adnic�ch)
    /// pomoc� knihovny LAPACK
    /// </summary>
    public class LHOQGCMRL: FncLHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMRL(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvo�� LHOQuantumGCMRL t��du (kvantov� GCM v kart�zsk� b�zi LHO, m = 3k, po��t�no pomoc� LAPACK)";
    }
}
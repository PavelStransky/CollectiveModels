using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vytvo�� LHOQuantumGCMRFull t��du (po��tanou v radi�ln�ch sou�adnic�ch, bereme �plnou b�zi)
    /// </summary>
    public class LHOQGCMIRF: FncLHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMIRFull(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvo�� LHOQuantumGCMRFull t��du (kvantov� GCM v kart�zsk� b�zi LHO, �pln� b�ze)";
    }
}
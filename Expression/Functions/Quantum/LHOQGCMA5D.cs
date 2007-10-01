using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates an object that calculates eigenenergies of QuantumGCM 
    /// Vytvo�� LHOQuantumGCMA5D t��du (po��tanou v radi�ln�ch sou�adnic�ch, jen sud� stavy, pomoc� algebraick�ch vztah�, 5D b�ze)
    /// </summary>
    public class LHOQGCMA5D: FncLHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMA5D(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvo�� LHOQuantumGCMRLO t��du (kvantov� GCM v kart�zsk� b�zi LHO, m = 3k, po��t�no pomoc� LAPACK, jen lich� stavy)";
    }
}
using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vytvo�� LHOQuantumGCMRALO t��du (po��tanou v radi�ln�ch sou�adnic�ch, jen lich� stavy, pomoc� algebraick�ch vztah�)
    /// a diagonalizuje pomoc� knihovny LAPACK
    /// </summary>
    public class LHOQGCMRALO: FncLHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMRALO(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvo�� LHOQuantumGCMRLO t��du (kvantov� GCM v kart�zsk� b�zi LHO, m = 3k, po��t�no pomoc� LAPACK, jen lich� stavy)";
    }
}
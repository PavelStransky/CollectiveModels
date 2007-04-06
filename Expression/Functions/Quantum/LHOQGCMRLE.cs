using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvo�� LHOQuantumGCMRLE t��du (po��tanou v radi�ln�ch sou�adnic�ch, jen sud� stavy)
    /// pomoc� knihovny LAPACK
    /// </summary>
    public class LHOQGCMRLE: LHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMRLE(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvo�� LHOQuantumGCMRLE t��du (kvantov� GCM v kart�zsk� b�zi LHO, m = 3k, po��t�no pomoc� LAPACK, jen sud� stavy)";
    }
}
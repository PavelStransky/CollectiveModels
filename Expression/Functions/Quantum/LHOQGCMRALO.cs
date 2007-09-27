using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vytvoøí LHOQuantumGCMRALO tøídu (poèítanou v radiálních souøadnicích, jen liché stavy, pomocí algebraických vztahù)
    /// a diagonalizuje pomocí knihovny LAPACK
    /// </summary>
    public class LHOQGCMRALO: FncLHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMRALO(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvoøí LHOQuantumGCMRLO tøídu (kvantový GCM v kartézské bázi LHO, m = 3k, poèítáno pomocí LAPACK, jen liché stavy)";
    }
}
using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vytvoøí LHOQuantumGCMR tøídu (poèítanou v radiálních souøadnicích)
    /// pomocí knihovny LAPACK
    /// </summary>
    public class LHOQGCMRL: FncLHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMRL(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvoøí LHOQuantumGCMRL tøídu (kvantový GCM v kartézské bázi LHO, m = 3k, poèítáno pomocí LAPACK)";
    }
}
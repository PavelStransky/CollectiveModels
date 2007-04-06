using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí LHOQuantumGCMRLE tøídu (poèítanou v radiálních souøadnicích, jen sudé stavy)
    /// pomocí knihovny LAPACK
    /// </summary>
    public class LHOQGCMRLE: LHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMRLE(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvoøí LHOQuantumGCMRLE tøídu (kvantový GCM v kartézské bázi LHO, m = 3k, poèítáno pomocí LAPACK, jen sudé stavy)";
    }
}
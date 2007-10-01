using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vytvoøí LHOQuantumGCMR tøídu (poèítanou v radiálních souøadnicích)
    /// </summary>
    public class LHOQGCMIR: FncLHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMIR(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvoøí LHOQuantumGCMC tøídu (kvantový GCM v polární bázi LHO)";
    }
}
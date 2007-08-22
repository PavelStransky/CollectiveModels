using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vytvoøí LHOQuantumGCMC tøídu
    /// </summary>
    public class LHOQGCMC: FncLHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMC(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvoøí LHOQuantumGCMC tøídu (kvantový GCM v kartézské bázi LHO, jen m = 3k)";
    }
}
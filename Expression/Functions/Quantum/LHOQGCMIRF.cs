using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Vytvoøí LHOQuantumGCMRFull tøídu (poèítanou v radiálních souøadnicích, bereme úplnou bázi)
    /// </summary>
    public class LHOQGCMIRF: FncLHOQGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double a0, double hbar) {
            return new LHOQuantumGCMIRFull(a, b, c, k, a0, hbar);
        }

        private const string help = "Vytvoøí LHOQuantumGCMRFull tøídu (kvantový GCM v kartézské bázi LHO, úplná báze)";
    }
}
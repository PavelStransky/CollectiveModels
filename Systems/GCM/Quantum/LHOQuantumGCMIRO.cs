using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Kvantový GCM v bázi 2D lineárního harmonického oscilátoru jen pro liché stavy
    /// </summary>
    public class LHOQuantumGCMIRO: LHOQuantumGCMIR {
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LHOQuantumGCMIRO() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMIRO(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        public LHOQuantumGCMIRO(Core.Import import) : base(import) { }

        protected override int GetBasisQuantumNumber2(int i) {
            if(i < 0)
                return this.index.MaxM / 3 + 2;
            else
                return (this.index.M[i] + this.index.MaxM) / 3;
        }

        /// <summary>
        /// Vytvoøí instanci tøídy LHOPolarIndex
        /// </summary>
        /// <param name="maxE">Maximální energie</param>
        protected override void CreateIndex(int maxE) {
            if(this.index == null || this.index.MaxE != maxE)
                this.index = new LHOPolarIndex(maxE, false, 1);            
        }

        /// <summary>
        /// Vlnová funkce ve 2D
        /// </summary>
        protected override double PsiBG(double beta, double gamma, int l) {
            int n = this.index.N[l];
            int m = this.index.M[l];

            return this.Psi2D(beta, n, m) * this.Phi2DO(gamma, m);
        }
    }
}
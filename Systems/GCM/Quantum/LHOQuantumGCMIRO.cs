using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Kvantov� GCM v b�zi 2D line�rn�ho harmonick�ho oscil�toru jen pro lich� stavy
    /// </summary>
    public class LHOQuantumGCMIRO: LHOQuantumGCMIR {
        /// <summary>
        /// Pr�zdn� konstruktor
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

        public LHOQuantumGCMIRO(Import import) : base(import) { }

        /// <summary>
        /// Vytvo�� instanci t��dy LHO5DIndex
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public override BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LHOPolarIndexIO(basisParams);
        }

        /// <summary>
        /// Vlnov� funkce ve 2D
        /// </summary>
        protected override double PsiBG(double beta, double gamma, int l) {
            LHOPolarIndexIO index = this.eigenSystem.BasisIndex as LHOPolarIndexIO;
            
            int n = index.N[l];
            int m = index.M[l];

            return this.Psi2D(beta, n, m) * this.Phi2DO(gamma, m);
        }
    }
}
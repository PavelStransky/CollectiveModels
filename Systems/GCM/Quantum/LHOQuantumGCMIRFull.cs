using System;
using System.IO;

using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Kvantový GCM v bázi 2D lineárního harmonického oscilátoru
    /// </summary>
    public class LHOQuantumGCMIRFull : LHOQuantumGCMIR {
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LHOQuantumGCMIRFull() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMIRFull(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        public LHOQuantumGCMIRFull(Import import) : base(import) { }

        /// <summary>
        /// Vytvoøí instanci tøídy LHOPolarIndexIFull
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public override BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LHOPolarIndexIFull(basisParams);
        }
    }
}
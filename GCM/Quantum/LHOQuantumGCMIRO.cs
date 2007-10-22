using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.DLLWrapper;

namespace PavelStransky.GCM {
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

        public LHOQuantumGCMIRO(Core.Import import) : base(import) { }

        protected override int GetBasisQuantumNumber2(int i) {
            if(i < 0)
                return this.index.MaxM / 3 + 2;
            else
                return (this.index.M[i] + this.index.MaxM) / 3;
        }

        /// <summary>
        /// Vytvo�� instanci t��dy LHOPolarIndex
        /// </summary>
        /// <param name="maxE">Maxim�ln� energie</param>
        protected override void CreateIndex(int maxE) {
            if(this.index == null || this.index.MaxE != maxE)
                this.index = new LHOPolarIndex(maxE, false, 1);            
        }
    }
}
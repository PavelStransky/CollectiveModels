using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Implementace komplexní čvtercové matice, která je vhodná přímo pro LAPACK diagonalizaci
    /// </summary>
    unsafe public class CMatrix : IDisposable, IMatrix {
        private double* item;

        int length;

        /// <summary>
        /// Kontruktor
        /// </summary>
        /// <param name="length">Velikost matice</param>
        public CMatrix(int length) {
            this.length = length;
            this.item = Memory.NewDouble(2 * this.length * this.length);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="re">Matice s reálnými hodnotami</param>
        /// <param name="im">Matice s komplexními hodnotami</param>
        public CMatrix(Matrix re, Matrix im) {
            this.length = re.Length;
            this.item = Memory.NewDouble(2 * this.length * this.length);
            for(int i = 0; i < this.length; i++)
                for(int j = 0; j < this.length; j++) {
                    this[i, 2 * j] = re[i, j];
                    this[i, 2 * j + 1] = im[i, j];
                }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="i">Řádek matice</param>
        /// <param name="j">Sloupec matice</param>
        /// <remarks>Sudé prvky - reálná část, liché prvky - komplexní část</remarks>
        public double this[int i, int j] {
            get {
                return this.item[2*(i * this.length) + j];
            }
            set {
                this.item[2*(i * this.length) + j] = value;
            }
        }

        /// <summary>
        /// Dimenze matice
        /// </summary>
        public int Length { get { return this.length; } }

        /// <summary>
        /// Pole s daty
        /// </summary>
        public double* GetItem() {
            return this.item;
        }

        /// <summary>
        /// Stopa matice (vrací jen reálnou část !!!)
        /// </summary>
        public double Trace() {
            int length = this.Length;
            double result = 0.0;

            for(int i = 0; i < length; i++)
                result += this.item[2*(i * length + i)];

            return result;
        }

        /// <summary>
        /// Systém vlastních čísel a vektorů
        /// </summary>        
        public Vector[] EigenSystem(bool ev, int numEV, IOutputWriter writer) {
            return LAPackDLL.zgeev(this, ev, ev);
        }

        #region Destrukce
        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            Memory.Delete(this.item);
        }

        ~CMatrix() {
            this.Dispose(false);
        }
        #endregion
    }
}

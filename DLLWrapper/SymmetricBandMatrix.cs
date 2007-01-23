using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Implementace symetrick� p�sov� matice
    /// </summary>
    unsafe public class SymmetricBandMatrix : IDisposable {
        // Uchov�v�me horn� troj�heln�k
        double* item;

        int n;
        int numSD;

        /// <summary>
        /// Kontruktor
        /// </summary>
        /// <param name="n">Velikost matice</param>
        /// <param name="numSD">Po�et superdiagon�l</param>
        public SymmetricBandMatrix(int n, int numSD) {
            this.n = n;
            this.numSD = numSD;
            this.item = Memory.NewDouble(n * (numSD + 1));
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="i">��dek matice</param>
        /// <param name="j">Sloupec matice</param>
        public double this[int i, int j] {
            get {
                if(System.Math.Abs(i - j) <= this.numSD) {
                    if(i < j)
                        return this.item[(this.numSD + i - j) + j * (this.numSD + 1)];
                    else
                        return this.item[(this.numSD + j - i) + i * (this.numSD + 1)];
                }
                else
                    return 0.0;
            }
            set {
                if(System.Math.Abs(i - j) <= this.numSD) {
                    if(i < j)
                        this.item[(this.numSD + i - j) + j * (this.numSD + 1)] = value;
                    else
                        this.item[(this.numSD + j - i) + i * (this.numSD + 1)] = value;
                }
                else
                    throw new IndexOutOfRangeException(string.Format("P��stup k nep��pustn�mu indexu ({0}, {1}) ve t��d� SymmetricBandMatrix.", i, j));
            }
        }

        /// <summary>
        /// P�etypov�n� na oby�ejnou matici
        /// </summary>
        public static explicit operator Matrix(SymmetricBandMatrix sbm) {
            int length = sbm.n;
            Matrix result = new Matrix(length);

            for(int i = 0; i < length; i++)
                for(int j = 0; j < length; j++)
                    result[i, j] = sbm[i, j];

            return result;
        }

        /// <summary>
        /// Vr�t� p�s jako matici
        /// </summary>
        public Matrix GetBand() {
            int lengthX = this.numSD + 1;
            int lengthY = this.n;

            Matrix result = new Matrix(lengthX, lengthY);
            for(int i = 0; i < lengthX; i++)
                for(int j = 0; j < lengthY; j++)
                    result[i, j] = this.item[i * lengthY + j];

            return result;
        }

        /// <summary>
        /// Dimenze matice
        /// </summary>
        public int Length { get { return this.n; } }

        /// <summary>
        /// Po�et superdiagon�l
        /// </summary>
        public int NumSD { get { return this.numSD; } }

        /// <summary>
        /// Pole s daty
        /// </summary>
        public double* GetItem() { 
            return this.item; 
        }

        #region Destrukce
        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            Memory.Delete(this.item);
        }

        ~SymmetricBandMatrix() {
            this.Dispose(false);
        }
        #endregion
    }
}

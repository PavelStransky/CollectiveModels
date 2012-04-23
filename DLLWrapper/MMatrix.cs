using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Implementace èvtercové matice, která je vhodná pøímo pro LAPACK diagonalizaci
    /// </summary>
    unsafe public class MMatrix: IDisposable, IMatrix {
        private double* item;

        int length;

        /// <summary>
        /// Kontruktor
        /// </summary>
        /// <param name="length">Velikost matice</param>
        public MMatrix(int length) {
            this.length = length;
            this.item = Memory.NewDouble(this.length * this.length);
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="i">Øádek matice</param>
        /// <param name="j">Sloupec matice</param>
        public double this[int i, int j] {
            get {
                return this.item[i * this.length + j];
            }
            set {
                this.item[i * this.length + j] = value;
            }
        }

        /// <summary>
        /// Pøetypování na obyèejnou matici
        /// </summary>
        public static explicit operator Matrix(MMatrix mmatrix) {
            int length = mmatrix.Length;
            Matrix result = new Matrix(length);

            int index = 0;
            for(int i = 0; i < length; i++)
                for(int j = 0; j < length; j++)
                    result[i, j] = mmatrix.item[index++];

            return result;
        }

        /// <summary>
        /// Pøetypování z obyèejné matice
        /// </summary>
        public static explicit operator MMatrix(Matrix matrix) {
            int length = matrix.Length;
            MMatrix result = new MMatrix(length);

            int index = 0;
            for(int i = 0; i < length; i++)
                for(int j = 0; j < length; j++)
                    result.item[index++] = matrix[i, j];

            return result;
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
        /// Stopa matice
        /// </summary>
        public double Trace() {
            int length = this.Length;
            double result = 0.0;

            for(int i = 0; i < length; i++)
                result += this.item[i * length + i];

            return result;
        }
        
        /// <summary>
        /// Systém vlastních èísel a vektorù
        /// </summary>        
        public Vector[] EigenSystem(bool ev, int numEV, IOutputWriter writer) {
            return LAPackDLL.dgeev(this, ev, ev);
        }

        #region Destrukce
        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            Memory.Delete(this.item);
        }

        ~MMatrix() {
            this.Dispose(false);
        }
        #endregion
    }
}

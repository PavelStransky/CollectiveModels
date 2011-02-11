using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.DLLWrapper {
    unsafe public class SparseMatrix: IMatrix {
        private double[] data;
        private int[] iMatrix;
        private int[] jMatrix;
        private int length;

        // Size of the index fields
        private int size;

        // Number of nonzero elements
        private int nonZeros = 0;

        /// <summary>
        /// Dimension of the matrix
        /// </summary>
        public int Length { get { return this.length; } }

        /// <summary>
        /// Number of nonzero elements of the matrix
        /// </summary>
        public int NonzeroElements { get { return this.nonZeros; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length">Size of the matrix</param>
        public SparseMatrix(int length) : this(length, length) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length">Size of the matrix</param>
        /// <param name="size">Initial size of the auxiliary fields (expected number of the nonzero elements)</param>
        public SparseMatrix(int length, int size) {
            this.length = length;
            this.size = size;

            this.data = new double[this.size];
            this.jMatrix = new int[this.size];
            this.iMatrix = new int[this.length + 1];
        }

        #region Testing section
        /// <summary>
        /// Read data from a given file
        /// </summary>
        public static SparseMatrix ReadExample(string fName) {
            FileStream f = new FileStream(fName, FileMode.Open);
            StreamReader s = new StreamReader(f);

            int nonZeros = int.Parse(s.ReadLine());
            int length = int.Parse(s.ReadLine());
            s.ReadLine();

            SparseMatrix sm = new SparseMatrix(length, nonZeros);

            int ind = 0;
            do {
                string[] data = s.ReadLine().Trim().Split(' ');
                for(int i = 0; i < data.Length; i++)
                    sm.iMatrix[ind++] = int.Parse(data[i]);
            } while(ind < length);

            ind = 0;
            do {
                string[] data = s.ReadLine().Trim().Split(' ');
                for(int i = 0; i < data.Length; i++)
                    sm.jMatrix[ind++] = int.Parse(data[i]);
            } while(ind < nonZeros);

            ind = 0;
            do {
                string[] data = s.ReadLine().Trim().Split(' ');
                for(int i = 0; i < data.Length; i++)
                    sm.data[ind++] = double.Parse(data[i]);
            } while(ind < nonZeros);

            s.Close();
            f.Close();

            return sm;
        }

        public static SparseMatrix RandomMatrix(int length, int nonZeros) {
            SparseMatrix sm = new SparseMatrix(length, 6 * nonZeros / 5);
            
            Random r = new Random();
            double ratio = 2.0 * (double)nonZeros / ((double)length * (double)length);

            sm.iMatrix[0] = 0;

            int index = 0;
            for(int i = 0; i < length; i++) {
                for(int j = 0; j < length; j++) {
                    double d = 0.0;
                    if(j < i) 
                        d = sm[i, j];
                    else {
                        if(r.NextDouble() < ratio)
                            d = 10 * r.NextDouble() - 5;
                    }

                    if(d != 0.0) {
                        sm.data[index] = d;
                        sm.jMatrix[index] = j;
                        index++;
                    }
                }
                sm.iMatrix[i + 1] = index;
            }

            sm.nonZeros = index;
            return sm;
        }
        #endregion

        /// <summary>
        /// Calculate the Matrix-Vector-Product with the stored Matrix in the
        /// old Yale sparse matrix format.
        /// </summary>
        /// <param name="invec">Pointer to the vector which should be multiplied by the matrix</param>
        /// <param name="outvec">Pointer to an array which will get the result</param>
        public void VectorProduct(double* invec, double* outvec) {
            for(int i = 0; i < this.length; i++)
                outvec[i] = 0.0;

            for(int i = 0; i < this.length; i++) {
                for(int j = this.iMatrix[i]; j < this.iMatrix[i + 1]; j++) {
                    outvec[i] = outvec[i] + this.data[j] * invec[this.jMatrix[j]];
                }
            }
        }

        /// <summary>
        /// Pøetypuje na normální matici
        /// </summary>
        public static explicit operator Matrix(SparseMatrix sbm) {
            Matrix m = new Matrix(sbm.length);

            for(int i = 0; i < sbm.length; i++) {
                int numCols = sbm.iMatrix[i + 1] - sbm.iMatrix[i];
                for(int j = 0; j < numCols; j++) {
                    int ki = sbm.iMatrix[i] + j;
                    int kj = sbm.jMatrix[ki];
                    m[i, kj] = sbm.data[ki];
                }
            }

            return m;
        }

        /// <summary>
        /// Pøetypuje normální (symetrickou) matici na Sparse formát
        /// </summary>
        public static explicit operator SparseMatrix(Matrix m) {
            int length = m.Length;
            int nonZeros = m.NumNonzeroItems();

            SparseMatrix sm = new SparseMatrix(length, nonZeros);

            sm.iMatrix[0] = 0;

            int index = 0;
            for(int i = 0; i < length; i++) {
                for(int j = 0; j < length; j++) {
                    if(m[i, j] != 0) {
                        sm.data[index] = m[i, j];
                        sm.jMatrix[index] = j;
                        index++;
                    }
                }
                sm.iMatrix[i + 1] = index;
            }

            sm.nonZeros = index;
            return sm;
        }

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="i">Øádek matice</param>
        /// <param name="j">Sloupec matice</param>
        public double this[int i, int j] {
            get {
                int numCols = this.iMatrix[i + 1] - this.iMatrix[i];
                for(int jj = 0; jj < numCols; jj++) {
                    int ki = this.iMatrix[i] + jj;
                    int kj = this.jMatrix[ki];
                    if(kj == j)
                        return this.data[ki];
                    else if(kj < j)
                        return 0.0;
                }
                return 0.0;
            }
            set {
                int numCols = this.iMatrix[i + 1] - this.iMatrix[i];
                int inserti = -1;

                for(int jj = 0; jj < numCols; jj++) {
                    int ki = this.iMatrix[i] + jj;
                    int kj = this.jMatrix[ki];
                    if(kj == j) {                    // Zmìníme hodnotu
                        if(value == 0.0) {           // Musíme vymazat hodnotu z matice
                            for(int ii = ki + 1; ii < this.nonZeros; ii++) {
                                this.data[ii - 1] = this.data[ii];
                                this.jMatrix[ii - 1] = this.jMatrix[ii];
                            }
                            for(int ii = i + 1; ii <= this.length; ii++)
                                this.iMatrix[ii]--;
                            this.nonZeros--;
                        }
                        else
                            this.data[ki] = value;
                        return;
                    }
                    else if(kj < j) {               // Pøidáváme
                        inserti = ki;
                        break;
                    }
                    else if(jj == numCols - 1) {
                        inserti = ki + 1;
                        break;
                    }
                }

                if(value == 0.0)                    // 0 pøidávat nebudeme
                    return;

                if(inserti == -1)                   // V dané øadì není ani jeden prvek
                    inserti = this.iMatrix[i];

                this.IncreaseSize(1);

                for(int ii = this.nonZeros; ii > inserti; ii--) {
                    this.jMatrix[ii] = this.jMatrix[ii - 1];
                    this.data[ii] = this.data[ii - 1];
                }
                this.jMatrix[inserti] = j;
                this.data[inserti] = value;

                for(int ii = i + 1; ii <= this.length; ii++)
                    this.iMatrix[ii]++;

                this.nonZeros++;
            }
        }

        /// <summary>
        /// Increase size of auxiliary fields
        /// </summary>
        /// <param name="n">How many new elements we need</param>
        private void IncreaseSize(int n) {
            if(this.size < this.nonZeros + n) {
                this.size *= 2;
                int[] newjMatrix = new int[this.size];
                double[] newData = new double[this.size];

                for(int i = 0; i < this.nonZeros; i++) {
                    newjMatrix[i] = this.jMatrix[i];
                    newData[i] = this.data[i];
                }

                this.data = newData;
                this.jMatrix = newjMatrix;
            }
        }

        /// <summary>
        /// Stopa matice
        /// </summary>
        public double Trace() {
            double result = 0.0;

            for(int i = 0; i < this.length; i++) {
                int numCols = this.iMatrix[i + 1] - this.iMatrix[i];
                for(int j = 0; j < numCols; j++) {
                    int ki = this.iMatrix[i] + j;
                    int kj = this.jMatrix[ki];
                    if(i == kj)
                        result += this.data[ki];
                    if(kj >= i)
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Systém vlastních hodnot a vektorù
        /// </summary>
        /// <param name="ev">Chceme vlastní vektory?</param>
        /// <param name="numEV">Poèet</param>
        public Vector[] EigenSystem(bool ev, int numEV, IOutputWriter writer) {
            return ARPackDLL.dsaupd(this, numEV, ev, false);
        }
    }
}

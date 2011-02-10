using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.DLLWrapper {
    unsafe public class SparseMatrix {
        private ArrayList data;
        private int[] iMatrix;
        private ArrayList jMatrix;
        private int length;

        /// <summary>
        /// Dimension of the matrix
        /// </summary>
        public int Length { get { return this.length; } }

        public SparseMatrix(int length) {
            this.length = length;
            this.data = new ArrayList();
            this.jMatrix = new ArrayList();
            this.iMatrix = new int[this.length + 1];
        }

        public void ReadExample() {
            FileStream f = new FileStream("c:\\matrix.txt", FileMode.Open);
            StreamReader s = new StreamReader(f);

            int nonzeros = int.Parse(s.ReadLine());
            this.length = int.Parse(s.ReadLine());
            s.ReadLine();

            this.data = new ArrayList();
            this.jMatrix = new ArrayList();
            this.iMatrix = new int[this.length + 1];

            int ind = 0;
            do {
                string[] data = s.ReadLine().Trim().Split(' ');
                for(int i = 0; i < data.Length; i++)
                    this.iMatrix[ind++] = int.Parse(data[i]);
            } while(ind < this.length);

            ind = 0;
            do {
                string[] data = s.ReadLine().Trim().Split(' ');
                for(int i = 0; i < data.Length; i++) {
                    this.jMatrix.Add(int.Parse(data[i]));
                    ind++;
                }
            } while(ind < nonzeros);

            ind = 0;
            do {
                string[] data = s.ReadLine().Trim().Split(' ');
                for(int i = 0; i < data.Length; i++) {
                    this.data.Add(double.Parse(data[i]));
                    ind++;
                }
            } while(ind < nonzeros);

            s.Close();
            f.Close();
        }

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
                    outvec[i] = outvec[i] + (double)(this.data[j]) * invec[(int)(this.jMatrix[j])];
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
                    int kj = (int)(sbm.jMatrix[ki]);
                    m[i, kj] = (double)(sbm.data[ki]);
//                    m[kj, i] = m[i, kj];
                }
            }

            return m;
        }

        /// <summary>
        /// Pøetypuje normální (symetrickou) matici na Sparse formát
        /// </summary>
        public static explicit operator SparseMatrix(Matrix m) {
            int length = m.Length;
            SparseMatrix sm = new SparseMatrix(length);

            int nonzero = 0;
            sm.iMatrix[0] = 0;

            for(int i = 0; i < length; i++) {
                for(int j = 0; j < length; j++) {
                    if(m[i, j] != 0) {
                        sm.data.Add(m[i, j]);
                        sm.jMatrix.Add(j);
                        nonzero++;
                    }
                }
                sm.iMatrix[i + 1] = nonzero;
            }

            return sm;
        }
    }
}

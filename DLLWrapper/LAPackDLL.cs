using System;
using System.Runtime.InteropServices;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Wrapper pro LAPack.dll
    /// </summary>
    unsafe public class LAPackDLL {
        private LAPackDLL() { }

        /// <summary>
        /// Matici p�evede na pole na nespravovan� hald�
        /// </summary>
        /// <param name="m">Matice</param>
        private static double* PrepareMatrix(Matrix m) {
            int lengthX = m.LengthX;
            int lengthY = m.LengthY;

            double* result = Memory.NewDouble(lengthX * lengthY);

            int index = 0;
            for(int i = 0; i < lengthX; i++)
                for(int j = 0; j < lengthY; j++)
                    result[index++] = m[i, j];

            return result;
        }

        /// <summary>
        /// Pole na nespravovan� hald� p�evede na Vector
        /// </summary>
        /// <param name="v">Ukazatel na pole</param>
        /// <param name="n">D�lka pole</param>
        private static Vector ProcessVector(double* v, int n) {
            Vector result = new Vector(n);

            for(int i = 0; i < n; i++)
                result[i] = v[i];

            return result;
        }

        /// <summary>
        /// DSYEV computes all eigenvalues and, optionally, eigenvectors of a
        /// real symmetric matrix A.
        /// </summary>
        [DllImport("LAPack.dll")]
        private static extern double dsyev_(char* jobz, char* uplo, int* n, double* a, int* lda,
            double* w, double* work, int* lwork, int* info);

        /// <summary>
        /// Nalezne vlastn� ��sla matice
        /// </summary>
        /// <param name="matrix">Vstupn� �tvercov� matice</param>
        public static Vector dsyev(Matrix matrix) {
            int n = matrix.Length;

            double *a = PrepareMatrix(matrix);
            double* w = Memory.NewDouble(n);

            char uplo = 'U';
            char jobz = 'N';
            int lda = n;
            int lwork = -1;
            double* work = null;

            int info = 0;

            Vector result = null;

            try {
                double lwork1 = 0;

                // Nejprve zjist�me optim�ln� velikost pomocn�ho pole
                dsyev_(&jobz, &uplo, &n, a, &lda, w, &lwork1, &lwork, &info);

                lwork = (int)lwork1;
                work = Memory.NewDouble(lwork);

                // Vlastn� v�po�et
                dsyev_(&jobz, &uplo, &n, a, &lda, w, work, &lwork, &info);

                result = ProcessVector(w, n);
            }
            finally {
                Memory.Delete(a);
                Memory.Delete(w);
                Memory.Delete(work);
            }

            return result;
        }
    }
}

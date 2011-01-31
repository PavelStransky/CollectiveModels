using System;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Funkce z LAPack.dll
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
        /// Nalezne vlastn� ��sla matice
        /// </summary>
        /// <param name="matrix">Vstupn� �tvercov� matice</param>
        public static Vector[] dsyev(Matrix matrix, bool ev) {
            char jobz = ev ? 'V' : 'N';
            char uplo = 'U';

            int n = matrix.Length;

            double* a = PrepareMatrix(matrix);
            double* w = Memory.NewDouble(n);

            int lda = n;
            int lwork = -1;
            double* work = null;

            int info = 0;

            Vector[] result = null;
            bool success = false;

            try {
                double lwork1 = 0;

                // Nejprve zjist�me optim�ln� velikost pomocn�ho pole
                LAPackDLLWrapper.dsyev(&jobz, &uplo, &n, a, &lda, w, &lwork1, &lwork, &info);

                lwork = (int)lwork1;
                work = Memory.NewDouble(lwork);

                // Vlastn� v�po�et
                LAPackDLLWrapper.dsyev(&jobz, &uplo, &n, a, &lda, w, work, &lwork, &info);
                success = true;
            }
            finally {
                Memory.Delete(work);
            }

            // Zpracov�n� v�sledk�
            try {
                if(success) {
                    if(ev)
                        result = new Vector[n + 1];
                    else
                        result = new Vector[1];
                }

                result[0] = ProcessVector(w, n);

                if(ev)
                    for(int i = 0; i < n; i++) {
                        result[i + 1] = new Vector(n);
                        for(int j = 0; j < n; j++)
                            result[i + 1][j] = a[i * n + j];
                    }
            }
            finally {
                Memory.Delete(a);
                Memory.Delete(w);
            }

            return result;
        }

        /// <summary>
        /// Nalezne vlastn� p�sov� matice
        /// </summary>
        /// <param name="band">Matice v p�sov�m tvaru</param>
        /// <param name="ev">True, pokud chceme po��tat i vlastn� vektory</param>
        /// <param name="iMin">Minim�ln� vlastn� hodnota</param>
        /// <param name="iMax">Maxim�ln� vlastn� hodnota</param>
        public static Vector[] dsbevx(SymmetricBandMatrix band, bool ev, int iMin, int iMax) {
            char jobz = ev ? 'V' : 'N';
            char range = 'I';
            char uplo = 'U';

            int n = band.Length;
            int kd = band.NumSD;
            double *ab = band.GetItem();

            int ldab = kd + 1;

            double vl, vu;
            int il = iMin + 1, iu = iMax;

            // ! IL mus� b�t v�t�� ne� 0
            if(il <= 0)
                il = 1;
            if(iu > n)
                iu = n;

            // Pro jobz == 'N' nepo�ebujeme
            double* q = null;
            int ldq = 0;
            int* ifail = null;

            int ldz = 1;

            if(ev) {
                ldq = n;
                q = Memory.NewDouble(n * n);
                ifail = Memory.NewInt(n);
                ldz = n;
            }

            double* z = Memory.NewDouble(ldz * (iu - il + 1));

            double abstol = 0;

            int m = 0;
            double* w = Memory.NewDouble(n);

            double* work = Memory.NewDouble(7 * n);
            int* iwork = Memory.NewInt(5 * n);

            int info = 0;

            // V�po�et
            bool success = false;
            try {
                LAPackDLLWrapper.dsbevx(&jobz, &range, &uplo, &n, &kd, ab, &ldab, q, &ldq,
                    &vl, &vu, &il, &iu, &abstol, &m, w, z, &ldz, work, iwork, ifail, &info);
                success = true;
            }
            finally{
            Memory.Delete(work);
            Memory.Delete(iwork);
            Memory.Delete(q);
            Memory.Delete(ifail);
            }

            // Zpracov�n� v�sledk�
            Vector[] result = null;
            try {
                if(success) {
                    if(ev)
                        result = new Vector[m + 1];
                    else
                        result = new Vector[1];

                    result[0] = ProcessVector(w, m);

                    if(ev)
                        for(int i = 0; i < m; i++) {
                            result[i + 1] = new Vector(n);
                            for(int j = 0; j < n; j++)
                                result[i + 1][j] = z[i * ldz + j];
                        }
                }
            }
            finally {
                Memory.Delete(w);
                Memory.Delete(z);
            }

            return result;
        }
    }
}

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
        /// Matici pøevede na pole na nespravované haldì
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
        /// Pole na nespravované haldì pøevede na Vector
        /// </summary>
        /// <param name="v">Ukazatel na pole</param>
        /// <param name="n">Délka pole</param>
        private static Vector ProcessVector(double* v, int n) {
            Vector result = new Vector(n);

            for(int i = 0; i < n; i++)
                result[i] = v[i];

            return result;
        }

        #region dsyev
        /// <summary>
        /// DSYEV computes all eigenvalues and, optionally, eigenvectors of a
        /// real symmetric matrix A.
        /// </summary>
        [DllImport("LAPack.dll")]
        private static extern double dsyev_(char* jobz, char* uplo, int* n, double* a, int* lda,
            double* w, double* work, int* lwork, int* info);

        /// <summary>
        /// Nalezne vlastní èísla matice
        /// </summary>
        /// <param name="matrix">Vstupní ètvercová matice</param>
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

                // Nejprve zjistíme optimální velikost pomocného pole
                dsyev_(&jobz, &uplo, &n, a, &lda, w, &lwork1, &lwork, &info);

                lwork = (int)lwork1;
                work = Memory.NewDouble(lwork);

                // Vlastní výpoèet
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
        #endregion

        #region dsbevx
        /// <summary>        
        ///DSBEVX computes selected eigenvalues and, optionally, eigenvectors
        ///of a real symmetric band matrix A.  Eigenvalues and eigenvectors can
        ///be selected by specifying either a range of values or a range of
        ///indices for the desired eigenvalues.
        /// </summary>
        /// <param name="jobz">
        ///JOBZ    (input) CHARACTER*1
        ///        = 'N':  Compute eigenvalues only;
        ///        = 'V':  Compute eigenvalues and eigenvectors.
        /// </param>
        /// <param name="range">
        ///RANGE   (input) CHARACTER*1
        ///        = 'A': all eigenvalues will be found;
        ///        = 'V': all eigenvalues in the half-open interval (VL,VU]
        ///               will be found;
        ///        = 'I': the IL-th through IU-th eigenvalues will be found.
        /// </param>
        /// <param name="uplo">
        ///UPLO    (input) CHARACTER*1
        ///        = 'U':  Upper triangle of A is stored;
        ///        = 'L':  Lower triangle of A is stored.
        /// </param>
        /// <param name="n">
        ///N       (input) INTEGER
        ///        The order of the matrix A.  N geq 0.
        /// </param>
        /// <param name="kd">
        ///KD      (input) INTEGER
        ///        The number of superdiagonals of the matrix A if UPLO = 'U',
        ///        or the number of subdiagonals if UPLO = 'L'.  KD geq 0.
        /// </param>
        /// <param name="ab">
        ///AB      (input/output) DOUBLE PRECISION array, dimension (LDAB, N)
        ///        On entry, the upper or lower triangle of the symmetric band
        ///        matrix A, stored in the first KD+1 rows of the array.  The
        ///        j-th column of A is stored in the j-th column of the array AB
        ///        as follows:
        ///        if UPLO = 'U', AB(kd+1+i-j,j) = A(i,j) for max(1,j-kd) leq i leq j;
        ///        if UPLO = 'L', AB(1+i-j,j)    = A(i,j) for j leq i leq min(n,j+kd).
        /// *
        ///        On exit, AB is overwritten by values generated during the
        ///        reduction to tridiagonal form.  If UPLO = 'U', the first
        ///        superdiagonal and the diagonal of the tridiagonal matrix T
        ///        are returned in rows KD and KD+1 of AB, and if UPLO = 'L',
        ///        the diagonal and first subdiagonal of T are returned in the
        ///        first two rows of AB.
        /// </param>
        /// <param name="ldab">
        ///LDAB    (input) INTEGER
        ///        The leading dimension of the array AB.  LDAB geq KD + 1.
        /// </param>
        /// <param name="q">
        ///Q       (output) DOUBLE PRECISION array, dimension (LDQ, N)
        ///        If JOBZ = 'V', the N-by-N orthogonal matrix used in the
        ///                       reduction to tridiagonal form.
        ///        If JOBZ = 'N', the array Q is not referenced.
        /// </param>
        /// <param name="ldq">
        ///LDQ     (input) INTEGER
        ///        The leading dimension of the array Q.  If JOBZ = 'V', then
        ///        LDQ geq max(1,N).
        /// </param>
        /// <param name="vl">
        ///VL      (input) DOUBLE PRECISION
        ///VU      (input) DOUBLE PRECISION
        ///        If RANGE='V', the lower and upper bounds of the interval to
        ///        be searched for eigenvalues. VL l VU.
        ///        Not referenced if RANGE = 'A' or 'I'.
        /// </param>
        /// <param name="il">
        ///IL      (input) INTEGER
        ///IU      (input) INTEGER
        ///        If RANGE='I', the indices (in ascending order) of the
        ///        smallest and largest eigenvalues to be returned.
        ///        1 leq IL leq IU leq N, if N g 0; IL = 1 and IU = 0 if N = 0.
        ///        Not referenced if RANGE = 'A' or 'V'.
        /// </param>
        /// <param name="abstol">
        ///ABSTOL  (input) DOUBLE PRECISION
        ///        The absolute error tolerance for the eigenvalues.
        ///        An approximate eigenvalue is accepted as converged
        ///        when it is determined to lie in an interval [a,b]
        ///        of width less than or equal to
        ///*
        ///                ABSTOL + EPS  max( |a|,|b| ) ,
        ///*
        ///        where EPS is the machine precision.  If ABSTOL is less than
        ///        or equal to zero, then  EPS*|T|  will be used in its place,
        ///        where |T| is the 1-norm of the tridiagonal matrix obtained
        ///        by reducing AB to tridiagonal form.
        ///*
        ///        Eigenvalues will be computed most accurately when ABSTOL is
        ///        set to twice the underflow threshold 2*DLAMCH('S'), not zero.
        ///        If this routine returns with INFO g 0, indicating that some
        ///        eigenvectors did not converge, try setting ABSTOL to
        ///        2*DLAMCH('S').
        ///*
        ///        See "Computing Small Singular Values of Bidiagonal Matrices
        ///        with Guaranteed High Relative Accuracy," by Demmel and
        ///        Kahan, LAPACK Working Note #3.
        /// </param>
        /// <param name="m">
        ///M       (output) INTEGER
        ///        The total number of eigenvalues found.  0 leq M leq N.
        ///        If RANGE = 'A', M = N, and if RANGE = 'I', M = IU-IL+1.
        /// </param>
        /// <param name="w">
        ///W       (output) DOUBLE PRECISION array, dimension (N)
        ///        The first M elements contain the selected eigenvalues in
        ///        ascending order.
        /// </param>
        /// <param name="z">
        ///Z       (output) DOUBLE PRECISION array, dimension (LDZ, max(1,M))
        ///        If JOBZ = 'V', then if INFO = 0, the first M columns of Z
        ///        contain the orthonormal eigenvectors of the matrix A
        ///        corresponding to the selected eigenvalues, with the i-th
        ///        column of Z holding the eigenvector associated with W(i).
        ///        If an eigenvector fails to converge, then that column of Z
        ///        contains the latest approximation to the eigenvector, and the
        ///        index of the eigenvector is returned in IFAIL.
        ///        If JOBZ = 'N', then Z is not referenced.
        ///        Note: the user must ensure that at least max(1,M) columns are
        ///        supplied in the array Z; if RANGE = 'V', the exact value of M
        ///        is not known in advance and an upper bound must be used.
        /// </param>
        /// <param name="ldz">
        ///LDZ     (input) INTEGER
        ///        The leading dimension of the array Z.  LDZ geq 1, and if
        ///        JOBZ = 'V', LDZ geq max(1,N).
        /// </param>
        /// <param name="work">
        ///WORK    (workspace) DOUBLE PRECISION array, dimension (7*N)
        /// </param>
        /// <param name="iwork">
        ///IWORK   (workspace) INTEGER array, dimension (5*N)
        /// </param>
        /// <param name="ifail">
        ///IFAIL   (output) INTEGER array, dimension (N)
        ///        If JOBZ = 'V', then if INFO = 0, the first M elements of
        ///        IFAIL are zero.  If INFO g 0, then IFAIL contains the
        ///        indices of the eigenvectors that failed to converge.
        ///        If JOBZ = 'N', then IFAIL is not referenced.
        /// </param>
        /// <param name="info">
        ///INFO    (output) INTEGER
        ///        = 0:  successful exit.
        ///        l 0:  if INFO = -i, the i-th argument had an illegal value.
        ///        g 0:  if INFO = i, then i eigenvectors failed to converge.
        ///              Their indices are stored in array IFAIL.
        ///</param>
        [DllImport("LAPack.dll")]
        private static extern void dsbevx_(char* jobz, char* range, char* uplo,
            int* n, int* kd, double* ab, int* ldab,
            double* q, int* ldq,
            double* vl, double* vu, int* il, int* iu,
            double* abstol, int* m, double* w,
            double* z, int* ldz,
            double* work, int* iwork, int* ifail, int* info);

        /// <summary>
        /// Nalezne vlastní pásové matice
        /// </summary>
        /// <param name="band">Matice v pásovém tvaru</param>
        /// <param name="ev">True, pokud chceme poèítat i vlastní vektory</param>
        /// <param name="iMin">Minimální vlastní hodnota</param>
        /// <param name="iMax">Maximální vlastní hodnota</param>
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

            // ! IL musí být vìtší než 0
            if(il <= 0)
                il = 1;
            if(iu > n)
                iu = n;

            // Pro jobz == 'N' nepoøebujeme
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

            // Výpoèet
            bool success = false;
            try {
                dsbevx_(&jobz, &range, &uplo, &n, &kd, ab, &ldab, q, &ldq,
                    &vl, &vu, &il, &iu, &abstol, &m, w, z, &ldz, work, iwork, ifail, &info);
                success = true;
            }
            finally{
            Memory.Delete(work);
            Memory.Delete(iwork);
            Memory.Delete(q);
            Memory.Delete(ifail);
            }

            // Zpracování výsledkù
            Vector[] result = null;
            try {
                if(success) {
                    if(ev)
                        result = new Vector[m + 1];
                    else
                        result = new Vector[1];

                    result[0] = new Vector(m);

                    for(int i = 0; i < m; i++)
                        result[0][i] = w[i];

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
        #endregion
    }
}

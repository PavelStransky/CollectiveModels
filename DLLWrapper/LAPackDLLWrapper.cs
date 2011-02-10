using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Wrapper pro LAPack32.dll a LAPack64.dll
    /// Vybírá 32-bitovou a 64-bitovou verzi podle toho, jestli program bìží jako 32 nebo 64 bitový
    /// </summary>
    unsafe public class LAPackDLLWrapper {
        /// <summary>
        /// 32-bitová verze
        /// </summary>
        private class LAPack32 {
            /// <summary>
            /// DSYEV computes all eigenvalues and, optionally, eigenvectors of a
            /// real symmetric matrix A.
            /// </summary>
            [DllImport("LAPack32.dll")]
            public static extern double dsyev_(byte* jobz, byte* uplo, int* n, double* a, int* lda,
                double* w, double* work, int* lwork, int* info);

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
            [DllImport("LAPack32.dll")]
            public static extern void dsbevx_(byte* jobz, byte* range, byte* uplo,
                int* n, int* kd, double* ab, int* ldab,
                double* q, int* ldq,
                double* vl, double* vu, int* il, int* iu,
                double* abstol, int* m, double* w,
                double* z, int* ldz,
                double* work, int* iwork, int* ifail, int* info);
        }

        /// <summary>
        /// 64-bitová verze
        /// </summary>
        private class LAPack64 {
            [DllImport("LAPack64.dll")]
            public static extern double dsyev_(byte* jobz, byte* uplo, int* n, double* a, int* lda,
                double* w, double* work, int* lwork, int* info);

            [DllImport("LAPack64.dll")]
            public static extern void dsbevx_(byte* jobz, byte* range, byte* uplo,
                int* n, int* kd, double* ab, int* ldab,
                double* q, int* ldq,
                double* vl, double* vu, int* il, int* iu,
                double* abstol, int* m, double* w,
                double* z, int* ldz,
                double* work, int* iwork, int* ifail, int* info);
        }

        private static bool is32bit = true;

        public static void dsyev(byte* jobz, byte* uplo, int* n, double* a, int* lda,
                double* w, double* work, int* lwork, int* info) {
            if(is32bit)
                LAPack32.dsyev_(jobz, uplo, n, a, lda, w, work, lwork, info);
            else
                LAPack64.dsyev_(jobz, uplo, n, a, lda, w, work, lwork, info);
        }

        public static void dsbevx(byte* jobz, byte* range, byte* uplo,
            int* n, int* kd, double* ab, int* ldab,
            double* q, int* ldq,
            double* vl, double* vu, int* il, int* iu,
            double* abstol, int* m, double* w,
            double* z, int* ldz,
            double* work, int* iwork, int* ifail, int* info) {
            if(is32bit)
                LAPack32.dsbevx_(jobz, range, uplo, n, kd, ab, ldab, q, ldq, vl, vu, il, iu, abstol, m, w, z, ldz, work, iwork, ifail, info);
            else
                LAPack64.dsbevx_(jobz, range, uplo, n, kd, ab, ldab, q, ldq, vl, vu, il, iu, abstol, m, w, z, ldz, work, iwork, ifail, info);
        }

        /// <summary>
        /// Konstruktor, který rozhodne, zda bìžíme na 32 nebo 64 bitech
        /// </summary>
        static LAPackDLLWrapper() {
            if(IntPtr.Size == 4)
                is32bit = true;
            else
                is32bit = false;
        }
    }
}

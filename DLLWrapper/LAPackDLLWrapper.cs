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
            /// DGEEV computes for an N-by-N real nonsymmetric matrix A, the eigenvalues and, 
            /// optionally, the left and/or right eigenvectors.
            /// 
            /// The right eigenvector v(j) of A satisfies
            ///        A * v(j) = lambda(j) * v(j)
            ///  where lambda(j) is its eigenvalue.
            ///  The left eigenvector u(j) of A satisfies
            ///      u(j)**T * A = lambda(j) * u(j)**T
            /// where u(j)**T denotes the transpose of u(j).
            ///
            /// The computed eigenvectors are normalized to have Euclidean norm equal to 1 and largest component real.
            /// </summary>
            /// <param name="jobvl">
            /// JOBVL   (input) CHARACTER*1
            ///         = 'N': left eigenvectors of A are not computed;
            ///         = 'V': left eigenvectors of A are computed.
            /// </param>
            /// <param name="jobvr">
            /// JOBVR   (input) CHARACTER*1
            ///         = 'N': right eigenvectors of A are not computed;
            ///         = 'V': right eigenvectors of A are computed.
            /// </param>
            /// <param name="n">
            /// N       (input) INTEGER
            ///         The order of the matrix A. N >= 0.
            /// </param>
            /// <param name="a">
            /// A       (input/output) DOUBLE PRECISION array, dimension (LDA,N)
            ///         On entry, the N-by-N matrix A.
            ///         On exit, A has been overwritten.
            /// </param>
            /// <param name="lda">
            /// LDA     (input) INTEGER
            ///         The leading dimension of the array A.  LDA >= max(1,N).
            /// </param>
            /// <param name="wr">
            /// WR      (output) DOUBLE PRECISION array, dimension (N)
            ///         WR and WI contain the real and imaginary parts,
            ///         respectively, of the computed eigenvalues.  Complex
            ///         conjugate pairs of eigenvalues appear consecutively
            ///         with the eigenvalue having the positive imaginary part
            ///         first.            
            /// </param>
            /// <param name="wi">
            /// WI      (output) DOUBLE PRECISION array, dimension (N)
            ///         WR and WI contain the real and imaginary parts,
            ///         respectively, of the computed eigenvalues.  Complex
            ///         conjugate pairs of eigenvalues appear consecutively
            ///         with the eigenvalue having the positive imaginary part
            ///         first.            
            /// </param>
            /// <param name="vl">
            /// VL      (output) DOUBLE PRECISION array, dimension (LDVL,N)
            ///         If JOBVL = 'V', the left eigenvectors u(j) are stored one
            ///         after another in the columns of VL, in the same order
            ///         as their eigenvalues.
            ///         If JOBVL = 'N', VL is not referenced.
            ///         If the j-th eigenvalue is real, then u(j) = VL(:,j),
            ///         the j-th column of VL.
            ///         If the j-th and (j+1)-st eigenvalues form a complex
            ///         conjugate pair, then u(j) = VL(:,j) + i*VL(:,j+1) and
            ///         u(j+1) = VL(:,j) - i*VL(:,j+1).            
            /// </param>
            /// <param name="ldvl">
            /// LDVL    (input) INTEGER
            ///         The leading dimension of the array VL.  LDVL >= 1; if
            ///         JOBVL = 'V', LDVL >= N.
            /// </param>
            /// <param name="vr">
            /// VR      (output) DOUBLE PRECISION array, dimension (LDVR,N)
            ///         If JOBVR = 'V', the right eigenvectors v(j) are stored one
            ///         after another in the columns of VR, in the same order
            ///         as their eigenvalues.
            ///         If JOBVR = 'N', VR is not referenced.
            ///         If the j-th eigenvalue is real, then v(j) = VR(:,j),
            ///         the j-th column of VR.
            ///         If the j-th and (j+1)-st eigenvalues form a complex
            ///         conjugate pair, then v(j) = VR(:,j) + i*VR(:,j+1) and
            ///         v(j+1) = VR(:,j) - i*VR(:,j+1).
            /// </param>
            /// <param name="ldvr">
            /// LDVR    (input) INTEGER
            ///         The leading dimension of the array VR.  LDVR >= 1; if
            ///         JOBVR = 'V', LDVR >= N.
            /// </param>
            /// <param name="work">
            /// WORK    (workspace/output) DOUBLE PRECISION array, dimension (MAX(1,LWORK))
            ///         On exit, if INFO = 0, WORK(1) returns the optimal LWORK.            
            /// </param>
            /// <param name="lwork">
            /// LWORK   (input) INTEGER
            ///         The dimension of the array WORK.  LWORK >= max(1,3*N), and
            ///         if JOBVL = 'V' or JOBVR = 'V', LWORK >= 4*N.  For good
            ///         performance, LWORK must generally be larger.
            ///
            ///         If LWORK = -1, then a workspace query is assumed; the routine
            ///         only calculates the optimal size of the WORK array, returns
            ///         this value as the first entry of the WORK array, and no error
            ///         message related to LWORK is issued by XERBLA.
            /// </param>
            /// <param name="info">
            /// INFO    (output) INTEGER
            ///         = 0:  successful exit
            ///         l 0:  if INFO = -i, the i-th argument had an illegal value.
            ///         g 0:  if INFO = i, the QR algorithm failed to compute all the
            ///               eigenvalues, and no eigenvectors have been computed;
            ///               elements i+1:N of WR and WI contain eigenvalues which
            ///               have converged.
            /// </param>
            [DllImport("LaPack32.dll")]
            public static extern double dgeev_(byte* jobvl, byte* jobvr, int* n, double* a, int* lda,
                double* wr, double* wi, double* vl, int* ldvl, double* vr, int* ldvr, double* work, int* lwork, int* info);
            
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

            /// <summary>        
            /// DGESVD computes the singular value decomposition (SVD) of a real
            /// M-by-N matrix A, optionally computing the left and/or right singular
            /// vectors. The SVD is written
            ///        A = U * SIGMA * transpose(V) 
            /// where SIGMA is an M-by-N matrix which is zero except for its
            /// min(m,n) diagonal elements, U is an M-by-M orthogonal matrix, and
            /// V is an N-by-N orthogonal matrix.  The diagonal elements of SIGMA
            /// are the singular values of A; they are real and non-negative, and
            /// are returned in descending order.  The first min(m,n) columns of
            /// U and V are the left and right singular vectors of A.
            /// Note that the routine returns V**T, not V.
            /// </summary>
            /// <param name="jobu">
            ///JOBU    (input) CHARACTER*1
            ///        Specifies options for computing all or part of the matrix U:
            ///        = 'A': all M columns of U are returned in array U:
            ///        = 'S': the first min(m,n) columns of U (the left singular
            ///               vectors) are returned in the array U;
            ///        = 'O': the first min(m,n) columns of U (the left singular
            ///               vectors) are overwritten on the array A;
            ///        = 'N': no columns of U (no left singular vectors) are
            ///               computed.
            /// JOBVT and JOBU cannot both be 'O'.
            /// </param>
            /// <param name="jobvt">
            ///JOBVT   (input) CHARACTER*1
            ///        Specifies options for computing all or part of the matrix V**T:
            ///        = 'A': all N rows of V**T are returned in the array VT;
            ///        = 'S': the first min(m,n) rows of V**T (the right singular
            ///               vectors) are returned in the array VT;
            ///        = 'O': the first min(m,n) rows of V**T (the right singular
            ///               vectors) are overwritten on the array A;
            ///        = 'N': no rows of V**T (no right singular vectors) are computed.
            /// JOBVT and JOBU cannot both be 'O'.
            /// </param>
            /// <param name="m">
            ///M       (input) INTEGER
            ///        The number of rows of the input matrix A. M >= 0.
            /// </param>
            /// <param name="n">
            ///N       (input) INTEGER
            ///        The number of columns of the input matrix A. N >= 0.
            /// </param>
            /// <param name="a">
            ///A       (input/output) DOUBLE PRECISION array, dimension (LDA,N)
            ///        On entry, the M-by-N matrix A.
            ///        On exit,
            ///        if JOBU = 'O',  A is overwritten with the first min(m,n)
            ///                        columns of U (the left singular vectors,
            ///                        stored columnwise);
            ///        if JOBVT = 'O', A is overwritten with the first min(m,n)
            ///                        rows of V**T (the right singular vectors,
            ///                        stored rowwise);
            ///        if JOBU != 'O' and JOBVT != 'O', the contents of A
            ///                       are destroyed.
            /// </param>
            /// <param name="lda">
            ///LDA     (input) INTEGER
            ///        The leading dimension of the array A. LDA >= max(1,M).
            /// </param>
            /// <param name="s">
            ///S       (output) DOUBLE PRECISION array, dimension (min(M,N))
            ///        The singular values of A, sorted so that S(i) >= S(i+1).
            /// </param>
            /// <param name="u">
            ///U       (output) DOUBLE PRECISION array, dimension (LDU,UCOL)
            ///        (LDU,M) if JOBU = 'A' or (LDU,min(M,N)) if JOBU = 'S'.
            ///        If JOBU = 'A', U contains the M-by-M orthogonal matrix U;
            ///        if JOBU = 'S', U contains the first min(m,n) columns of U
            ///        (the left singular vectors, stored columnwise);
            ///        if JOBU = 'N' or 'O', U is not referenced.
            /// </param>
            /// <param name="ldu">
            ///LDU     (input) INTEGER
            ///        The leading dimension of the array U. LDU >= 1; if
            ///        JOBU = 'S' or 'A', LDU >= M.
            /// </param>
            /// <param name="vt">
            ///VT      (output) DOUBLE PRECISION array, dimension (LDVT,N)
            ///        If JOBVT = 'A', VT contains the N-by-N orthogonal matrix V**T;
            ///        if JOBVT = 'S', VT contains the first min(m,n) rows of
            ///        V**T (the right singular vectors, stored rowwise);
            ///        if JOBVT = 'N' or 'O', VT is not referenced.
            /// </param>
            /// <param name="ldvt">
            ///LDVT    (input) INTEGER
            ///        The leading dimension of the array VT.  LDVT >= 1; if
            ///        JOBVT = 'A', LDVT >= N; if JOBVT = 'S', LDVT >= min(M,N).
            /// </param>
            /// <param name="work">
            ///WORK    (workspace/output) DOUBLE PRECISION array, dimension (MAX(1,LWORK))
            ///        On exit, if INFO = 0, WORK(1) returns the optimal LWORK;
            ///        if INFO > 0, WORK(2:MIN(M,N)) contains the unconverged
            ///        superdiagonal elements of an upper bidiagonal matrix B
            ///        whose diagonal is in S (not necessarily sorted). B
            ///        satisfies A = U * B * VT, so it has the same singular values
            ///        as A, and singular vectors related by U and VT.
            /// </param>
            /// <param name="lwork">
            ///LWORK   (input) INTEGER
            ///        The dimension of the array WORK.
            ///        LWORK >= MAX(1,5*MIN(M,N)) for the paths (see comments inside code):
            ///         - PATH 1  (M much larger than N, JOBU='N') 
            ///         - PATH 1t (N much larger than M, JOBVT='N')
            ///        LWORK >= MAX(1,3*MIN(M,N)+MAX(M,N),5*MIN(M,N)) for the other paths
            ///        For good performance, LWORK should generally be larger.
            ///        If LWORK = -1, then a workspace query is assumed; the routine
            ///        only calculates the optimal size of the WORK array, returns
            ///        this value as the first entry of the WORK array, and no error
            ///        message related to LWORK is issued by XERBLA.
            /// </param>
            /// <param name="info">
            ///INFO    (output) INTEGER
            ///        = 0:  successful exit.
            ///        < 0:  if INFO = -i, the i-th argument had an illegal value.
            ///        > 0:  if DBDSQR did not converge, INFO specifies how many
            ///        superdiagonals of an intermediate bidiagonal form B
            ///        did not converge to zero. See the description of WORK
            ///        above for details.
            /// </param>
            [DllImport("LAPack32.dll")]
            public static extern void dgesvd_(byte* jobu, byte* jobvt, 
                int* m, int* n, double* a, 
                int* lda, double* s, double* u,
                int* ldu, double* vt,
                int* ldvt, double* work, int* lwork, int* info);        
        }

        /// <summary>
        /// 64-bitová verze
        /// </summary>
        private class LAPack64 {
            [DllImport("LaPack64.dll")]
            public static extern double dgeev_(byte* jobvl, byte* jobvr, int* n, double* a, int* lda,
                double* wr, double* wi, double* vl, int* ldvl, double* vr, int* ldvr, double* work, int* lwork, int* info);

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

            [DllImport("LAPack64.dll")]
            public static extern void dgesvd_(byte* jobu, byte* jobvt,
                int* m, int* n, double* a,
                int* lda, double* s, double* u,
                int* ldu, double* vt,
                int* ldvt, double* work, int* lwork, int* info);
        }

        private static bool is32bit = true;

        public static void dgeev(byte* jobvl, byte* jobvr, int* n, double* a, int* lda,
                double* wr, double* wi, double* vl, int* ldvl, double* vr, int* ldvr, double* work, int* lwork, int* info) {
            if(is32bit)
                LAPack32.dgeev_(jobvl, jobvr, n, a, lda, wr, wi, vl, ldvl, vr, ldvr, work, lwork, info);
            else
                LAPack64.dgeev_(jobvl, jobvr, n, a, lda, wr, wi, vl, ldvl, vr, ldvr, work, lwork, info);
        }

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

        public static void dgesvd(byte* jobu, byte* jobvt,
                int* m, int* n, double* a,
                int* lda, double* s, double* u,
                int* ldu, double* vt,
                int* ldvt, double* work, int* lwork, int* info) {
            if(is32bit)
                LAPack32.dgesvd_(jobu, jobvt, m, n, a, lda, s, u, ldu, vt, ldvt, work, lwork, info);
            else
                LAPack64.dgesvd_(jobu, jobvt, m, n, a, lda, s, u, ldu, vt, ldvt, work, lwork, info);
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

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.DLLWrapper {
    /// <summary>
    /// Wrapper pro ARPack32.dll a ARPack64.dll
    /// Vybírá 32-bitovou a 64-bitovou verzi podle toho, jestli program bìží jako 32 nebo 64 bitový
    /// </summary>
    unsafe public class ARPackDLLWrapper {
        /// <summary>
        /// 32-bitová verze
        /// </summary>
        private class ARPack32 {
            /// <summary>
            /// Reverse communication interface for the Implicitly Restarted Arnoldi Iteration.  
            /// For symmetric problems this reduces to a variant of the Lanczos method.  
            /// This method has been designed to compute approximations to a few eigenpairs 
            /// of a linear operator OP that is real and symmetric with respect to a real 
            /// positive semi-definite symmetric matrix B, i.e.
            /// 
            ///         B*OP = (OP')*B.  
            /// 
            /// Another way to express this condition is 
            /// 
            ///         [ x,OPy ] = [ OPx,y ]  where [ z,w ] = z'Bw.
            /// 
            /// In the standard eigenproblem B is the identity matrix ( A' denotes transpose of A).
            /// 
            /// The computed approximate eigenvalues are called Ritz values and the corresponding 
            /// approximate eigenvectors are called Ritz vectors.
            /// 
            /// dsaupd is usually called iteratively to solve one of the following problems:
            /// 
            /// Mode 1:  A*x = lambda*x, A symmetric 
            ///          === OP = A  and  B = I.
            /// 
            /// Mode 2:  A*x = lambda*M*x, A symmetric, M symmetric positive definite
            ///          === OP = inv[M]*A  and  B = M.
            ///          === (If M can be factored see remark 3 below)
            /// 
            /// Mode 3:  K*x = lambda*M*x, K symmetric, M symmetric positive semi-definite
            ///          === OP = (inv[K - sigma*M])*M  and  B = M. 
            ///          === Shift-and-Invert mode
            /// 
            /// Mode 4:  K*x = lambda*KG*x, K symmetric positive semi-definite, KG symmetric indefinite
            ///          === OP = (inv[K - sigma*KG])*K  and  B = K.
            ///          === Buckling mode
            /// Mode 5:  A*x = lambda*M*x, A symmetric, M symmetric positive semi-definite
            ///          === OP = inv[A - sigma*M]*[A + sigma*M]  and  B = M.
            ///          === Cayley transformed mode
            /// 
            /// NOTE: The action of w = inv[A - sigma*M]*v or w = inv[M]*v 
            /// should be accomplished either by a direct method using a sparse matrix factorization and solving
            /// 
            ///         [A - sigma*M]*w = v  or M*w = v,
            /// 
            /// or through an iterative method for solving these systems.  
            /// If an iterative method is used, the convergence test must be more stringent 
            /// than the accuracy requirements for the eigenvalue approximations.
            /// </summary>
            /// <param name="ido">IDO Integer (INPUT/OUTPUT)
            /// Reverse communication flag. IDO must be zero on the first call to dsaupd.
            /// IDO will be set internally to indicate the type of operation to be performed.
            /// Control is then given back to the calling routine which has the responsibility 
            /// to carry out the requested operation and call dsaupd with the result.  
            /// The operand is given in
            /// WORKD(IPNTR(1)), the result must be put in WORKD(IPNTR(2)).
            /// (If Mode = 2 see remark 5 below)
            /// ------------------------------------------------------------------------------
            /// IDO =  0: first call to the reverse communication interface
            /// IDO = -1: compute  Y = OP * X  where
            ///           IPNTR(1) is the pointer into WORKD for X,
            ///           IPNTR(2) is the pointer into WORKD for Y.
            ///           This is for the initialization phase to force the
            ///           starting vector into the range of OP.
            /// IDO =  1: compute  Y = OP * Z  and Z = B * X where
            ///           IPNTR(1) is the pointer into WORKD for X,
            ///           IPNTR(2) is the pointer into WORKD for Y,
            ///           IPNTR(3) is the pointer into WORKD for Z.
            /// IDO =  2: compute  Y = B * X  where
            ///           IPNTR(1) is the pointer into WORKD for X,
            ///           IPNTR(2) is the pointer into WORKD for Y.
            /// IDO =  3: compute the IPARAM(8) shifts where
            ///           IPNTR(11) is the pointer into WORKL for placing the shifts. See remark 6 below.
            /// IDO = 99: done
            /// ------------------------------------------------------------------------------
            /// After the initialization phase, when the routine is used in either 
            /// the "shift-and-invert" mode or the Cayley transform mode, 
            /// the vector B * X is already available and does not need to be 
            /// recomputed in forming OP*X. 
            /// </param>
            /// <param name="bmat">BMAT Character(1) (INPUT)
            /// BMAT specifies the type of the matrix B that defines 
            /// the semi-inner product for the operator OP.
            /// B = 'I' - standard eigenvalue problem A*x = lambda*x
            /// B = 'G' - generalized eigenvalue problem A*x = lambda*B*x
            /// </param>
            /// <param name="n">N Integer (INPUT)
            /// Dimension of the eigenproblem.
            /// </param>
            /// <param name="which">WHICH Character(2) (INPUT)
            /// Specify which of the Ritz values of OP to compute.
            /// 
            /// 'LA' - compute the NEV largest (algebraic) eigenvalues.
            /// 'SA' - compute the NEV smallest (algebraic) eigenvalues.
            /// 'LM' - compute the NEV largest (in magnitude) eigenvalues.
            /// 'SM' - compute the NEV smallest (in magnitude) eigenvalues. 
            /// 'BE' - compute NEV eigenvalues, half from each end of the spectrum.  
            ///        When NEV is odd, compute one more from the high end than from the low end.
            /// (see remark 1 below)
            /// </param>
            /// <param name="nev">NEV Integer. (INPUT)
            /// Number of eigenvalues of OP to be computed. 0 .L. NEV .L. N.
            /// </param>
            /// <param name="tol">TOL Double precision scalar. (INPUT)
            /// Stopping criterion: the relative accuracy of the Ritz value is considered 
            /// acceptable if BOUNDS(I) .LE. TOL*ABS(RITZ(I)).
            /// If TOL .LE. 0. is passed a default is set:
            /// DEFAULT = DLAMCH('EPS')  (machine precision as computed
            ///           by the LAPACK auxiliary subroutine DLAMCH).
            /// </param>
            /// <param name="resid">RESID Double precision(N) (INPUT/OUTPUT)
            /// On INPUT: 
            /// If INFO .EQ. 0, a random initial residual vector is used.
            /// If INFO .NE. 0, RESID contains the initial residual vector, possibly from a previous run.
            /// On OUTPUT:
            /// RESID contains the final residual vector. 
            /// </param>
            /// <param name="ncv">NCV Integer (INPUT)
            /// Number of columns of the matrix V (less than or equal to N). This will indicate 
            /// how many Lanczos vectors are generated at each iteration. After the startup phase 
            /// in which NEV Lanczos vectors are generated, the algorithm generates NCV-NEV 
            /// Lanczos vectors at each subsequent update iteration. Most of the cost in generating 
            /// each Lanczos vector is in the matrix-vector product OP*x. 
            /// (See remark 4 below).
            /// </param>
            /// <param name="v">c  V Double precision(N by NCV) (OUTPUT)
            /// The NCV columns of V contain the Lanczos basis vectors.
            /// </param>
            /// <param name="ldv">LDV Integer (INPUT)
            /// Leading dimension of V exactly as declared in the calling program.
            /// </param>
            /// <param name="iparam">IPARAM Integer (11) (INPUT/OUTPUT)
            /// IPARAM(1) = ISHIFT: method for selecting the implicit shifts.
            /// The shifts selected at each iteration are used to restart the Arnoldi iteration in an implicit fashion.
            /// ------------------------------------------------------------------------------
            /// ISHIFT = 0: the shifts are provided by the user via reverse communication.  
            ///             The NCV eigenvalues of the current tridiagonal matrix T are returned in
            ///             the part of WORKL array corresponding to RITZ.
            ///             See remark 6 below.
            /// ISHIFT = 1: exact shifts with respect to the reduced tridiagonal matrix T.
            ///             This is equivalent to restarting the iteration with a starting vector 
            ///             that is a linear combination of Ritz vectors associated with the "wanted" Ritz values.
            /// ------------------------------------------------------------------------------
            /// IPARAM(2) = LEVEC
            /// No longer referenced. See remark 2 below.
            /// 
            /// IPARAM(3) = MXITER
            /// On INPUT:  maximum number of Arnoldi update iterations allowed. 
            /// On OUTPUT: actual number of Arnoldi update iterations taken. 
            /// 
            /// IPARAM(4) = NB: blocksize to be used in the recurrence.
            /// The code currently works only for NB = 1.
            /// 
            /// IPARAM(5) = NCONV: number of "converged" Ritz values.
            /// This represents the number of Ritz values that satisfy the convergence criterion.
            /// 
            /// IPARAM(6) = IUPD
            /// No longer referenced. Implicit restarting is ALWAYS used. 
            /// 
            /// IPARAM(7) = MODE
            /// On INPUT determines what type of eigenproblem is being solved.
            /// Must be 1,2,3,4,5; See under \Description of dsaupd for the five modes available.
            /// 
            /// IPARAM(8) = NP
            /// When ido = 3 and the user provides shifts through reverse communication (IPARAM(1)=0), 
            /// dsaupd returns NP, the number of shifts the user is to provide. 0 .L. NP .LE. NCV-NEV.
            /// See Remark 6 below.
            /// 
            /// IPARAM(9) = NUMOP, IPARAM(10) = NUMOPB, IPARAM(11) = NUMREO,
            /// OUTPUT: NUMOP  = total number of OP*x operations,
            ///         NUMOPB = total number of B*x operations if BMAT='G',
            ///         NUMREO = total number of steps of re-orthogonalization.       
            /// </param>
            /// <param name="ipntr">IPNTR Integer (11) (OUTPUT)
            /// Pointer to mark the starting locations in the WORKD and WORKL arrays 
            /// for matrices/vectors used by the Lanczos iteration.
            /// IPNTR(1): pointer to the current operand vector X in WORKD.
            /// IPNTR(2): pointer to the current result vector Y in WORKD.
            /// IPNTR(3): pointer to the vector B * X in WORKD when used in the shift-and-invert mode.
            /// IPNTR(4): pointer to the next available location in WORKL that is untouched by the program.
            /// IPNTR(5): pointer to the NCV by 2 tridiagonal matrix T in WORKL.
            /// IPNTR(6): pointer to the NCV RITZ values array in WORKL.
            /// IPNTR(7): pointer to the Ritz estimates in array WORKL associated with the Ritz values 
            ///           located in RITZ in WORKL.
            /// Note: IPNTR(8:10) is only referenced by dseupd. See Remark 2.
            /// IPNTR(8): pointer to the NCV RITZ values of the original system.
            /// IPNTR(9): pointer to the NCV corresponding error bounds.
            /// IPNTR(10): pointer to the NCV by NCV matrix of eigenvectors of the tridiagonal matrix T. 
            ///            Only referenced by dseupd if RVEC = .TRUE. See Remarks.
            /// Note: IPNTR(8:10) is only referenced by dseupd. See Remark 2.
            /// IPNTR(11): pointer to the NP shifts in WORKL. See Remark 6 below.
            /// </param>
            /// <param name="workd">WORKD Double precision (3*N) (REVERSE COMMUNICATION)
            /// Distributed array to be used in the basic Arnoldi iteration for reverse communication.  
            /// The user should not use WORKD as temporary workspace during the iteration. 
            /// Upon termination WORKD(1:N) contains B*RESID(1:N). If the Ritz vectors are desired
            /// subroutine dseupd uses this output. See Data Distribution Note below.  
            /// </param>
            /// <param name="workl">WORKL Double precision (LWORKL) (OUTPUT/WORKSPACE)
            /// Private (replicated) array on each PE or array allocated on the front end.  
            /// See Data Distribution Note below.
            /// </param>
            /// <param name="lworkl">LWORKL Integer (INPUT)
            /// LWORKL must be at least NCV**2 + 8*NCV.
            /// </param>
            /// <param name="info">INFO Integer (INPUT/OUTPUT)
            /// If INFO .EQ. 0, a randomly initial residual vector is used.
            /// If INFO .NE. 0, RESID contains the initial residual vector, possibly from a previous run.
            /// Error flag on output.
            /// =  0: Normal exit.
            /// =  1: Maximum number of iterations taken. All possible eigenvalues 
            ///       of OP has been found. IPARAM(5) returns the number of wanted converged Ritz values.
            /// =  2: No longer an informational error. Deprecated starting with release 2 of ARPACK.
            /// =  3: No shifts could be applied during a cycle of the Implicitly restarted Arnoldi iteration. 
            ///       One possibility is to increase the size of NCV relative to NEV. 
            ///       See remark 4 below.
            /// = -1: N must be positive.
            /// = -2: NEV must be positive.
            /// = -3: NCV must be greater than NEV and less than or equal to N.
            /// = -4: The maximum number of Arnoldi update iterations allowed must be greater than zero.
            /// = -5: WHICH must be one of 'LM', 'SM', 'LA', 'SA' or 'BE'.
            /// = -6: BMAT must be one of 'I' or 'G'.
            /// = -7: Length of private work array WORKL is not sufficient.
            /// = -8: Error return from trid. eigenvalue calculation; Informational error from LAPACK routine dsteqr.
            /// = -9: Starting vector is zero.
            /// = -10: IPARAM(7) must be 1,2,3,4,5.
            /// = -11: IPARAM(7) = 1 and BMAT = 'G' are incompatable.
            /// = -12: IPARAM(1) must be equal to 0 or 1.
            /// = -13: NEV and WHICH = 'BE' are incompatable.
            /// = -9999: Could not build an Arnoldi factorization. IPARAM(5) returns the size of the current 
            ///          Arnoldi factorization. The user is advised to check that enough workspace 
            ///          and array storage has been allocated.
            /// </param>
            /// <remarks>
            /// 1. The converged Ritz values are always returned in ascending algebraic order.  
            /// The computed Ritz values are approximate eigenvalues of OP.  
            /// The selection of WHICH should be made with this in mind when Mode = 3,4,5.  
            /// After convergence, approximate eigenvalues of the original problem may be obtained 
            /// with the ARPACK subroutine dseupd. 
            /// 
            /// 2. If the Ritz vectors corresponding to the converged Ritz values are needed, 
            /// the user must call dseupd immediately following completion of dsaupd. 
            /// This is new starting with version 2.1 of ARPACK.
            /// 
            /// 3. If M can be factored into a Cholesky factorization M = LL' then Mode = 2 
            /// should not be selected. Instead one should use Mode = 1 with OP = inv(L)*A*inv(L'). 
            /// Appropriate triangular linear systems should be solved with L and L' rather than 
            /// computing inverses. After convergence, an approximate eigenvector z of the original problem 
            /// is recovered by solving L'z = x  where x is a Ritz vector of OP.
            /// 
            /// 4. At present there is no a-priori analysis to guide the selection of NCV relative to NEV.  
            /// The only formal requirement is that NCV .G. NEV. However, it is recommended 
            /// that NCV .ge. 2*NEV. If many problems of the same type are to be solved, 
            /// one should experiment with increasing NCV while keeping NEV fixed for a given test problem.  
            /// This will usually decrease the required number of OP*x operations but it also increases 
            /// the work and storage required to maintain the orthogonal basis vectors. The optimal "cross-over" 
            /// with respect to CPU time is problem dependent and must be determined empirically.
            /// 
            /// 5. If IPARAM(7) = 2 then in the Reverse communication interface the user must do the following. 
            /// When IDO = 1, Y = OP * X is to be computed.
            /// When IPARAM(7) = 2 OP = inv(B)*A. After computing A*X the user must overwrite X with A*X. 
            /// Y is then the solution to the linear set of equations B*Y = A*X.
            /// 
            /// 6. When IPARAM(1) = 0, and IDO = 3, the user needs to provide the NP = IPARAM(8) shifts in locations: 
            /// 1   WORKL(IPNTR(11))           
            /// 2   WORKL(IPNTR(11)+1)         
            ///         .           
            ///         .           
            ///         .      
            /// NP  WORKL(IPNTR(11)+NP-1). 
            /// 
            /// The eigenvalues of the current tridiagonal matrix are located in WORKL(IPNTR(6)) 
            /// through WORKL(IPNTR(6)+NCV-1). They are in the order defined by WHICH. The associated 
            /// Ritz estimates are located in WORKL(IPNTR(8)), WORKL(IPNTR(8)+1), ... , WORKL(IPNTR(8)+NCV-1).
            /// </remarks>
            [DllImport("ARPack32.dll")]
            public static extern void dsaupd_(int* ido, byte* bmat, int* n, byte* which,
                int* nev, double* tol, double* resid, int* ncv, double* v, int* ldv,
                int* iparam, int* ipntr, double* workd, double* workl, int* lworkl, int* info);

            /// <summary>
            /// DSEUPD returns the converged approximations to eigenvalues of A*z = lambda*B*z and (optionally):
            /// 
            ///    (1) the corresponding approximate eigenvectors,
            ///    (2) an orthonormal (Lanczos) basis for the associated approximate invariant subspace,
            ///    (3) Both.
            /// 
            /// There is negligible additional cost to obtain eigenvectors.  An orthonormal (Lanczos) 
            /// basis is always computed. There is an additional storage cost of n*nev if both are requested 
            /// (in this case a separate array Z must be supplied).
            /// 
            /// These quantities are obtained from the Lanczos factorization computed by DSAUPD for the 
            /// linear operator OP prescribed by the MODE selection (see IPARAM(7) in DSAUPD documentation.)  
            /// DSAUPD must be called before this routine is called. These approximate eigenvalues 
            /// and vectors are commonly called Ritz values and Ritz vectors respectively.  They are
            /// referred to as such in the comments that follow. The computed orthonormal basis 
            /// for the invariant subspace corresponding to these Ritz values is referred to as a Lanczos basis.
            /// 
            /// See documentation in the header of the subroutine DSAUPD for a definition of OP as well as 
            /// other terms and the relation of computed Ritz values and vectors of OP with respect to 
            /// the given problem  A*z = lambda*B*z.
            /// 
            /// The approximate eigenvalues of the original problem are returned in ascending algebraic order.  
            /// The user may elect to call this routine once for each desired Ritz vector and store it 
            /// peripherally if desired. There is also the option of computing a selected set of these vectors
            /// with a single call.
            /// </summary>
            /// <param name="rvec">RVEC LOGICAL (INPUT)
            /// Specifies whether Ritz vectors corresponding to the Ritz value approximations to 
            /// the eigenproblem A*z = lambda*B*z are computed.
            /// RVEC = .FALSE.     Compute Ritz values only.
            /// RVEC = .TRUE.      Compute Ritz vectors.
            /// </param>
            /// <param name="all">All STRING "All"</param>
            /// <param name="select">SELECT Logical (NCV) (INPUT/WORKSPACE)
            /// If HOWMNY = 'S', SELECT specifies the Ritz vectors to be computed. To select 
            /// the Ritz vector corresponding to a Ritz value D(j), SELECT(j) must be set to .TRUE.. 
            /// If HOWMNY = 'A' , SELECT is used as a workspace for reordering the Ritz values.
            /// </param>
            /// <param name="d">D Double precision (NEV). (OUTPUT)
            /// On exit, D contains the Ritz value approximations to the eigenvalues of A*z = lambda*B*z. 
            /// The values are returned in ascending order. If IPARAM(7) = 3,4,5 then D represents
            /// the Ritz values of OP computed by DSAUPD transformed to those of the original 
            /// eigensystem A*z = lambda*B*z. If IPARAM(7) = 1,2 then the Ritz values of OP are the same
            /// as the those of A*z = lambda*B*z.
            /// </param>
            /// <param name="z">Z Double precision (N by NEV) if HOWMNY = 'A'. (OUTPUT)
            /// On exit, Z contains the B-orthonormal Ritz vectors of the eigensystem A*z = lambda*B*z 
            /// corresponding to the Ritz value approximations. If  RVEC = .FALSE. then Z is not referenced.
            /// NOTE: The array Z may be set equal to first NEV columns of the Arnoldi/Lanczos basis 
            /// array V computed by DSAUPD.
            /// </param>
            /// <param name="ldz">LDZ Integer. (INPUT)
            /// The leading dimension of the array Z.  If Ritz vectors are desired, then  LDZ .ge.  max( 1, N ).  
            /// In any case, LDZ .ge. 1.
            /// </param>
            /// <param name="sigma">SIGMA Double precision (INPUT)
            /// If IPARAM(7) = 3,4,5 represents the shift. Not referenced if IPARAM(7) = 1 or 2.
            /// </param>
            /// <param name="bmat">BMAT Character(1) (INPUT)
            /// BMAT specifies the type of the matrix B that defines 
            /// the semi-inner product for the operator OP.
            /// B = 'I' - standard eigenvalue problem A*x = lambda*x
            /// B = 'G' - generalized eigenvalue problem A*x = lambda*B*x
            /// </param>
            /// <param name="n">N Integer (INPUT)
            /// Dimension of the eigenproblem.
            /// </param>
            /// <param name="which">WHICH Character(2) (INPUT)
            /// Specify which of the Ritz values of OP to compute.
            /// 
            /// 'LA' - compute the NEV largest (algebraic) eigenvalues.
            /// 'SA' - compute the NEV smallest (algebraic) eigenvalues.
            /// 'LM' - compute the NEV largest (in magnitude) eigenvalues.
            /// 'SM' - compute the NEV smallest (in magnitude) eigenvalues. 
            /// 'BE' - compute NEV eigenvalues, half from each end of the spectrum.  
            ///        When NEV is odd, compute one more from the high end than from the low end.
            /// (see remark 1 below)
            /// </param>
            /// <param name="nev">NEV Integer. (INPUT)
            /// Number of eigenvalues of OP to be computed. 0 .L. NEV .L. N.
            /// </param>
            /// <param name="tol">TOL Double precision scalar. (INPUT)
            /// Stopping criterion: the relative accuracy of the Ritz value is considered 
            /// acceptable if BOUNDS(I) .LE. TOL*ABS(RITZ(I)).
            /// If TOL .LE. 0. is passed a default is set:
            /// DEFAULT = DLAMCH('EPS')  (machine precision as computed
            ///           by the LAPACK auxiliary subroutine DLAMCH).
            /// </param>
            /// <param name="resid">RESID Double precision(N) (INPUT/OUTPUT)
            /// On INPUT: 
            /// If INFO .EQ. 0, a random initial residual vector is used.
            /// If INFO .NE. 0, RESID contains the initial residual vector, possibly from a previous run.
            /// On OUTPUT:
            /// RESID contains the final residual vector. 
            /// </param>
            /// <param name="ncv">NCV Integer (INPUT)
            /// Number of columns of the matrix V (less than or equal to N). This will indicate 
            /// how many Lanczos vectors are generated at each iteration. After the startup phase 
            /// in which NEV Lanczos vectors are generated, the algorithm generates NCV-NEV 
            /// Lanczos vectors at each subsequent update iteration. Most of the cost in generating 
            /// each Lanczos vector is in the matrix-vector product OP*x. 
            /// (See remark 4 below).
            /// </param>
            /// <param name="v">c  V Double precision(N by NCV) (OUTPUT)
            /// The NCV columns of V contain the Lanczos basis vectors.
            /// </param>
            /// <param name="ldv">LDV Integer (INPUT)
            /// Leading dimension of V exactly as declared in the calling program.
            /// </param>
            /// <param name="iparam">IPARAM Integer (11) (INPUT/OUTPUT)
            /// IPARAM(1) = ISHIFT: method for selecting the implicit shifts.
            /// The shifts selected at each iteration are used to restart the Arnoldi iteration in an implicit fashion.
            /// ------------------------------------------------------------------------------
            /// ISHIFT = 0: the shifts are provided by the user via reverse communication.  
            ///             The NCV eigenvalues of the current tridiagonal matrix T are returned in
            ///             the part of WORKL array corresponding to RITZ.
            ///             See remark 6 below.
            /// ISHIFT = 1: exact shifts with respect to the reduced tridiagonal matrix T.
            ///             This is equivalent to restarting the iteration with a starting vector 
            ///             that is a linear combination of Ritz vectors associated with the "wanted" Ritz values.
            /// ------------------------------------------------------------------------------
            /// IPARAM(2) = LEVEC
            /// No longer referenced. See remark 2 below.
            /// 
            /// IPARAM(3) = MXITER
            /// On INPUT:  maximum number of Arnoldi update iterations allowed. 
            /// On OUTPUT: actual number of Arnoldi update iterations taken. 
            /// 
            /// IPARAM(4) = NB: blocksize to be used in the recurrence.
            /// The code currently works only for NB = 1.
            /// 
            /// IPARAM(5) = NCONV: number of "converged" Ritz values.
            /// This represents the number of Ritz values that satisfy the convergence criterion.
            /// 
            /// IPARAM(6) = IUPD
            /// No longer referenced. Implicit restarting is ALWAYS used. 
            /// 
            /// IPARAM(7) = MODE
            /// On INPUT determines what type of eigenproblem is being solved.
            /// Must be 1,2,3,4,5; See under \Description of dsaupd for the five modes available.
            /// 
            /// IPARAM(8) = NP
            /// When ido = 3 and the user provides shifts through reverse communication (IPARAM(1)=0), 
            /// dsaupd returns NP, the number of shifts the user is to provide. 0 .L NP .LE. NCV-NEV. 
            /// See Remark 6 below.
            /// 
            /// IPARAM(9) = NUMOP, IPARAM(10) = NUMOPB, IPARAM(11) = NUMREO,
            /// OUTPUT: NUMOP  = total number of OP*x operations,
            ///         NUMOPB = total number of B*x operations if BMAT='G',
            ///         NUMREO = total number of steps of re-orthogonalization.       
            /// </param>
            /// <param name="ipntr">IPNTR Integer (11) (OUTPUT)
            /// Pointer to mark the starting locations in the WORKD and WORKL arrays 
            /// for matrices/vectors used by the Lanczos iteration.
            /// IPNTR(1): pointer to the current operand vector X in WORKD.
            /// IPNTR(2): pointer to the current result vector Y in WORKD.
            /// IPNTR(3): pointer to the vector B * X in WORKD when used in the shift-and-invert mode.
            /// IPNTR(4): pointer to the next available location in WORKL that is untouched by the program.
            /// IPNTR(5): pointer to the NCV by 2 tridiagonal matrix T in WORKL.
            /// IPNTR(6): pointer to the NCV RITZ values array in WORKL.
            /// IPNTR(7): pointer to the Ritz estimates in array WORKL associated with the Ritz values 
            ///           located in RITZ in WORKL.
            /// Note: IPNTR(8:10) is only referenced by dseupd. See Remark 2.
            /// IPNTR(8): pointer to the NCV RITZ values of the original system.
            /// IPNTR(9): pointer to the NCV corresponding error bounds.
            /// IPNTR(10): pointer to the NCV by NCV matrix of eigenvectors of the tridiagonal matrix T. 
            ///            Only referenced by dseupd if RVEC = .TRUE. See Remarks.
            /// Note: IPNTR(8:10) is only referenced by dseupd. See Remark 2.
            /// IPNTR(11): pointer to the NP shifts in WORKL. See Remark 6 below.
            /// </param>
            /// <param name="workd">WORKD Double precision (3*N) (REVERSE COMMUNICATION)
            /// Distributed array to be used in the basic Arnoldi iteration for reverse communication.  
            /// The user should not use WORKD as temporary workspace during the iteration. 
            /// Upon termination WORKD(1:N) contains B*RESID(1:N). If the Ritz vectors are desired
            /// subroutine dseupd uses this output. See Data Distribution Note below.  
            /// </param>
            /// <param name="workl">WORKL Double precision (LWORKL) (OUTPUT/WORKSPACE)
            /// Private (replicated) array on each PE or array allocated on the front end.  
            /// See Data Distribution Note below.
            /// </param>
            /// <param name="lworkl">LWORKL Integer (INPUT)
            /// LWORKL must be at least NCV**2 + 8*NCV.
            /// </param>
            /// <param name="info">INFO Integer (INPUT/OUTPUT)
            /// If INFO .EQ. 0, a randomly initial residual vector is used.
            /// If INFO .NE. 0, RESID contains the initial residual vector, possibly from a previous run.
            /// Error flag on output.
            /// =  0: Normal exit.
            /// =  1: Maximum number of iterations taken. All possible eigenvalues 
            ///       of OP has been found. IPARAM(5) returns the number of wanted converged Ritz values.
            /// =  2: No longer an informational error. Deprecated starting with release 2 of ARPACK.
            /// =  3: No shifts could be applied during a cycle of the Implicitly restarted Arnoldi iteration. 
            ///       One possibility is to increase the size of NCV relative to NEV. 
            ///       See remark 4 below.
            /// = -1: N must be positive.
            /// = -2: NEV must be positive.
            /// = -3: NCV must be greater than NEV and less than or equal to N.
            /// = -4: The maximum number of Arnoldi update iterations allowed must be greater than zero.
            /// = -5: WHICH must be one of 'LM', 'SM', 'LA', 'SA' or 'BE'.
            /// = -6: BMAT must be one of 'I' or 'G'.
            /// = -7: Length of private work array WORKL is not sufficient.
            /// = -8: Error return from trid. eigenvalue calculation; Informational error from LAPACK routine dsteqr.
            /// = -9: Starting vector is zero.
            /// = -10: IPARAM(7) must be 1,2,3,4,5.
            /// = -11: IPARAM(7) = 1 and BMAT = 'G' are incompatable.
            /// = -12: IPARAM(1) must be equal to 0 or 1.
            /// = -13: NEV and WHICH = 'BE' are incompatable.
            /// = -9999: Could not build an Arnoldi factorization. IPARAM(5) returns the size of the current 
            ///          Arnoldi factorization. The user is advised to check that enough workspace 
            ///          and array storage has been allocated.
            /// </param>
            [DllImport("ARPack32.dll")]
            public static extern void dseupd_(int* rvec, byte* all, int* select, double* d, double* z,
                int* ldz, double* sigma, byte* bmat, int* n, byte* which, int* nev, double* tol,
                double* resid, int* ncv, double* v, int* ldv, int* iparam, int* ipntr,
                double* workd, double* workl, int* lworkl, int* info);
        }

        /// <summary>
        /// 64-bitová verze
        /// </summary>
        private class ARPack64 {
            [DllImport("ARPack64.dll")]
            public static extern void dsaupd_(int* ido, byte* bmat, int* n, byte* which,
                int* nev, double* tol, double* resid, int* ncv, double* v, int* ldv,
                int* iparam, int* ipntr, double* workd, double* workl, int* lworkl, int* info);

            [DllImport("ARPack64.dll")]
            public static extern void dseupd_(int* rvec, byte* all, int* select, double* d, double* z,
                int* ldz, double* sigma, byte* bmat, int* n, byte* which, int* nev, double* tol,
                double* resid, int* ncv, double* v, int* ldv, int* iparam, int* ipntr,
                double* workd, double* workl, int* lworkl, int* info);
        }

        private static bool is32bit = true;

        public static void dsaupd(int* ido, byte* bmat, int* n, byte* which,
                int* nev, double* tol, double* resid, int* ncv, double* v, int* ldv,
                int* iparam, int* ipntr, double* workd, double* workl, int* lworkl, int* info) {
            if(is32bit)
                ARPack32.dsaupd_(ido, bmat, n, which, nev, tol, resid, ncv, v, ldv, iparam, ipntr, workd, workl, lworkl, info);
            else
                ARPack64.dsaupd_(ido, bmat, n, which, nev, tol, resid, ncv, v, ldv, iparam, ipntr, workd, workl, lworkl, info);
        }

        public static void dseupd(int* rvec, byte* all, int* select, double* d, double* z,
                int* ldz, double* sigma, byte* bmat, int* n, byte* which, int* nev, double* tol,
                double* resid, int* ncv, double* v, int* ldv, int* iparam, int* ipntr,
                double* workd, double* workl, int* lworkl, int* info) {
            if(is32bit)
                ARPack32.dseupd_(rvec, all, select, d, z, ldz, sigma, bmat, n, which, nev, tol,
                    resid, ncv, v, ldv, iparam, ipntr, workd, workl, lworkl, info);
            else
                ARPack64.dseupd_(rvec, all, select, d, z, ldz, sigma, bmat, n, which, nev, tol,
                    resid, ncv, v, ldv, iparam, ipntr, workd, workl, lworkl, info);
        }

        /// <summary>
        /// Konstruktor, který rozhodne, zda bìžíme na 32 nebo 64 bitech
        /// </summary>
        static ARPackDLLWrapper() {
            if(IntPtr.Size == 4)
                is32bit = true;
            else
                is32bit = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.DLLWrapper {
    unsafe public class ARPackDLL {
        /// <summary>
        /// This is a wrapper routine to the arpach routine dsaupd.
        /// </summary>
        /// <param name="sm">The matrix in ld Yale sparse matrix format</param>
        /// <param name="numev">Number of eigenvalues to calculate. 
        /// The valid range is 1 .. dim-1 (if the matrix is dim times dim).</param>
        /// <param name="ev">If false, no eigenvectors will be calculated 
        /// (the argument eigenvecs does not need to point to a valid array).</param>
        /// <param name="highev">For false, lowest eigenvaluenum eigenvalues are calculated,
        /// and for true, the highest ones.</param>
        public static Vector[] dsaupd(SparseMatrix sm, int numev, bool ev, bool highev, IOutputWriter writer) {
            Vector[] result = null;

            /* Specifies that the right hand side matrix should be the identity matrix; 
             * this makes the problem a standard eigenvalue problem.
             * Setting bmat = "G" would have us solve the problem Av = lBv 
             * (this would involve using some other programs from BLAS, however). */
            byte* bmat = Memory.NewByte(2);
            bmat[0] = (byte)'I';

            // for the maximum dimensions of the problem
            int maxn = 10000;   // enlarge this if it is not big enougth
            int maxnev = maxn;
            int maxncv = maxn;

            /* This tells arpack which eigenvalues we want. The possible options are
             * LM: largest magnitude
             * SM: smallest magnitude
             * LA: largest real component
             * SA: smallest real compoent
             * LI: largest imaginary component
             * SI: smallest imaginary component */
            byte* which = Memory.NewByte(3);
            if(!highev) {
                which[0] = (byte)'S';
                which[1] = (byte)'A';
            }
            else {
                which[0] = (byte)'L';
                which[1] = (byte)'A';
            }

            byte* all = Memory.NewByte(4);
            all[0] = (byte)'A';
            all[1] = (byte)'l';
            all[2] = (byte)'l';

            double tol = 0.0;           // Set the tolerance. A value <= 0 specifies machine recision

            int n = sm.Length;          // Dimension of the matrix : n by n
            int ldv = n;                // Leading dimension of the array v

            // Correct the number of calculated Eigenlevels
            if(numev > sm.Length || numev <= 0)
                numev = sm.Length;

            int nev = numev;            // number of eigenvalue requested

            /* The largest number of basis vectors that will be used in the Implicitly Restarted Arnoldi Process.  
             * Work per major iteration is proportional to N*NCV*NCV. */
            int ncv = 4 * nev;

            if(ncv > n)
                ncv = n;                // maximum value for ncv is n

            int lworkl = ncv * (ncv + 8); //Length of the workl array 

            // Now initialize arrays for input/output and working space for arpack
            double* v = Memory.NewDouble(n * ncv);
            double* d = Memory.NewDouble(2 * ncv);
            double* workl = Memory.NewDouble(lworkl);
            double* workd = Memory.NewDouble(3 * n);
            double* resid = Memory.NewDouble(n);
            double* ax = Memory.NewDouble(n);

            int* select = Memory.NewInt(ncv);
            int* iparam = Memory.NewInt(11);
            int* ipntr = Memory.NewInt(11);

            int info = 0;               // Passes convergence information out of the iteration routine. 

            // Specifications for the algorithm arpack should use
            int ishfts = 1;
            int maxitr = 3 * n;         // maximum number of iterations
            int mode1 = 1;

            iparam[0] = ishfts;
            iparam[2] = maxitr;
            iparam[6] = mode1;

            /* Here we enter the main loop where the calculations are performed.  
             * The communication parameter ido tells us when the desired tolerance is reached, 
             * and at that point we exit and extract the solutions. */
            int ido = 0;                // Must be zero at initial call

            int iterations = 0;
            do {
                ARPackDLLWrapper.dsaupd(&ido, bmat, &n, which, &nev, &tol, resid, &ncv, v, &ldv, iparam, ipntr, workd, workl, &lworkl, &info);
                if(ido == 1 || ido == -1) {
                    /*   %--------------------------------------%
                         | Perform matrix vector multiplication |
                         |              y <--- OP*x             |
                         | The user should supply his/her own   |
                         | matrix vector multiplication routine |
                         | here that takes workd(ipntr(1)) as   |
                         | the input, and return the result to  |
                         | workd(ipntr(2)).                     |
                         %--------------------------------------% */
                    // In FORTRAN the starting indext of an array is 1 but in c/c++ it is 0.
                    // Therefore we have to subtract 1 from ipntr[0] and ipntr[1].
                    sm.VectorProduct(&workd[ipntr[0] - 1], &workd[ipntr[1] - 1]);
                }
                iterations++;
            } while(ido == 1 || ido == -1);

            if(info < 0) { 
                if(writer != null)
                    writer.Write(string.Format("Error in dsaupd {0}...", info));
            }
            else {
                /*  %-------------------------------------------%
                | No fatal errors occurred.                 |
                | Post-Process using DSEUPD.                |
                |                                           |
                | Computed eigenvalues may be extracted.    |  
                |                                           |
                | Eigenvectors may be also computed now if  |
                | desired.  (indicated by rvec = .true.)    | 
                |                                           |
                | The routine DSEUPD now called to do this  |
                | post processing (Other modes may require  |
                | more complicated post processing than     |
                | mode1.)                                   |
                |                                           |
                %-------------------------------------------% */

                // extract also eigenvectors ?
                int rvec = ev ? 0 : 1;

                double sigma = 0.0;
                int ierr = 0;

                // extract now
                ARPackDLLWrapper.dseupd(&rvec, all, select, d, v, &ldv, &sigma, bmat, &n, which, &nev, &tol,
                    resid, &ncv, v, &ldv, iparam, ipntr, workd, workl, &lworkl, &ierr);

                if(ierr != 0) {
                    if(writer != null)
                        writer.Write(string.Format("Error in dseupd {0}...", ierr));
                }
                else {
                    // sometimes the order of the eigenvalues is reversed, I was not able
                    // to figure out when this is the case. The documentation states that
                    // "the eigenvalues are given in algebraic ascending order"
                    if(ev)
                        result = new Vector[numev + 1];
                    else
                        result = new Vector[1];

                    for(int i = 0; i < result.Length; i++)
                        result[i] = new Vector(nev);

                    if(d[0] > d[nev - 1]) {             // revers order
                        for(int i = 0; i < nev; i++)
                            result[0][i] = d[nev - 1 - i];
                    }
                    else {
                        for(int i = 0; i < nev; i++)
                            result[0][i] = d[i];
                    }

                    if(ev) {                     // if also eigenvecs were requested ..
                        if(d[0] > d[nev - 1]) { // reverse order
                            for(int i = 0; i < iparam[4]; i++)
                                for(int j = 0; j < n; j++)
                                    result[i + 1][j] = v[(iparam[4] - 1 - i) * n + j];

                        }
                        else {
                            for(int i = 0; i < iparam[4]; i++)
                                for(int j = 0; j < n; j++)
                                    result[i + 1][j] = v[i * n + j];
                        }
                    }
                }
            }

            if(writer != null)
                writer.Write(string.Format("{0} iterations...", iterations));

            Memory.Delete(bmat);
            Memory.Delete(which);
            Memory.Delete(all);

            Memory.Delete(v);
            Memory.Delete(d);
            Memory.Delete(workl);
            Memory.Delete(workd);
            Memory.Delete(resid);
            Memory.Delete(ax);

            Memory.Delete(select);
            Memory.Delete(iparam);
            Memory.Delete(ipntr);

            return result;
        }    
    }
}
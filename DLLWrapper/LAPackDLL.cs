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

        /// <summary>
        /// Reálnou èást komplexního pole na nespravované haldì pøevede na Vector
        /// </summary>
        /// <param name="v">Ukazatel na pole</param>
        /// <param name="n">Délka pole</param>
        private static Vector ProcessVectorReal(double* v, int n) {
            Vector result = new Vector(n);

            for(int i = 0; i < n; i++)
                result[i] = v[2 * i];

            return result;
        }

        /// <summary>
        /// Imaginární èást komplexního pole na nespravované haldì pøevede na Vector
        /// </summary>
        /// <param name="v">Ukazatel na pole</param>
        /// <param name="n">Délka pole</param>
        private static Vector ProcessVectorImaginary(double* v, int n) {
            Vector result = new Vector(n);

            for(int i = 0; i < n; i++)
                result[i] = v[2 * i + 1];

            return result;
        }

        /// <summary>
        /// Nalezne vlastní èísla symetrické matice
        /// </summary>
        /// <param name="matrix">Vstupní symetrická ètvercová matice</param>
        public static Vector[] dsyev(Matrix matrix, bool ev) {
            return dsyev((MMatrix)matrix, ev);
        }

        /// <summary>
        /// Nalezne vlastní èísla symetrické matice
        /// </summary>
        /// <param name="matrix">Vstupní symetrická ètvercová matice</param>
        public static Vector[] dsyev(MMatrix matrix, bool ev) {
            byte jobz = ev ? (byte)'V' : (byte)'N';
            byte uplo = (byte)'U';

            int n = matrix.Length;

            double* a = matrix.GetItem();
            double* w = Memory.NewDouble(n);

            int lda = n;
            int lwork = -1;
            double* work = null;

            int info = 0;

            Vector[] result = null;
            bool success = false;

            try {
                double lwork1 = 0;

                // Nejprve zjistíme optimální velikost pomocného pole
                LAPackDLLWrapper.dsyev(&jobz, &uplo, &n, a, &lda, w, &lwork1, &lwork, &info);

                lwork = (int)lwork1;
                work = Memory.NewDouble(lwork);

                // Vlastní výpoèet
                LAPackDLLWrapper.dsyev(&jobz, &uplo, &n, a, &lda, w, work, &lwork, &info);
                success = true;
            }
            finally {
                Memory.Delete(work);
            }

            // Zpracování výsledkù
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
                Memory.Delete(w);
            }

            return result;
        }

        /// <summary>
        /// Nalezne vlastní èísla obecné matice
        /// </summary>
        /// <param name="matrix">Vstupní ètvercová matice</param>
        public static Vector[] dgeev(Matrix matrix, bool ev) {
            return dgeev((MMatrix)matrix, ev, ev);
        }

        /// <summary>
        /// Nalezne vlastní èísla matice
        /// </summary>
        /// <param name="matrix">Vstupní ètvercová matice</param>
        public static Vector[] dgeev(MMatrix matrix, bool evl, bool evr) {
            byte jobvl = evl ? (byte)'V' : (byte)'N';
            byte jobvr = evr ? (byte)'V' : (byte)'N';

            int n = matrix.Length;

            double* a = Memory.NewDouble(n * n);
            int lda = n;

            double* wr = Memory.NewDouble(n);
            double* wi = Memory.NewDouble(n);
            
            double* vl = evl ? Memory.NewDouble(n * n) : null;
            int ldvl = n;
            double* vr = evr ? Memory.NewDouble(n * n) : null;
            int ldvr = n;

            int lwork = -1;
            double* work = null;

            int info = 0;

            Vector[] result = null;
            int length = 0;
            bool success = false;

            try {
                double lwork1 = 0;

                // Nejprve zjistíme optimální velikost pomocného pole
                matrix.Fill(a);
                LAPackDLLWrapper.dgeev(&jobvl, &jobvr, &n, a, &lda, wr, wi, vl, &ldvl, vr, &ldvr, &lwork1, &lwork, &info);

                lwork = (int)lwork1;
                work = Memory.NewDouble(lwork);

                if(lwork < 3 * n)
                    throw new Exception("Diagonalization failed. LWORK = " + lwork);

                // Vlastní výpoèet
                matrix.Fill(a);
                LAPackDLLWrapper.dgeev(&jobvl, &jobvr, &n, a, &lda, wr, wi, vl, &ldvl, vr, &ldvr, work, &lwork, &info);
                success = (info == 0);
            }
            finally {
                Memory.Delete(work);
            }

            // Zpracování výsledkù
            try {
                if(success) {
                    length = 2;
                    if(evl) length += n;
                    if(evr) length += n;
                    result = new Vector[length];
                }
                else
                    throw new Exception("Diagonalization failed. Info = " + info);

                result[0] = ProcessVector(wr, n);
                result[1] = ProcessVector(wi, n);

                length = 2;

                if(evl)
                    for(int i = 0; i < n; i++) {
                        result[length] = new Vector(n);
                        for(int j = 0; j < n; j++)
                            result[length][j] = vl[i * n + j];
                        length++;
                    }
                if(evr)
                    for(int i = 0; i < n; i++) {
                        result[length] = new Vector(n);
                        for(int j = 0; j < n; j++)
                            result[length][j] = vr[i * n + j];
                        length++;
                    }
            }
            finally {
                Memory.Delete(wr);
                Memory.Delete(wi);
                Memory.Delete(vl);
                Memory.Delete(vr);
                Memory.Delete(a);
            }

            return result;
        }

        /// <summary>
        /// Nalezne vlastní èísla obecné komplexní matice
        /// </summary>
        /// <param name="matrix">Vstupní ètvercová matice</param>
        public static Vector[] zgeev(CMatrix matrix, bool evl, bool evr) {
            byte jobvl = evl ? (byte)'V' : (byte)'N';
            byte jobvr = evr ? (byte)'V' : (byte)'N';

            int n = matrix.Length;
            double* a = Memory.NewDouble(2 * n * n);
            int lda = n;

            double* w = Memory.NewDouble(2 * n);

            double* vl = evl ? Memory.NewDouble(2 * n * n) : null;
            int ldvl = n;
            double* vr = evr ? Memory.NewDouble(2 * n * n) : null;
            int ldvr = n;

            int lwork = -1;
            double* work = null;
            double* rwork = Memory.NewDouble(2 * n);

            int info = 0;

            Vector[] result = null;
            int length = 0;
            bool success = false;

            try {
                double lwork1 = 0;

                // Nejprve zjistíme optimální velikost pomocného pole
                matrix.Fill(a);
                LAPackDLLWrapper.zgeev(&jobvl, &jobvr, &n, a, &lda, w, vl, &ldvl, vr, &ldvr, &lwork1, &lwork, rwork, &info);

                lwork = (int)lwork1;
                work = Memory.NewDouble(2 * (lwork + 1));

                if(lwork < 2 * n)
                    throw new Exception("Diagonalization failed. LWORK = " + lwork);

                // Vlastní výpoèet
                matrix.Fill(a);
                LAPackDLLWrapper.zgeev(&jobvl, &jobvr, &n, a, &lda, w, vl, &ldvl, vr, &ldvr, work, &lwork, rwork, &info);
                success = (info == 0);
            }
            finally {
                Memory.Delete(work);
            }

            try {
                // Zpracování výsledkù
                if(success) {
                    length = 2;
                    if(evl) length += 2 * n;
                    if(evr) length += 2 * n;
                    result = new Vector[length];
                }
                else
                    throw new Exception("Diagonalization failed. Info = " + info);

                result[0] = ProcessVectorReal(w, n);
                result[1] = ProcessVectorImaginary(w, n);

                length = 2;

                if(evl)
                    for(int i = 0; i < n; i++) {
                        result[length] = new Vector(n);
                        for(int j = 0; j < n; j++) {
                            result[length][j] = vl[2 * (i * n + j)];
                            result[length + 1][j] = vl[2 * (i * n + j) + 1];
                        }
                        length += 2;
                    }
                if(evr)
                    for(int i = 0; i < n; i++) {
                        result[length] = new Vector(n);
                        for(int j = 0; j < n; j++) {
                            result[length][j] = vr[2 * (i * n + j)];
                            result[length + 1][j] = vr[2 * (i * n + j)];
                        }
                        length += 2;
                    }
            }
            finally {
                Memory.Delete(w);
                Memory.Delete(vl);
                Memory.Delete(vr);
                Memory.Delete(rwork);
                Memory.Delete(a);
            }

            return result;
        }

        /// <summary>
        /// Nalezne vlastní pásové matice
        /// </summary>
        /// <param name="band">Matice v pásovém tvaru</param>
        /// <param name="ev">True, pokud chceme poèítat i vlastní vektory</param>
        /// <param name="iMin">Minimální vlastní hodnota</param>
        /// <param name="iMax">Maximální vlastní hodnota</param>
        public static Vector[] dsbevx(SymmetricBandMatrix band, bool ev, int iMin, int iMax) {
            byte jobz = ev ? (byte)'V' : (byte)'N';
            byte range = (byte)'I';
            byte uplo = (byte)'U';

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

            // Zpracování výsledkù
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

        /// <summary>
        /// Singular Value Decomposition
        /// </summary>
        /// <param name="matrix">Vstupní obdélníková matice</param>
        public static void dgesvd(Matrix matrix, Matrix resultU, Vector resultS, Matrix resultVT) {
            byte jobu = (byte)'S';
            byte jobvt = (byte)'S';

            int m = matrix.LengthX;
            int n = matrix.LengthY;

            int mindim = System.Math.Min(m, n);

            double* a = Memory.NewDouble(m * n);
            int k = 0;

            for(int j = 0; j < n; j++)
                for(int i = 0; i < m; i++) 
                    a[k++] = matrix[i, j];

            int lda = m;

            double* s = Memory.NewDouble(mindim);

            double* u = Memory.NewDouble(m * mindim);
            int ldu = m;

            double* vt = Memory.NewDouble(mindim * n);
            int ldvt = mindim;

            int lwork = -1;
            double* work = Memory.NewDouble(1);

            int info = 0;

            bool success = false;
            try {
                // Nejprve zjistíme optimální velikost pomocného pole
                LAPackDLLWrapper.dgesvd(&jobu, &jobvt, &m, &n, a, &lda, s, u, &ldu, vt, &ldvt, work, &lwork, &info);

                lwork = (int)work[0];

                Memory.Delete(work);
                work = Memory.NewDouble(lwork);

                // Vlastní výpoèet
                LAPackDLLWrapper.dgesvd(&jobu, &jobvt, &m, &n, a, &lda, s, u, &ldu, vt, &ldvt, work, &lwork, &info);
                success = true;
            }
            finally {
                Memory.Delete(work);
            }

            // Zpracování výsledkù
            try {
                if(success) {
                    k = 0;
                    for(int j = 0; j < mindim; j++)
                        for(int i = 0; i < m; i++) {
                            if(i < resultU.LengthX && j < resultU.LengthY)
                                resultU[i, j] = u[k];
                            k++;
                        }

                    k = 0;
                    for(int j = 0; j < n; j++)
                        for(int i = 0; i < mindim; i++) {
                            if(i < resultVT.LengthX && j < resultVT.LengthY)
                                resultVT[i, j] = vt[k];
                            k++;
                        }

                    for(int i = 0; i < mindim; i++)
                        if(i < resultS.Length)
                            resultS[i] = s[i];
                }
            }
            finally {
                Memory.Delete(a);
                Memory.Delete(s);
                Memory.Delete(u);
                Memory.Delete(vt);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.DLLWrapper;
using PavelStransky.Systems;

namespace PavelStransky.Systems {
    /// <summary>
    /// T��da, kter� implementuje funkci pro p�len� interval� p�i v�po�tu nul V matice
    /// </summary>
    public class VMatrixBisectionFunction {
        private ClassicalGCM gcm;
        private double gamma, e;
        private int ei;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="gcm">T��da GCM</param>
        /// <param name="gamma">Gamma, pro kter� se bude po��tat</param>
        /// <param name="e">Energie</param>
        /// <param name="ei">Index vlastn� hodnoty (�azen� odspodu, tj. 0 je nejni���)</param>
        public VMatrixBisectionFunction(ClassicalGCM gcm, double gamma, double e, int ei) {
            this.gcm = gcm;
            this.gamma = gamma;
            this.e = e;
            this.ei = ei;
        }

        /// <summary>
        /// Funkce pro p�len� interval�
        /// </summary>
        /// <param name="beta">Parametr</param>
        public double Function(double beta) {
            Matrix m = this.gcm.VMatrixBG(this.e, beta, this.gamma);
            Vector ev = LAPackDLL.dsyev(m, false)[0];
            return ev[this.ei];
        }
    }
}

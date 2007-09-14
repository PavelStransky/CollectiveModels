using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.DLLWrapper;

namespace PavelStransky.GCM {
    /// <summary>
    /// T��da, kter� implementuje funkci pro p�len� interval� p�i v�po�tu nul V matice
    /// </summary>
    public class VMatrixBisectionFunction {
        private GCM gcm;
        private double gamma, e;
        private bool lowerEV;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="gcm">T��da GCM</param>
        /// <param name="gamma">Gamma, pro kter� se bude po��tat</param>
        /// <param name="e">Energie</param>
        /// <param name="lowerEV">True, pokud se pt�me po ni��� vlastn� hodnot�</param>
        public VMatrixBisectionFunction(GCM gcm, double gamma, double e, bool lowerEV) {
            this.gcm = gcm;
            this.gamma = gamma;
            this.e = e;
            this.lowerEV = lowerEV;
        }

        /// <summary>
        /// Funkce pro p�len� interval�
        /// </summary>
        /// <param name="beta">Parametr</param>
        public double Function(double beta) {
            Matrix m = this.gcm.VMatrixBG(this.e, beta, this.gamma);
            Vector ev = LAPackDLL.dsyev(m, false)[0];
            return this.lowerEV ? ev[0] : ev[1];
        }
    }
}

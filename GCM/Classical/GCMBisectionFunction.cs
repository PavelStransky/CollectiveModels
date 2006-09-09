using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.GCM {
    /// <summary>
    /// T��da, kter� implementuje funkci pro p�len� interval�
    /// </summary>
    public class GCMBisectionFunction {
        private GCM gcm;
        private double gamma, e;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="gcm">T��da GCM</param>
        /// <param name="gamma">Gamma, pro kter� se bude po��tat</param>
        /// <param name="e">Energie</param>
        public GCMBisectionFunction(GCM gcm, double gamma, double e) {
            this.gcm = gcm;
            this.gamma = gamma;
            this.e = e;
        }

        /// <summary>
        /// Funkce pro p�len� interval�
        /// </summary>
        /// <param name="beta">Parametr</param>
        public double Function(double beta) {
            return this.gcm.VBG(beta, this.gamma) - this.e;
        }

        public double Gamma { get { return this.gamma; } set { this.gamma = value; } }
        public double E { get { return this.e; } set { this.e = value; } }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Systems {
    /// <summary>
    /// T��da, kter� implementuje funkci pro p�len� interval�
    /// </summary>
    public class GCMJBisectionFunction {
        private ClassicalGCMJ gcm;
        private double e;
        private Vector x;
        private int i;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="gcm">T��da GCM</param>
        /// <param name="e">Energie</param>
        /// <param name="i">Index, podle kter�ho se bude po��tat</param>
        /// <param name="x">Hodnoty ostatn�ch sou�adnic</param>
        public GCMJBisectionFunction(ClassicalGCMJ gcm, double e, int i, Vector x) {
            this.gcm = gcm;
            this.e = e;
            this.i = i;

            this.x = x;
        }

        /// <summary>
        /// Funkce pro p�len� interval�
        /// </summary>
        /// <param name="p">Parametr</param>
        public double Function(double p) {
            this.x[this.i] = p;
            return this.gcm.V(this.x) - this.e;
        }
    }
}

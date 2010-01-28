using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.IBM {
    /// <summary>
    /// T��da, kter� implementuje funkci pro p�len� interval�
    /// </summary>
    public class IBMBisectionFunction {
        private ClassicalIBM ibm;
        private Vector x;
        private int i;
        private double e;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="ibm">T��da IBM</param>
        /// <param name="x">Sou�adnice a hybnosti, pro kter� se bude po��tat</param>
        /// <param name="i">Index sou�adnice (resp. hybnosti), kter� se bude po��tat</param>
        /// <param name="e">Energie</param>
        public IBMBisectionFunction(ClassicalIBM ibm, Vector x, int i, double e) {
            this.ibm = ibm;
            this.x = x;
            this.i = i;
            this.e = e;
        }

        /// <summary>
        /// Funkce pro p�len� interval�
        /// </summary>
        /// <param name="beta">Parametr</param>
        public double Function(double d) {
            this.x[this.i] = d;
            return this.ibm.E(this.x) - this.e;
        }
    }
}
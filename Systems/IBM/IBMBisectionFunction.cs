using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.IBM {
    /// <summary>
    /// Tøída, která implementuje funkci pro pùlení intervalù
    /// </summary>
    public class IBMBisectionFunction {
        private ClassicalIBM ibm;
        private Vector x;
        private int i;
        private double e;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="ibm">Tøída IBM</param>
        /// <param name="x">Souøadnice a hybnosti, pro které se bude poèítat</param>
        /// <param name="i">Index souøadnice (resp. hybnosti), která se bude poèítat</param>
        /// <param name="e">Energie</param>
        public IBMBisectionFunction(ClassicalIBM ibm, Vector x, int i, double e) {
            this.ibm = ibm;
            this.x = x;
            this.i = i;
            this.e = e;
        }

        /// <summary>
        /// Funkce pro pùlení intervalù
        /// </summary>
        /// <param name="beta">Parametr</param>
        public double Function(double d) {
            this.x[this.i] = d;
            return this.ibm.E(this.x) - this.e;
        }
    }
}
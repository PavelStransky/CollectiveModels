using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Empirical Mode Decomposition
    /// </summary>
    /// <remarks>Irving y Emmanuel, marzo 2011</remarks>
    public class EMD {
        PointVector data;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Time series</param>
        public EMD(PointVector data){
            int length = data.Length;

            this.data = data;
            this.Compute();
        }
               
        public void Compute(){
            int length = this.data.Length;
            PointVector resid = this.data.Clone() as PointVector;

            PointVector maxima = resid.Maxima();
            PointVector minima = resid.Minima();

            // Correction
            int nmax = maxima.Length;
            int nmin = minima.Length;

            while(nmax < length - 2 && nmax > 1 && nmin < length - 2 && nmin > 1) {

            }
        }
    }
}
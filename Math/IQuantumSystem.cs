using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Interface pro kvantov� syst�m
    /// </summary>
    public interface IQuantumSystem {
        /// <summary>
        /// Vlastn� hodnoty
        /// </summary>
        Vector GetEigenValues();

        /// <summary>
        /// Vlastn� vektory
        /// </summary>
        Vector GetEigenVector(int i);

        /// <summary>
        /// Po�et vlastn�ch vektor�
        /// </summary>
        int NumEV { get;}

        /// <summary>
        /// Matice s hustotou vlastn� funkce
        /// </summary>
        /// <param name="n">��slo vlastn� funkce</param>
        /// <param name="interval">Oblast hodnot k zhobrazen� (se�azen� ve tvaru minx, maxx, numx, ...)</param>
        Matrix DensityMatrix(int n, params Vector[] interval);
    }
}

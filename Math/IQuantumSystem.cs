using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Interface pro kvantový systém
    /// </summary>
    public interface IQuantumSystem {
        /// <summary>
        /// Vlastní hodnoty
        /// </summary>
        Vector GetEigenValues();

        /// <summary>
        /// Vlastní vektory
        /// </summary>
        Vector GetEigenVector(int i);

        /// <summary>
        /// Poèet vlastních vektorù
        /// </summary>
        int NumEV { get;}

        /// <summary>
        /// Matice s hustotou vlastní funkce
        /// </summary>
        /// <param name="n">Èíslo vlastní funkce</param>
        /// <param name="interval">Oblast hodnot k zhobrazení (seøazená ve tvaru minx, maxx, numx, ...)</param>
        Matrix DensityMatrix(int n, params Vector[] interval);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

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
        /// Hustota vlastní funkce - více bodù
        /// </summary>
        /// <param name="n">Èíslo nebo èísla vlastních funkcí</param>
        /// <param name="interval">Oblast hodnot k zhobrazení (seøazená ve tvaru minx, maxx, numx, ...)</param>
        object ProbabilityDensity(int []n, IOutputWriter writer, params Vector[] interval);

        /// <summary>
        /// Amplituda vlastní funkce - jeden bod
        /// </summary>
        /// <param name="n">Èíslo vlastní funkce</param>
        /// <param name="interval">Souøadnice k zobrazení</param>
        double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] x);
    }
}

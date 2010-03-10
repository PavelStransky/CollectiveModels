using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Interface pro kvantový systém
    /// </summary>
    public interface IQuantumSystem {
        /// <summary>
        /// Systém vlastních hodnot a vektorù
        /// </summary>
        EigenSystem EigenSystem { get;}

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

        /// <summary>
        /// Hamiltonova matice
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        Matrix HamiltonianMatrix(BasisIndex basisIndex, IOutputWriter writer);

        /// <summary>
        /// Hamiltonova matice v pásové formì
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        SymmetricBandMatrix HamiltonianSBMatrix(BasisIndex basisIndex, IOutputWriter writer);

        /// <summary>
        /// Stopa Hamiltonovy matice
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        double HamiltonianMatrixTrace(BasisIndex basisIndex);

        /// <summary>
        /// Peresùv invariant
        /// </summary>
        /// <param name="type">Typ Peresova invariantu</param>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        Vector PeresInvariant(int type);

        /// <summary>
        /// Vytvoøí tøídu s parametry báze
        /// </summary>
        BasisIndex CreateBasisIndex(Vector basisParams);
    }
}

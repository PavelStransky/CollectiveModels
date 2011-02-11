using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Interface pro kvantov� syst�m
    /// </summary>
    public interface IQuantumSystem {
        /// <summary>
        /// Syst�m vlastn�ch hodnot a vektor�
        /// </summary>
        EigenSystem EigenSystem { get;}

        /// <summary>
        /// Hustota vlastn� funkce - v�ce bod�
        /// </summary>
        /// <param name="n">��slo nebo ��sla vlastn�ch funkc�</param>
        /// <param name="interval">Oblast hodnot k zhobrazen� (se�azen� ve tvaru minx, maxx, numx, ...)</param>
        object ProbabilityDensity(int []n, IOutputWriter writer, params Vector[] interval);

        /// <summary>
        /// Amplituda vlastn� funkce - jeden bod
        /// </summary>
        /// <param name="n">��slo vlastn� funkce</param>
        /// <param name="interval">Sou�adnice k zobrazen�</param>
        double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] x);

        /// <summary>
        /// Hamiltonova matice
        /// </summary>
        /// <param name="matrix">Matice k napln�n�</param>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer);

        /// <summary>
        /// Peres�v invariant
        /// </summary>
        /// <param name="type">Typ Peresova invariantu</param>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        Vector PeresInvariant(int type);

        /// <summary>
        /// Vytvo�� t��du s parametry b�ze
        /// </summary>
        BasisIndex CreateBasisIndex(Vector basisParams);
    }
}

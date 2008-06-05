using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

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
    }
}

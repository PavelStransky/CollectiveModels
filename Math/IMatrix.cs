using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Interface pro matici
    /// </summary>
    public interface IMatrix {
        /// <summary>
        /// Indexer
        /// </summary>
        double this[int i, int j] { get;set;}

        /// <summary>
        /// Velikost matice
        /// </summary>
        int Length { get;}

        /// <summary>
        /// Stopa matice
        /// </summary>
        double Trace();

        /// <summary>
        /// Syst�m vlastn�ch hodnot
        /// </summary>
        /// <param name="ev">Chceme vlastn� vektory?</param>
        /// <param name="numEV">Po�et vlastn�ch hodnot (a vektor�)</param>
        Vector[] EigenSystem(bool ev, int numEV, IOutputWriter writer);
    }
}

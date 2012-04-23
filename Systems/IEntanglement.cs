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
    public interface IEntanglement {
        /// <summary>
        /// Parci�ln� stopa
        /// </summary>
        /// <param name="n">Index vlastn� hodnoty</param>
        /// <returns>Matice hustoty podsyst�mu</returns>
        Matrix PartialTrace(int n);
    }
}

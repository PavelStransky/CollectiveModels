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
    public interface IEntanglement {
        /// <summary>
        /// Parciální stopa
        /// </summary>
        /// <param name="n">Index vlastní hodnoty</param>
        /// <returns>Matice hustoty podsystému</returns>
        Matrix PartialTrace(int n);
    }
}

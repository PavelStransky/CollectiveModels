using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Interface pro systém, který vrací minimum a maximum (potenciálu)
    /// </summary>
    public interface IMinMax {
        /// <summary>
        /// Minima
        /// </summary>
        PointVector Minima(double precision);

        /// <summary>
        /// Maxima
        /// </summary>
        PointVector Maxima(double precision);
    }
}

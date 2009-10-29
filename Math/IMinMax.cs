using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Interface pro syst�m, kter� vrac� minimum a maximum (potenci�lu)
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

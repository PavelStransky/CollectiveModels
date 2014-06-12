using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// AM-FM decomposition by the Direct quadrature procedure
    /// </summary>
    /// <remarks>Huang et al., On Instantaneous Frequency (Advances in Adaptive Data Analysis, 2009)</remarks>
    public class AMFMDecomposition {
        private PointVector data;

        public AMFMDecomposition(PointVector data) {
            this.data = data;
        }

        public PointVector[] Decompose(IOutputWriter writer) {
            return null;
        }
    }
}

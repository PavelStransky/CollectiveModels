using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates times of crossing of the plane of Poincaré section for given energy or trajectory given by its initial condition
    /// </summary>
    public class PSTimes: Poincare {
        public override string Help { get { return Messages.HelpPSTimes; } }

        protected override object Compute(PoincareSection ps, Vector plane, int i1, int i2, Vector ic, int numPoints, bool oneOrientation, Guider guider) {
            return ps.SectionTimes(plane, i1, i2, ic, numPoints, oneOrientation, guider);
        }
    }
}
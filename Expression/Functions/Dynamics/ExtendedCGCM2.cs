using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Creates an ExtendedClassicalGCM class with kinetic term proportional to beta^2
    /// </summary>
    public class ExtendedCGCM2 : ExtendedCGCM {
        public override string Help { get { return Messages.ExtendedCGCM2Help; } }

        protected override object Create(double a, double b, double c, double k, double p) {
            return new ExtendedClassicalGCM2(a, b, c, k, p);
        }
    }
}
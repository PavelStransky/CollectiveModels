using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Creates an ExtendedClassicalGCM class with mass proportional to beta^2
    /// </summary>
    public class ExtendedCGCM1 : FunctionDefinition {
        public override string Help { get { return Messages.ExtendedCGCM1Help; } }

        protected virtual object Create(double a, double b, double c, double k, double p) {
            return new ExtendedClassicalGCM1(a, b, c, k, p);
        }
    }
}
using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates an ExtendedClassicalGCM class with mass proportional to beta^2
    /// </summary>
    public class ExtendedCGCM1 : FncExtendedCGCM {
        public override string Help { get { return Messages.HelpExtendedCGCM1; } }

        protected override object Create(double a, double b, double c, double k, double p) {
            return new ExtendedClassicalGCM1(a, b, c, k, p);
        }
    }
}
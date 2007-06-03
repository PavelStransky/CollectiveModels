using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Creates a ClassicalGCMJ class (case with nonzero angular momentum)
    /// </summary>
    public class CGCMJ: CGCM {
        public override string Help { get { return Messages.CGCMJHelp; } }

        protected override object Create(double a, double b, double c, double k) {
            return new ClassicalGCMJ(a, b, c, k);
        }
    }
}
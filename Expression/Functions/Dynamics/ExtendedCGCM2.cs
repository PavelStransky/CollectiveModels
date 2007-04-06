using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvo�� ExtendedClassicalGCM2 t��du
    /// </summary>
    public class ExtendedCGCM2 : ExtendedCGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double p) {
            return new ExtendedClassicalGCM2(a, b, c, k, p);
        }

        private const string help = "Vytvo�� t��du roz���en�ho GCM (s kinetick�m �lenem �m�rn�m beta^2)";
    }
}
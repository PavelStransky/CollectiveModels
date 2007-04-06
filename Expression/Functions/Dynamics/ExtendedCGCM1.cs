using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvo�� ExtendedClassicalGCM1 t��du
    /// </summary>
    public class ExtendedCGCM1 : FunctionDefinition {
        public override string Help { get { return help; } }

        protected virtual object Create(double a, double b, double c, double k, double p) {
            return new ExtendedClassicalGCM1(a, b, c, k, p);
        }

        private const string help = "Vytvo�� t��du roz���en�ho GCM (s hmotou z�vislou na beta^2)";
    }
}
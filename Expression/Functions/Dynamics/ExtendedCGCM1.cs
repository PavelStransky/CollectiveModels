using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí ExtendedClassicalGCM1 tøídu
    /// </summary>
    public class ExtendedCGCM1 : FunctionDefinition {
        public override string Help { get { return help; } }

        protected virtual object Create(double a, double b, double c, double k, double p) {
            return new ExtendedClassicalGCM1(a, b, c, k, p);
        }

        private const string help = "Vytvoøí tøídu rozšíøeného GCM (s hmotou závislou na beta^2)";
    }
}
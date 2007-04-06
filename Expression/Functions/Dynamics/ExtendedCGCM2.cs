using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí ExtendedClassicalGCM2 tøídu
    /// </summary>
    public class ExtendedCGCM2 : ExtendedCGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k, double p) {
            return new ExtendedClassicalGCM2(a, b, c, k, p);
        }

        private const string help = "Vytvoøí tøídu rozšíøeného GCM (s kinetickým èlenem úmìrným beta^2)";
    }
}
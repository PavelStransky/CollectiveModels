using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Vytvoøí Classical GCM tøídu s nenulovým úhlovým momentem
    /// </summary>
    public class CGCMJ: CGCM {
        public override string Help { get { return help; } }

        protected override object Create(double a, double b, double c, double k) {
            return new ClassicalGCMJ(a, b, c, k);
        }

        private const string help = "Vytvoøí GCM tøídu s nenulovým úhlovým momentem";
    }
}
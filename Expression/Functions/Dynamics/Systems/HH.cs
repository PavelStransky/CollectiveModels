using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a HenonHeiles class
    /// </summary>
    public class HH: Fnc {
        public override string Help { get { return Messages.HelpHH; } }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return new HenonHeiles();
        }
    }
}

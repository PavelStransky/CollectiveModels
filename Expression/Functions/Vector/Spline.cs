using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a spline object
    /// </summary>
    public class FnSpline: Fnc {
        public override string Help { get { return Messages.HelpSpline; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return new Spline(arguments[0] as PointVector);
        }

        private const string name = "spline";
    }
}

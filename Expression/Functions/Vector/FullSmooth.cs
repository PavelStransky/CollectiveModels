using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Smooths a vector in such a manner that all components before computed position are used for averaging
    /// </summary>
    public class FullSmooth: Fnc {
        public override string Help { get { return Messages.HelpFullSmooth; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null,
                typeof(Vector), typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is Vector)
                return (item as Vector).FullSmooth();
            else if(item is PointVector)
                return new PointVector((item as PointVector).VectorX, (item as PointVector).VectorY.FullSmooth());

            return null;
        }
    }
}

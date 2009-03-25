using System;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Swaps X and Y coordinates in a Point or PointVector
    /// </summary>
    public class SwapXY : Fnc {
        public override string Help { get { return Messages.HelpSwapXY; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.P1SwapXY, Messages.P1SwapXYDescription, null, typeof(PointD), typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            if(item is PointVector)
                return (item as PointVector).SwapXY();
            else if(item is PointD)
                return new PointD((item as PointD).Y, (item as PointD).X);

            return null;
        }
    }
}

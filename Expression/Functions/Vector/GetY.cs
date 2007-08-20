using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// From point or pointvector separates coordinates y
    /// </summary>
    public class GetY: FunctionDefinition {
        public override string Help { get { return Messages.HelpGetY; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PPoint, Messages.PPointDescription, null,
                typeof(PointD), typeof(PointVector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments[0] is PointVector)
                return (arguments[0] as PointVector).VectorY;
            else if(arguments[0] is PointD)
                return (arguments[0] as PointD).Y;

            return null;
        }
    }
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Rotates the elements of the vector
    /// </summary>
    public class Rotate : Fnc {
        public override string Help { get { return Messages.HelpRotate; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, false, true, false, Messages.PRotationOffset, Messages.PRotationOffsetDescription, 1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int offset = (int)arguments[1];
            return (arguments[0] as Vector).Rotate(offset);
        }
    }
}

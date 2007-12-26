using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Given array transforms into one dimensional array
    /// </summary>
    public class Deflate: Fnc {
        public override string Help { get { return Messages.HelpDeflate; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);

            this.SetParam(0, true, true, false, Messages.PDeflate, Messages.PDeflateDescription, null, typeof(TArray));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as TArray).Deflate();
        }
    }
}

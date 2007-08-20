using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Given array transforms into one dimensional array
    /// </summary>
    public class Deflate: FunctionDefinition {
        public override string Help { get { return Messages.HelpDeflate; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);

            this.SetParam(0, true, true, false, Messages.PDeflate1, Messages.PDeflate1Description, null, typeof(TArray));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            return (arguments[0] as TArray).Deflate();
        }
    }
}

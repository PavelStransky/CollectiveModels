using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Reads Atomic Mass Evaluation from a graph
    /// </summary>
    public class ReadAME : Fnc {
        public override string Help { get { return Messages.HelpReadAME; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PFileName, Messages.PFileNameDescription, null, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string fName = (string)arguments[0];
            return new AME(fName);
        }

    }
}

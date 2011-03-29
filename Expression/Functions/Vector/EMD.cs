using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Empirical mode decomposition
    /// </summary>
    public class FnEMD: Fnc {
        public override string Help { get { return Messages.HelpDFA; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, false, true, false, Messages.PPointVector, Messages.PPointVectorDescription, 10, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            EMD emd = new EMD(arguments[0] as PointVector);
            int s = (int)arguments[1];
            return new TArray(emd.Compute(guider, s));
        
        }

        private const string name = "emd";
    }
}

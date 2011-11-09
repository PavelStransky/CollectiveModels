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
            this.SetNumParams(4);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, false, true, false, Messages.P2EMD, Messages.P2EMDDescription, 10, typeof(int));
            this.SetParam(2, false, true, true, Messages.P3EMD, Messages.P3EMDDescription, 0.0, typeof(double));
            this.SetParam(3, false, true, false, Messages.PFlat, Messages.PFlatDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            bool flat = (bool)arguments[3];
            EMD emd = new EMD(arguments[0] as PointVector, flat);
            int s = (int)arguments[1];
            double delta = (double)arguments[2];

            return new TArray(emd.ComputeAll(guider, s, delta));
        }

        private const string name = "emd";
    }
}
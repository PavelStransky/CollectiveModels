using System    ;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates one Intrinsic Mode Function
    /// </summary>
    public class IMF: Fnc {
        public override string Help { get { return Messages.HelpIMF; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, false, true, false, Messages.PPointVector, Messages.PPointVectorDescription, 10, typeof(int));
            this.SetParam(2, false, true, true, Messages.P3EMD, Messages.P3EMDDescription, 0.0, typeof(double));
            this.SetParam(3, false, true, false, Messages.PFlat, Messages.PFlatDescription, false, typeof(bool));
            this.SetParam(4, false, true, false, Messages.PAllIterations, Messages.PAllIterationsDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            bool flat = (bool)arguments[3];
            EMD emd = new EMD(arguments[0] as PointVector, flat);
            int s = (int)arguments[1];
            double delta = (double)arguments[2];
            bool alli = (bool)arguments[4];
            if(alli)
                return new TArray(emd.ComputeIMF(guider, s, delta, alli));
            else
                return emd.ComputeIMF(guider, s, delta, alli)[0];
        }
    }
}

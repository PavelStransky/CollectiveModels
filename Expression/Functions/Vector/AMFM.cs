using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the AM-FM decomposition of a IMF
    /// </summary>
    public class AMFM : Fnc {
        public override string Help { get { return Messages.HelpAMFM; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector));
            this.SetParam(1, false, true, false, Messages.PFlat, Messages.PFlatDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            bool flat = (bool)arguments[1];
            AMFMDecomposition amfm = new AMFMDecomposition(arguments[0] as PointVector, flat);

            List result = new List();
            result.Add(amfm.GetAM());
            result.Add(amfm.GetFM());
            result.Add(amfm.Phase());
            result.Add(amfm.Frequency());
            return result;
        }
    }
}

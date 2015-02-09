using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the entanglement entropy of a combined system
    /// </summary>
    public class EntanglementEntropy: Fnc {
        public override string Help { get { return Messages.HelpEntanglementEntropy; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PCombinedSystem, Messages.PCombinedSystemDescription, null, typeof(IEntanglement));
            this.SetParam(1, true, true, false, Messages.PEigenValueIndex, Messages.PEigenValueIndexDescription, null, typeof(int));
            this.SetParam(2, false, true, false, Messages.PNormalize, Messages.PNormalizeDescription, true, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IEntanglement system = arguments[0] as IEntanglement;
            int n = (int)arguments[1];
            bool normalize = (bool)arguments[2];

            Vector ev = LAPackDLL.dsyev(system.PartialTrace(n), false)[0];

            double result = 0.0;
            int length = ev.Length;
            for(int i = 0; i < length; i++)
                if(ev[i] > 0)
                    result -= ev[i] * System.Math.Log(ev[i]);
            if(length == 1)
                result = 0.0;
            else if(normalize)
                result /= System.Math.Log(length);

            return result;
        }
    }
}
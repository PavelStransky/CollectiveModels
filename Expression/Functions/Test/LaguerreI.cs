using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Test - Value of the integral of a product of two laguerre polynomial
    /// </summary>
    public class LaguerreI: Fnc {
        public override string Help { get { return string.Empty; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PX, Messages.PXDetail, null, typeof(int));
            this.SetParam(1, true, true, false, Messages.PX, Messages.PXDetail, null, typeof(int));
            this.SetParam(2, true, true, false, Messages.PX, Messages.PXDetail, null, typeof(int));
            this.SetParam(3, true, true, false, Messages.PX, Messages.PXDetail, null, typeof(int));
            this.SetParam(4, true, true, false, Messages.PX, Messages.PXDetail, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int l1 = (int)arguments[0];
            int mu1 = (int)arguments[1];
            int l2 = (int)arguments[2];
            int mu2 = (int)arguments[3];
            int p = (int)arguments[4];

            return LHOQuantumGCM5D1.L2Integral(l1, mu1, l2, mu2, p);
        }
    }
}
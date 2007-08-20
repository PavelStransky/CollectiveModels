using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// General parent for ExtendedClassicalGCM clases
    /// </summary>
    public abstract class ExtendedCGCM: FunctionDefinition {
        protected override void CreateParameters() {
            this.SetNumParams(5);
            this.SetParam(0, false, true, true, Messages.PA, Messages.PADescription, -1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PB, Messages.PBDescription, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PC, Messages.PCDescription, 1.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PK, Messages.PKDescription, 1.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PExtendedGCM, Messages.PExtendedGCMDescription, 1.0, typeof(double));
        }

        protected abstract object Create(double a, double b, double c, double k, double p);

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;

            double a = (double)arguments[0];
            double b = (double)arguments[1];
            double c = (double)arguments[2];
            double k = (double)arguments[3];
            double p = (double)arguments[4];

            return this.Create(a, b, c, k, p);
        }
    }
}
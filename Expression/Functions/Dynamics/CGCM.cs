using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Creates ClassicalGCM class
    /// </summary>
    public class CGCM : FunctionDefinition {
        public override string Help { get { return Messages.HelpCGCM; } }

        protected override void CreateParameters() {
            this.NumParams(4);
            this.SetParam(0, false, true, true, Messages.PA, Messages.PADescription, -1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PB, Messages.PBDescription, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PC, Messages.PCDescription, 1.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PK, Messages.PKDescription, 1.0, typeof(double));
        }

        protected virtual object Create(double a, double b, double c, double k) {
            return new ClassicalGCM(a, b, c, k);
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;

            double a = (double)arguments[0];
            double b = (double)arguments[1];
            double c = (double)arguments[2];
            double k = (double)arguments[3];

            return this.Create(a, b, c, k);
        }
    }
}
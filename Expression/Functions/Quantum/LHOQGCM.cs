using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Parent for all LHOQuantumGCM functions
    /// </summary>
    public abstract class LHOQGCM: FunctionDefinition {
        protected override void CreateParameters() {
            this.NumParams(6);

            this.SetParam(0, false, true, true, Messages.PA, Messages.PADescription, -1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PB, Messages.PBDescription, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PC, Messages.PCDescription, 1.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PK, Messages.PKDescription, 1.0, typeof(double));
            this.SetParam(4, false, true, true, Messages.PA0, Messages.PA0Description, 1.0, typeof(double));
            this.SetParam(5, false, true, true, Messages.PHBar, Messages.PHBarDescription, 0.01, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int count = arguments.Count;

            double a = (double)arguments[0];
            double b = (double)arguments[1];
            double c = (double)arguments[2];
            double k = (double)arguments[3];
            double a0 = (double)arguments[4];
            double hbar = (double)arguments[5];

            return this.Create(a, b, c, k, a0, hbar);
        }

        protected abstract object Create(double a, double b, double c, double k, double a0, double hbar);
   }
}
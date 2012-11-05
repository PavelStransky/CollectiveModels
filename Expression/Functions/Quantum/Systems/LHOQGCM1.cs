using System;
using System.Collections;

using PavelStransky.Systems;
using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// LHOQuantumGCM1 function (Hamiltonian with 1 minimum in Gamma and with a potential given by an expansion of Cos(bx))
    /// </summary>
    public class LHOQGCM1: Fnc {
        public override string Help { get { return Messages.HelpLHOQGCM1; } }

        protected override void CreateParameters() {
            this.SetNumParams(7);

            Vector cosCoef = new Vector(2);
            cosCoef[1] = 1.0;

            this.SetParam(0, false, true, true, Messages.PA, Messages.PADescription, -1.0, typeof(double));
            this.SetParam(1, false, true, true, Messages.PB, Messages.PBDescription, 1.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PC, Messages.PCDescription, 1.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PK, Messages.PKDescription, 1.0, typeof(double));
            this.SetParam(4, false, true, true, Messages.PA0, Messages.PA0Description, 1.0, typeof(double));
            this.SetParam(5, false, true, true, Messages.PHBar, Messages.PHBarDescription, 0.01, typeof(double));
            this.SetParam(6, true, true, false, Messages.PVector, Messages.PVectorDescription, cosCoef, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double a = (double)arguments[0];
            double b = (double)arguments[1];
            double c = (double)arguments[2];
            double k = (double)arguments[3];
            double a0 = (double)arguments[4];
            double hbar = (double)arguments[5];

            Vector cosCoef = (Vector)arguments[6];

            return new LHOQuantumGCM1(a, b, c, k, a0, hbar, cosCoef);
        }
    }
}
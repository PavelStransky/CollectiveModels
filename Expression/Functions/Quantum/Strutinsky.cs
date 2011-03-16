using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns a smooth level density for the Strutinsky corrections
    /// </summary>
    public class Strutinsky: FncMathD {
        public override string Help { get { return Messages.HelpStrutinsky; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetXParam();
            this.SetParam(1, true, true, false, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(Vector));
            this.SetParam(2, true, true, false, Messages.POrder, Messages.POrderDetail, null, typeof(int));
            this.SetParam(3, true, true, true, Messages.PRange, Messages.PRangeDescription, null, typeof(double), typeof(Vector));
        }

        protected override double FnDouble(double x, params object[] p) {
            Vector energy = (Vector)p[0];
            int degree = (int)p[1];
            Vector range = null;
            double ranged = 0.0;

            int length = energy.Length;

            if(p[2] is Vector)
                range = (Vector)p[2];
            else
                ranged = (double)p[2];

            double result = 0;
            for(int i = 0; i < length; i++) {
                double r = range == null ? ranged : range[i];
                double y = (energy[i] - x) / r;
                y *= y;
                result += SpecialFunctions.Laguerre(y, degree, 0.5) * System.Math.Exp(y) / (r * System.Math.Sqrt(System.Math.PI));
            }

            return result;
        }
    }
}

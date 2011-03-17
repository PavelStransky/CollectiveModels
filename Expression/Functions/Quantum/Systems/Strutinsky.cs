using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns a smooth level density for the Strutinsky corrections
    /// </summary>
    public class FnStrutinsky: Fnc {
        public override string Help { get { return Messages.HelpStrutinsky; } }
        public override string Name { get { return name; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.POrder, Messages.POrderDetail, null, typeof(int));
            this.SetParam(2, true, true, true, Messages.PRange, Messages.PRangeDescription, null, typeof(double), typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector energy = (Vector)arguments[0];
            int degree = (int)arguments[1];
            Vector range = null;

            if(arguments[2] is Vector)
                range = (Vector)arguments[2];
            else
            {
                range = new Vector(energy.Length);
                for(int i = 0; i < energy.Length; i++)
                    range[i]=(double)arguments[2];
            }

            return new Strutinsky(energy, range, degree);
        }

        private const string name = "strutinsky";
    }
}

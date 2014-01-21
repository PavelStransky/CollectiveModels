using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For the Creagh-Whelan system returns true if the equipotential surface at the given energy is convex
    /// </summary>
    public class IsConvex : Fnc {
        public override string Help { get { return Messages.HelpIsConvex; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PCW, Messages.PCWDescription, null, typeof(ClassicalCW));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, false, true, false, Messages.P3Equipotential, Messages.P3EquipotentialDescription, 1000, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            ClassicalCW cw = arguments[0] as ClassicalCW;
            double e = (double)arguments[1];
            int n = (int)arguments[2];
            if(cw.IsConvex(e, n)) 
                return 1;
            return 0;
        }
    }
}

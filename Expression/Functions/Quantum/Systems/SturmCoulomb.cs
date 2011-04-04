using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates a quantum SturmCoulomb class
    /// </summary>
    public class QSturmCoulomb: Fnc {
        public override string Help { get { return Messages.HelpQuantumSturmCoulomb; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, true, Messages.PIntensityMagneticField, Messages.PIntensityMagneticFieldDescription, 0.0, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double h = (double)arguments[0];
            return new SturmCoulomb(h);
        }
    }
}
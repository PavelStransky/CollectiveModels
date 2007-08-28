using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// cal_{V} matrix for Toda lattice Hamiltonian
    /// PRL 98, 234301 (2007)
    /// </summary>
    public class VMatrixHH: Fnc {
        public override string Help { get { return Messages.HelpVMatrixHH; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, true, Messages.PX, Messages.PXDetail, null, typeof(double));
            this.SetParam(1, true, true, true, Messages.PY, Messages.PYDescription, null, typeof(double));
            this.SetParam(2, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double x = (double)arguments[0];
            double y = (double)arguments[1];
            double e = (double)arguments[2];

            double v = 0.5 * (x * x + y * y) + x * x * y - y * y * y / 3.0 + 1.5 * x * x * x * x + 0.5 * y * y * y * y;

            double a = 3.0 / (2.0 * (e - v));

            double vx = x + 2.0 * x * y - x * x + 6.0 * x * x * x;
            double vy = y + x * x - y * y + 2.0 * y * y * y;

            double vxx = 1.0 + 2.0 * y - 2.0 * x + 18.0 * x * x;
            double vyy = 1.0 - 2.0 * y + 6.0 * y * y;
            double vxy = 2.0 * x;

            Matrix result = new Matrix(2);
            result[0, 0] = a * vx * vx + vxx;
            result[0, 1] = a * vx * vy + vxy;
            result[1, 0] = result[0, 1];
            result[1, 1] = a * vy * vy + vyy;

            return result;
        }
    }
}

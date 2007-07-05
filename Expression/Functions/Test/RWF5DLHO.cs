using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Test - value of the radial part of basis wave function for 5D LHO
    /// </summary>
    public class RWF5DLHO: MathFnD {
        public override string Help { get { return Messages.HelpRWF5DLHO; } }

        protected override void CreateParameters() {
            this.NumParams(4);

            this.SetParam(0, true, true, true, Messages.PX, Messages.PXDetail, null, typeof(double));
            this.SetParam(1, true, true, false, Messages.POrder, Messages.POrderDetail, null, typeof(int));
            this.SetParam(2, true, true, false, Messages.PAssociatedOrder, Messages.PAssociatedOrderDetail, null, typeof(int));
            this.SetParam(3, true, true, true, Messages.PSCoef, Messages.PSCoefDescription, null, typeof(double));
        }

        protected override double FnDouble(double x, params object[] p) {
            return LHOQuantumGCM5D.Psi(x, (int)p[0], (int)p[1], (double)p[2]);
        }
    }
}
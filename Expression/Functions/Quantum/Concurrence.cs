using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Calculates the concurrence of the LipkinOneOne system
    /// </summary>
    public class Concurrence: Fnc {
        public override string Help { get { return Messages.HelpConcurrence; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PLipkin, Messages.PLipkinDescription, null, typeof(LipkinOneOne));
            this.SetParam(1, true, true, false, Messages.PEigenValueIndex, Messages.PEigenValueIndexDescription, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            LipkinOneOne system = arguments[0] as LipkinOneOne;
            int n = (int)arguments[1];

            Matrix pt = system.PartialTrace(n);

            // Concurrence
            Matrix m = new Matrix(4);
            m[0, 3] = -1; m[3, 0] = -1;
            m[1, 2] = 1; m[2, 1] = 1;
            Matrix ptt = m * pt * m;

            Vector ev = LAPackDLL.dgeev(pt * ptt, false)[0];
            for(int i = 0; i < 4; i++)
                if(ev[i] > 0)
                    ev[i] = System.Math.Sqrt(ev[i]);
                else
                    ev[i] = 0.0;
            ev = ev.Sort() as Vector;

            return System.Math.Max(0, ev[3] - ev[2] - ev[1] - ev[0]);
       }
    }
}
using System;
using System.IO;
using System.Text;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Convert an array of pointvectors into a matrix
    /// </summary>
    public class PV2DToMatrix: Fnc {
        public override string Help { get { return Messages.HelpPV2DToMatrix; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);
            this.SetParam(0, true, true, false, Messages.PPVArray, Messages.PPVArrayDescription, null, typeof(List), typeof(TArray));
            this.SetParam(1, true, true, false, Messages.PIntervalX, Messages.PIntervalXDescription, null, typeof(Vector));
            this.SetParam(2, true, true, false, Messages.PIntervalY, Messages.PIntervalYDescription, null, typeof(Vector));
            this.SetParam(3, true, true, false, Messages.PIntervalZ, Messages.PIntervalZDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            TArray data = arguments[0] is TArray ? (arguments[0] as TArray) : (arguments[0] as List).ToTArray();

            Vector intervalX = arguments[1] as Vector;
            Vector intervalY = arguments[2] as Vector;
            Vector intervalZ = arguments[3] as Vector;

            int numx = (int)intervalX[2];
            int numz = (int)intervalZ[2];

            Matrix m = new Matrix(numx, numz);
            Matrix e = new Matrix(numx, numz);

            int i = 0;
            int numi = data.GetNumElements();

            double koefx = (intervalX[1] - intervalX[0]) / numx;
            double maxx = intervalX[1];

            data.ResetEnumerator();
            foreach(PointVector pv in data) {
                int length = pv.Length;
                int iz = i * numz / numi;

                if(iz >= 0 && iz < numz)
                    for(int j = 0; j < pv.Length; j++) {
                        int ix = (int)((maxx - pv[j].X) / koefx);
                        if(ix >= 0 && ix < numx) {
                            m[ix, iz] += pv[j].Y;
                            e[ix, iz]++;
                        }
                    }
                i++;
            }

            // Vydìlení prvkù
            for(int ix = 0; ix < numx; ix++)
                for(int iz = 0; iz < numz; iz++)
                    if(e[ix, iz] > 0)
                        m[ix, iz] /= e[ix, iz];

            // Doplnìní nul
            for(int ix = 0; ix < numx; ix++)
                for(int iz = 0; iz < numz; iz++)
                    if(e[ix, iz] == 0) {
                        int n = 0;

                        for(int iix = System.Math.Max(0, ix - 1); iix < System.Math.Min(numx, ix + 2); iix++)
                            for(int iiz = System.Math.Max(0, iz - 1); iiz < System.Math.Min(numz, iz + 2); iiz++)
                                if(iix != ix || iiz != iz) {
                                    m[ix, iz] += m[iix, iiz];
                                    n++;
                                }

                        m[ix, iz] /= n;
                    }

            return m;
        }
    }
}

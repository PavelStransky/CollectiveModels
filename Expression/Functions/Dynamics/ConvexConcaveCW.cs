using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For GCM system returns all the convex-concave or concave-convex borders at given points
    /// </summary>
    public class ConvexConcaveCW : Fnc {
        public override string Help { get { return Messages.HelpConvexConcave; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);
            this.SetParam(0, true, true, false, Messages.PCW, Messages.PCWDescription, null, typeof(ClassicalCW));
            this.SetParam(1, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(2, false, true, true, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, 100.0, typeof(double));
            this.SetParam(3, false, true, true, Messages.PPrecisionEnergy, Messages.PPrecisionEnergyDescription, defaultPrecision, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            ClassicalCW cw = arguments[0] as ClassicalCW;
            Vector a = arguments[1] as Vector;
            double maxE = (double)arguments[2];
            double precision = (double)arguments[3];

            a = (Vector)a.Sort();

            int length = a.Length;

            List result = new List();

            ArrayList[] curves = new ArrayList[0];
            for(int i = 0; i < length; i++) {
                ClassicalCW cwx = new ClassicalCW(a[i], cw.B, cw.C, cw.Mu, cw.Power);
                Vector cc = cwx.ConvexConcave(maxE, precision);

                if(cc.Length != curves.Length) {
                    this.StoreCurves(result, curves);
                    curves = new ArrayList[cc.Length];
                    for(int j = 0; j < curves.Length; j++)
                        curves[j] = new ArrayList();
                }
                for(int j = 0; j < cc.Length; j++)
                    curves[j].Add(new PointD(a[i], cc[j]));
            }
            this.StoreCurves(result, curves);

            return result.ToTArray();
        }

        private void StoreCurves(List result, ArrayList[] curves) {
            for(int j = 0; j < curves.Length; j++) {
                ArrayList curve = curves[j];
                PointVector pv = new PointVector(curve.Count);
                int k = 0;
                foreach(PointD p in curve)
                    pv[k++] = p;
                result.Add(pv);
            }
        }

        private static double defaultPrecision = 1E-3; 
    }
}

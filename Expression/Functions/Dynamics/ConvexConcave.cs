using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For GCM system returns the negative energy for which the border changes from
    /// convex to concave shape
    /// </summary>
    public class ConvexConcave: Fnc {
        public override string Help { get { return Messages.HelpConvexConcave; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(GCM), typeof(ClassicalCW));
            this.SetParam(1, false, true, true, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, 100.0, typeof(double));
            this.SetParam(2, false, true, true, Messages.PPrecisionEnergy, Messages.PPrecisionEnergyDescription, defaultPrecision, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            double precision = (double)arguments[2];
            
            if(arguments[0] is GCM) {
                GCM gcm = arguments[0] as GCM;

                Vector extrem = gcm.ExtremalBeta(0);
                int el = extrem.Length;

                Vector extremV = new Vector(el);
                for(int i = 0; i < el; i++)
                    extremV[i] = gcm.V(extrem[i], 0);

                extremV = (Vector)extremV.Sort();

                double minE = extremV[0] + precision;
                double saddleE = extremV[1] - precision;

                if(this.IsConvex(gcm.EquipotentialContours(saddleE, 0, 0)[0]))
                    return 0;

                // Hledani minima metodou puleni intervalu
                while(System.Math.Abs(saddleE - minE) > precision) {
                    double e = (saddleE + minE) * 0.5;
                    if(this.IsConvex(gcm.EquipotentialContours(e, 0, 0)[0]))
                        minE = e;
                    else
                        saddleE = e;
                }

                return (minE + saddleE) * 0.5;
            }
            else if(arguments[0] is ClassicalCW) {
                ClassicalCW cw = arguments[0] as ClassicalCW;
                double maxE = (double)arguments[1];

                return cw.ConvexConcave(maxE, precision);
            }

            return null;
        }

        /// <summary>
        /// Vraci true, pokud je uvedena hranice konvexni
        /// </summary>
        private bool IsConvex(PointVector v) {
            Vector x = v.VectorX;
            int length = x.Length;
            int maxIndex = x.MaxIndex();

            if(System.Math.Abs(length - maxIndex * 2) < 5)
                return true;
            else
                return false;
        }

        private static double defaultPrecision = 1E-3;
    }
}

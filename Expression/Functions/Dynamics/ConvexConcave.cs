using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For GCM system returns the negative energy for which the border changes from
    /// convex to concave shape
    /// </summary>
    public class ConvexConcave: Fnc {
        public override string Help { get { return Messages.HelpConvexConcave; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);
            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(PavelStransky.GCM.GCM));
            this.SetParam(1, false, true, false, Messages.P2ConvexConcave, Messages.P2ConvexConcaveDescription, true, typeof(bool));
            this.SetParam(2, false, true, true, Messages.PPrecisionEnergy, Messages.PPrecisionEnergyDescription, defaultPrecision, typeof(double));
        }

        /// <summary>
        /// NEHOTOVE - chybi zavislost na druhem parametru !!!!!!!!!!!
        /// </summary>
        /// <param name="guider"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PavelStransky.GCM.GCM gcm = arguments[0] as PavelStransky.GCM.GCM;
            bool island = (bool)arguments[1];
            double precision = (double)arguments[2];

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

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Finds all intersection points of two pointvectors
    /// </summary>
    public class Intersection: Fnc {
        public override string Help { get { return Messages.HelpIntersection; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector), typeof(TArray));
            this.SetParam(1, true, true, false, Messages.PPointVector, Messages.PPointVectorDescription, null, typeof(PointVector), typeof(TArray));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            TArray a1 = null;
            if(arguments[0] is TArray)
                a1 = arguments[0] as TArray;
            else
                a1 = new TArray(arguments[0]);

            TArray a2 = null;
            if(arguments[1] is TArray)
                a2 = arguments[1] as TArray;
            else 
                a2 = new TArray(arguments[1]);

            int length1 = a1.Length;
            int length2 = a2.Length;

            ArrayList r = new ArrayList();
            int length = 0;

            for(int i = 0; i < length1; i++) {
                if(guider != null && length1 > 1 && length2 > 1)
                    guider.Write(".");
                for(int j = 0; j < length2; j++) {
                    PointVector pv = (a1[i] as PointVector).Intersection(a2[j] as PointVector);
                    length += pv.Length;
                    if(pv.Length > 0)
                        r.Add(pv);
                }
            }

            // Join together
            PointVector result = new PointVector(length);
            int k = 0;
            foreach(PointVector pv in r) {
                for(int j = 0; j < pv.Length; j++)
                    result[k++] = pv[j];
            }

            return result;
        }
    }
}
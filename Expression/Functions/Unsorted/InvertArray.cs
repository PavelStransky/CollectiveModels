using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// From an array f[i][j] makes array f[j][i]
    /// </summary>
    public class InvertArray : Fnc {
        public override string Help { get { return Messages.HelpInvertArray; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, true, Messages.PArray, Messages.PArrayDescription, null, typeof(TArray));
            this.SetParam(1, false, true, true, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            TArray a = arguments[0] as TArray;
            
            if(a.Rank > 1)
                throw new FncException(this, Messages.EMInvalidRank, string.Format(Messages.EMInvalidRankDetail, 1, a.Rank));

            List result = new List();

            if(a.GetItemType() == typeof(Vector)) {
                int lengthX = a.Length;
                int lengthY = 0;
                for(int i = 0; i < lengthX; i++)
                    lengthY = System.Math.Max(lengthY, (a[i] as Vector).Length);

                if(arguments.Count > 1) {
                    Vector x = arguments[1] as Vector;

                    if(x.Length != lengthX)
                        throw new FncException(this, Messages.EMNotEqualLength, string.Format(Messages.EMNotEqualLengthDetail, lengthX, x.Length));

                    for(int i = 0; i < lengthY; i++) {
                        PointVector pv = new PointVector(lengthX);
                        int k = 0;
                        for(int j = 0; j < lengthX; j++) {
                            Vector v = a[j] as Vector;
                            if(i < v.Length)
                                pv[k++] = new PointD(x[j], v[i]);
                        }
                        pv.Length = k;
                        result.Add(pv);
                    }                    
                }
                else {
                    for(int i = 0; i < lengthY; i++) {
                        Vector v = new Vector(lengthX);
                        for(int j = 0; j < lengthX; j++) 
                            v[j] = (a[j] as Vector)[i];
                        result.Add(v);
                    }                    
                }
            }

            else if(a.GetItemType() == typeof(PointVector)) {
                int lengthX = a.Length;
                int lengthY = (a[0] as PointVector).Length;

                if(arguments.Count > 1) {
                    Vector x = arguments[1] as Vector;

                    if(x.Length != lengthX)
                        throw new FncException(this, Messages.EMNotEqualLength, string.Format(Messages.EMNotEqualLengthDetail, lengthX, x.Length));

                    for(int i = 0; i < lengthY; i++) {
                        PointVector pv = new PointVector(lengthX);
                        for(int j = 0; j < lengthX; j++) 
                            pv[j] = new PointD(x[j], (a[j] as PointVector)[i].Y);
                        result.Add(pv);
                    }                    
                }
                else {
                    for(int i = 0; i < lengthY; i++) {
                        PointVector pv = new PointVector(lengthX);
                        for(int j = 0; j < lengthX; j++) 
                            pv[j] = (a[j] as PointVector)[i];
                        result.Add(pv);
                    }                    
                }
            }
            
            else
                throw new FncException(this, string.Format(Messages.EMBadParamType, 0, this.Name));

            return result.ToTArray();
        }
    }
}

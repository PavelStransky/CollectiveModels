using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Reduces the size of a matrix or vector by a given factor
    /// </summary>
    public class ReduceSize : Fnc {
        public override string Help { get { return Messages.HelpReduceSize; } }

        protected override void CreateParameters() {
            this.SetNumParams(2, true);
            this.SetParam(0, true, true, false, Messages.PObject, Messages.PObjectDescription, null, typeof(Matrix), typeof(Vector), typeof(PointVector));
            this.SetParam(1, true, true, false, Messages.PFactor, Messages.PFactorDescription, 1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments[0] is Matrix) {
                Matrix m = arguments[0] as Matrix;
                int lengthX = m.LengthX;
                int lengthY = m.LengthY;
                int factorX = (int)(arguments[1]);
                int factorY = (int)(arguments[2]);
                if(factorY <= 0)
                    factorY = factorX;

                int lx = lengthX / factorX;
                int ly = lengthY / factorY;

                Matrix result = new Matrix(lx, ly);

                for(int i = 0; i < lx; i++)
                    for(int j = 0; j < ly; j++) {
                        int num = 0;
                        int ix = factorX * i;
                        int iy = factorY * j;
                        for(int k = 0; k < factorX; k++)
                            for(int l = 0; l < factorY; l++)
                                if(ix + k < lengthX && iy + l < lengthY) {
                                    result[i, j] += m[ix + k, iy + l];
                                    num++;
                                }
                        if(num > 0)
                            result[i, j] /= num;
                    }

                return result;
            }

            else if(arguments[0] is Vector) {
                Vector v = arguments[0] as Vector;
                int length = v.Length;
                int factor = (int)(arguments[1]);

                int l = length / factor;

                Vector result = new Vector(l);
                for(int i = 0; i < l; i++) {
                    int num = 0;
                    int ix = factor * i;
                    for(int k = 0; k < factor; k++)
                        if(ix + k < length) {
                            result[i] += v[ix + k];
                            num++;
                        }
                    if(num > 0)
                        result[i] /= num;
                }

                return result;
            }
            
            else if(arguments[0] is PointVector) {
                PointVector pv = arguments[0] as PointVector;
                int length = pv.Length;
                int factor = (int)(arguments[1]);

                int l = length / factor;

                PointVector result = new PointVector(l);
                for(int i = 0; i < l; i++) {
                    int num = 0;
                    int ix = factor * i;
                    for(int k = 0; k < factor; k++)
                        if(ix + k < length) {
                            result[i] += new PointD(pv[ix + k].X, pv[ix + k].Y);
                            num++;
                        }
                    if(num > 0)
                        result[i] /= new PointD(num, num);
                }

                return result;
            }

            return null;
        }
    }
}
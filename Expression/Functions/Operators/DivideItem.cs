using System.Collections;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Operator divide, items of vectors and matrices is divided among one another
    /// </summary>
    public class DivideItem: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpDivideItem; } }
        public override OperatorPriority Priority { get { return OperatorPriority.ProductPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, true, true, false, Messages.PDividend, Messages.PDividendDescription, null,
                typeof(int), typeof(double), typeof(Vector), typeof(Matrix), typeof(PointD),
                typeof(LongNumber), typeof(LongFraction));

            this.AddCompatibility(typeof(int), typeof(double));
            this.AddCompatibility(typeof(int), typeof(Vector));
            this.AddCompatibility(typeof(int), typeof(Matrix));
            this.AddCompatibility(typeof(int), typeof(LongNumber));
            this.AddCompatibility(typeof(int), typeof(LongFraction));
            this.AddCompatibility(typeof(double), typeof(Vector));
            this.AddCompatibility(typeof(double), typeof(Matrix));
            this.AddCompatibility(typeof(LongNumber), typeof(LongFraction));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int[] lengths = this.GetTypesLength(arguments, true);

            // Výpoèet 
            if(lengths[4] >= 0) {       // Vektor
                int lengthv = lengths[4];
                Vector resultv = new Vector(lengthv);

                for(int i = 0; i < lengthv; i++) {
                    resultv[i] = 1.0;
                    int k = 0;
                    foreach(object o in arguments) {
                        if(k == 0) {
                            if(o is Vector)
                                resultv[i] *= (o as Vector)[i];
                            else if(o is double)
                                resultv[i] *= (double)o;
                            else if(o is int)
                                resultv[i] *= (int)o;
                        }
                        else {
                            if(o is Vector)
                                resultv[i] /= (o as Vector)[i];
                            else if(o is double)
                                resultv[i] /= (double)o;
                            else if(o is int)
                                resultv[i] /= (int)o;
                        }
                        k++;
                    }
                }
                return resultv;
            }

            else if(lengths[6] >= 0) {    // Matice
                int lengthmx = lengths[6];
                int lengthmy = lengths[7];
                Matrix resultm = new Matrix(lengthmx, lengthmy);

                for(int i = 0; i < lengthmx; i++)
                    for(int j = 0; j < lengthmy; j++) {
                        resultm[i, j] = 1.0;
                        int k = 0;
                        foreach(object o in arguments) {
                            if(k == 0) {
                                if(o is Matrix)
                                    resultm[i, j] *= (o as Matrix)[i, j];
                                else if(o is double)
                                    resultm[i, j] *= (double)o;
                                else if(o is int)
                                    resultm[i, j] *= (int)o;
                            }
                            else {
                                if(o is Matrix)
                                    resultm[i, j] /= (o as Matrix)[i, j];
                                else if(o is double)
                                    resultm[i, j] /= (double)o;
                                else if(o is int)
                                    resultm[i, j] /= (int)o;
                            }
                            k++;
                        }
                    }
                return resultm;
            }

            else if(lengths[8] >= 0) {  // PointD
                double x = 1.0;
                double y = 1.0;

                int k = 0;

                foreach(PointD p in arguments) {
                    if(k == 0 ) {
                        x *= p.X;
                        y *= p.Y;
                    }
                    else {
                        x /= p.X;
                        y /= p.Y;
                    }
                    k++;
                }

                return new PointD(x, y);
            }

            else if(lengths[12] >= 0 || lengths[10] >= 0) {     // LongFraction or LongNumber
                LongFraction result = new LongFraction(1);

                if(arguments.Count == 1) {
                    if(arguments[0] is LongNumber)
                        return result / (LongNumber)arguments[0];
                    else 
                        return result / (LongFraction)arguments[0];
                }

                foreach(object o in arguments) {
                    if(result == null) {
                        if(o is int)
                            result *= (int)o;
                        else if(o is LongNumber)
                            result *= (LongNumber)o;
                        else if(o is LongFraction)
                            result *= (LongFraction)o;
                    }
                    else {
                        if(o is int)
                            result /= (int)o;
                        else if(o is LongNumber)
                            result /= (LongNumber)o;
                        else if(o is LongFraction)
                            result /= (LongFraction)o;
                    }
                }

                return result;
            }
           
            else if(lengths[0] >= 0 && lengths[2] < 0) {    // int
                int resulti = 1;

                int k = 0;

                foreach(int i in arguments) {
                    if(k == 0)
                        resulti *= i;
                    else
                        resulti /= i;
                    k++;
                }

                return resulti;
            }

            else {                      // double a int - výsledek double
                double resultd = 1.0;

                int k = 0;

                foreach(object o in arguments) {
                    if(k == 0) {
                        if(o is int)
                            resultd *= (int)o;
                        else
                            resultd *= (double)o;
                    }
                    else {
                        if(o is int)
                            resultd /= (int)o;
                        else
                            resultd /= (double)o;
                    }
                    k++;
                }

                return resultd;
            }
        }

        private const string name = "//";
    }
}

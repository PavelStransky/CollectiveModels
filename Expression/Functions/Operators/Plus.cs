using System.Collections;
using System.Text;
using System;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
	/// <summary>
	/// Operator plus
	/// </summary>
	public class Plus: Operator {
        public override string Name { get { return name; } }
        public override string Help { get { return Messages.HelpPlus; } }
        public override OperatorPriority Priority { get { return OperatorPriority.SumPriority; } }

        protected override void CreateParameters() {
            this.SetNumParams(1, true);
            this.SetParam(0, true, true, false, Messages.PAddend, Messages.PAddendDescription, null,
                typeof(int), typeof(double), typeof(Vector), typeof(Matrix), typeof(PointD), typeof(string), 
                typeof(LongNumber), typeof(LongFraction), typeof(PointVector));

            this.AddCompatibility(typeof(int), typeof(double));
            this.AddCompatibility(typeof(int), typeof(Vector));
            this.AddCompatibility(typeof(int), typeof(Matrix));
            this.AddCompatibility(typeof(int), typeof(LongNumber));
            this.AddCompatibility(typeof(int), typeof(LongFraction));
            this.AddCompatibility(typeof(double), typeof(Vector));
            this.AddCompatibility(typeof(double), typeof(Matrix));
            this.AddCompatibility(typeof(PointD), typeof(PointVector));
            this.AddCompatibility(typeof(Vector), typeof(PointVector));
            this.AddCompatibility(typeof(LongNumber), typeof(LongFraction));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            int[] lengths = this.GetTypesLength(arguments, true);

            // V�po�et 
            if(lengths[16] >= 0) {          // Pointvector
                int lengthpv = lengths[16];
                PointVector resultpv = new PointVector(lengthpv);

                for(int i = 0; i < lengthpv; i++)
                    foreach(object o in arguments)
                        if(o is Vector)
                            resultpv[i].Y += (o as Vector)[i];
                        else if(o is double)
                            resultpv[i].Y += (double)o;
                        else if(o is int)
                            resultpv[i].Y += (int)o;
                        else if(o is PointD)
                            resultpv[i] += (PointD)o;
                        else if(o is PointVector)
                            resultpv[i] += (o as PointVector)[i];

                return resultpv;
            }

            else if(lengths[4] >= 0) {       // Vektor
                int lengthv = lengths[4];
                Vector resultv = new Vector(lengthv);

                for(int i = 0; i < lengthv; i++)
                    foreach(object o in arguments)
                        if(o is Vector)
                            resultv[i] += (o as Vector)[i];
                        else if(o is double)
                            resultv[i] += (double)o;
                        else if(o is int)
                            resultv[i] += (int)o;

                return resultv;
            }

            else if(lengths[6] >= 0) {    // Matice
                int lengthmx = lengths[6];
                int lengthmy = lengths[7];
                Matrix resultm = new Matrix(lengthmx, lengthmy);

                for(int i = 0; i < lengthmx; i++)
                    for(int j = 0; j < lengthmy; j++)
                        foreach(object o in arguments)
                            if(o is Matrix)
                                resultm[i, j] += (o as Matrix)[i, j];
                            else if(o is double)
                                resultm[i, j] += (double)o;
                            else if(o is int)
                                resultm[i, j] += (int)o;

                return resultm;
            }

            else if(lengths[8] >= 0) {  // PointD
                double x = 0.0;
                double y = 0.0;

                foreach(PointD p in arguments) {
                    x += p.X;
                    y += p.Y;
                }

                return new PointD(x, y);
            }

            else if(lengths[10] >= 0) { // string
                int ls = lengths[10];
                StringBuilder results = new StringBuilder(ls);

                for(int i = 0; i < ls; i++) {
                    char c = (char)0;
                    foreach(string s in arguments)
                        c += s[i];
                    results.Append(c);
                }

                return results.ToString();
            }

            else if(lengths[14] >= 0) { // LongFraction
                LongFraction result = new LongFraction(0);

                foreach(object o in arguments) {
                    if(o is int)
                        result += (int)o;
                    else if(o is LongNumber)
                        result += (LongNumber)o;
                    else if(o is LongFraction)
                        result += (LongFraction)o;
                }

                return result;
            }

            else if(lengths[12] >= 0) { // LongNumber
                LongNumber result = new LongNumber(0);

                foreach(object o in arguments) {
                    if(o is int)
                        result += (int)o;
                    else if(o is LongNumber)
                        result += (LongNumber)o;
                }

                return result;
            }

            else if(lengths[0] >= 0 && lengths[2] < 0) {    // int
                int resulti = 0;

                foreach(int i in arguments)
                    resulti += i;

                return resulti;
            }

            else {                      // double a int - v�sledek double
                double resultd = 0;

                foreach(object o in arguments)
                    if(o is int)
                        resultd += (int)o;
                    else
                        resultd += (double)o;

                return resultd;
            }
        }
		
		private const string name = "+";
	}
}

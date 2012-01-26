using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Histogram according to Benford's law
    /// </summary>
    public class Benford: Fnc {
        public override string Help { get { return Messages.HelpBenford; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, false, true, false, Messages.PBenfordFirstDigit, Messages.PBenfordFirstDigitDescription, 0, typeof(int));
            this.SetParam(2, false, true, false, Messages.PBenfordNumDigits, Messages.PBenfordNumDigitDescription, 1, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector data = arguments[0] as Vector;
            int firstDigit = (int)arguments[1];
            int numDigits = (int)arguments[2];

            int rlength = 1;
            int rmin = 0;

            for(int i = 0; i < numDigits; i++) {
                rmin += rlength;
                rlength *= 10;
            }

            Vector result = new Vector(rlength);

            int length = data.Length;
            for(int i = 0; i < length; i++) {
                double x = System.Math.Abs(data[i]);
                if(x == 0.0)
                    continue;

                double y = System.Math.Log10(x);
                y = y - System.Math.Floor(y) + (numDigits + firstDigit - 1);
                y = System.Math.Pow(10, y);

                string s = System.Math.Floor(y).ToString();
                int r = int.Parse(s.Substring(firstDigit));

                result[r]++;
            }

            return result;
        }
    }
}

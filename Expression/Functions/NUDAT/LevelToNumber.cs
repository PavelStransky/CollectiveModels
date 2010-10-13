using System;
using System.IO;
using System.Net;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Transforms the string value of the level angular momentum 1/2- and parity into the numerical form -0.5 and vice versa
    /// </summary>
    public class LevelToNumber: Fnc {
        public override string Help { get { return Messages.HelpLevelToNumber; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PNuclearLevel, Messages.PNuclearLevelDescription, null, typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            string level = (string)arguments[0];

            int sign = (level.IndexOf("-") >= 0) ? -1 : 1;
            level = level.Replace("-", string.Empty).Replace("+", string.Empty);
            level = level.Replace("(", string.Empty).Replace(")", string.Empty).Replace("[", string.Empty).Replace("]", string.Empty);

            int commaIndex = level.IndexOf("/");

            double result = 0.0;

            if(level != string.Empty) {
                try {
                    if(commaIndex < 0)
                        result = double.Parse(level);
                    else {
                        result = double.Parse(level.Substring(0, commaIndex)) / double.Parse(level.Substring(commaIndex + 1));
                    }
                }
                catch(Exception) { }
            }

            return sign * result;
        }
    }
}

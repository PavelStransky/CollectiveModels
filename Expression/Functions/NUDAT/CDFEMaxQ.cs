using System;
using System.IO;
using System.Net;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Find the maximum value of the quadrupole deformation
    /// </summary>
    public class CDFEMaxQ: Fnc {
        public override string Help { get { return Messages.HelpCDFEMaxQ; } }

        protected override void CreateParameters() {
            this.SetNumParams(1);
            this.SetParam(0, true, true, false, Messages.PCDFE, Messages.PCDFEDescription, null, typeof(List));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            List data = arguments[0] as List;

            bool first = true;
            double result = 0.0;
            int resultSign = 0;

            List Qmom = data[1] as List;
            foreach(object o in Qmom) {
                if(!(o is List))
                    continue;

                List l = o as List;

                if(first) {
                    resultSign = (int)l[0];
                    result = System.Math.Abs((double)l[1]);
                    first = false;
                }
                else {
                    if(resultSign != (int)l[0])
                        resultSign = 0;
                    else
                        resultSign = (int)l[0];
                    result = System.Math.Max(System.Math.Abs((double)l[1]), result);
                }
            }

            List r = new List();
            r.Add(resultSign);
            r.Add(result);
            return r;
        }
    }
}

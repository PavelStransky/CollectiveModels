using System;
using System.IO;
using System.Net;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Find the maximum value of the deformation parameter
    /// </summary>
    public class CDFEMaxBeta: Fnc {
        public override string Help { get { return Messages.HelpCDFEMaxBeta; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PCDFE, Messages.PCDFEDescription, null, typeof(List));
            this.SetParam(1, false, true, false, Messages.PCDFEBeta, Messages.PCDFEBetaDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            List data = arguments[0] as List;
            int b = (int)arguments[1];

            bool first = true;
            double result = 0.0;
            int resultSign = 0;

            List Qmom = data[b + 1] as List;
            int resultSignIndex = (b == 0) ? 3 : 0;
            int resultIndex = (b == 0) ? 4 : 1;

            foreach(object o in Qmom) {
                if(!(o is List))
                    continue;

                List l = o as List;

                if(first) {
                    resultSign = (int)l[resultSignIndex];
                    result = System.Math.Abs((double)l[resultIndex]);
                    first = false;
                }
                else {
                    if(resultSign != (int)l[resultSignIndex])
                        resultSign = 0;
                    else
                        resultSign = (int)l[resultSignIndex];
                    result = System.Math.Max(System.Math.Abs((double)l[resultIndex]), result);
                }
            }

            List r = new List();
            r.Add(resultSign);
            r.Add(result);
            return r;
        }
    }
}

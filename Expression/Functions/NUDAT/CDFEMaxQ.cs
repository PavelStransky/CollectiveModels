using System;
using System.IO;
using System.Net;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Find the maximum and minimum value of the quadrupole deformation with its errors
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
            double resultMax = 0.0;
            double resultErrorMax = 0.0;
            int resultSign = 0;

            double resultMin = 0.0;
            double resultErrorMin = 0.0;

            List Qmom = data[1] as List;
            foreach(object o in Qmom) {
                if(!(o is List))
                    continue;

                List l = o as List;

                if(first) {
                    resultSign = (int)l[0];
                    resultMax = System.Math.Abs((double)l[1]);
                    resultErrorMax = System.Math.Abs((double)l[2]);
                    resultMin = resultMax;
                    resultErrorMin = resultErrorMax;
                    first = false;
                }
                else {
                    if(resultSign != (int)l[0])
                        resultSign = 0;
                    else
                        resultSign = (int)l[0];
                    if(resultMax < System.Math.Abs((double)l[1]))
                    {
                        resultMax = System.Math.Abs((double)l[1]);
                        resultErrorMax = System.Math.Abs((double)l[2]);
                    }
                    if(resultMin > System.Math.Abs((double)l[1])) {
                        resultMin = System.Math.Abs((double)l[1]);
                        resultErrorMin = System.Math.Abs((double)l[2]);
                    }
                }
            }

            List r = new List();
            r.Add(resultSign);
            r.Add(resultMax);
            r.Add(resultErrorMax);
            r.Add(resultMin);
            r.Add(resultErrorMin);
            return r;
        }
    }
}

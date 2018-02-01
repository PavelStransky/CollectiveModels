using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the survival probability and LDS of a quench
    /// </summary>
    public class QuenchEntropy : Fnc {
        public override string Help { get { return Messages.HelpQuench; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, false, Messages.PInitialSystem, Messages.PInitialSystemDescription, null, typeof(QuantumDicke));
            this.SetParam(1, true, true, false, Messages.PFinalSystem, Messages.PFinalSystemDescription, null, typeof(QuantumDicke));
            this.SetParam(2, true, true, false, Messages.PInitialState, Messages.PInitialStateDescription, 0, typeof(int), typeof(Vector));
            this.SetParam(3, true, true, false, Messages.PTime, Messages.PTimeDescription, null, typeof(Vector));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            QuantumDicke di = arguments[0] as QuantumDicke;
            QuantumDicke df = arguments[1] as QuantumDicke;

            int length = di.EigenSystem.GetEigenVector(0).Length;
            int numEV = di.EigenSystem.NumEV;

            Vector initial = null;
            if(arguments[2] is Vector) {
                initial = arguments[2] as Vector;
                initial /= initial.EuklideanNorm();
            }
            else {
                initial = new Vector(length);
                initial[(int)arguments[2]] = 1;
            }

            Vector time = arguments[3] as Vector;
            int tLength = time.Length;

            Vector b = new Vector(length);
            for(int i = 0; i < numEV; i++) {
                Vector ci = di.EigenSystem.GetEigenVector(i);
                for(int j = 0; j < length; j++)
                    b[j] += ci[j] * initial[i];
            }
            if(guider != null)
                guider.Write("B");

            Vector f = new Vector(numEV);
            for(int i = 0; i < numEV; i++) {
                Vector cf = df.EigenSystem.GetEigenVector(i);
                for(int j = 0; j < length; j++)
                    f[i] += cf[j] * b[j];
            }
            if(guider != null)
                guider.Write("F");

            Vector es = df.EigenSystem.GetEigenValues() as Vector;
            es /= (df.Omega0 * df.J);

            Matrix dmre = new Matrix(length);
            Matrix dmim = new Matrix(length);

            double t = 1;
            for(int i = 0; i < length; i++) {
                if(guider != null)
                    guider.Write(".");
                for(int j = 0; j < length; j++) {
                    double dre = 0.0;
                    double dim = 0.0;
                    for(int k = 0; k < numEV; k++) {
                        Vector c1 = df.EigenSystem.GetEigenVector(k);
                        for(int l = 0; l < numEV; l++) {
                            Vector c2 = df.EigenSystem.GetEigenVector(l);
                            double de = es[k] - es[l];
                            double d = f[k] * f[l] * c1[i] * c2[j];
                            dre += d * System.Math.Cos(de * t);
                            dim += d * System.Math.Sin(de * t);
                        }
                    }
                    dmre[i, j] = dre;
                    dmim[i, j] = dim;
                }
            }

            List result = new List();
            result.Add(dmre);
            result.Add(dmim);

            return result;
        }
    }
}

using System;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def
{
    /// <summary>
    /// Returns the mean value of a Peres operator after a quench
    /// </summary>
    public class QuenchPeres : Fnc
    {
        public override string Help { get { return Messages.HelpQuenchPeres; } }

        protected override void CreateParameters() {
            this.SetNumParams(6);

            this.SetParam(0, true, true, false, Messages.PInitialSystem, Messages.PInitialSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PFinalSystem, Messages.PFinalSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(2, true, true, false, Messages.PInitialState, Messages.PInitialStateDescription, 0, typeof(int), typeof(Vector));
            this.SetParam(3, true, true, false, Messages.PTime, Messages.PTimeDescription, null, typeof(Vector));
            this.SetParam(4, true, true, false, Messages.PPeresOperatorType, Messages.PPeresOperatorTypeDescription, null, typeof(int));
            this.SetParam(5, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 1E-4, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem qi = arguments[0] as IQuantumSystem;
            IQuantumSystem qf = arguments[1] as IQuantumSystem;

            int lengthi = qi.EigenSystem.GetEigenVector(0).Length;
            int numEVi = qi.EigenSystem.NumEV;

            int lengthf = qf.EigenSystem.GetEigenVector(0).Length;
            int numEVf = qf.EigenSystem.NumEV;

            Vector ef = qf.EigenSystem.GetEigenValues() as Vector;

            int type = (int)arguments[4];

            if (guider != null)
                guider.Write(string.Format("L=({0},{1}) ", lengthi, lengthf));

            DateTime startTime = DateTime.Now;

            Vector initial = null;
            if (arguments[2] is Vector) {
                initial = arguments[2] as Vector;
                initial /= initial.EuklideanNorm();
            }
            else {
                initial = new Vector(numEVi);
                initial[(int)arguments[2]] = 1;
            }

            Vector time = arguments[3] as Vector;
            int tLength = time.Length;

            Vector b = new Vector(lengthi);
            for (int j = 0; j < numEVi; j++) {
                Vector cj = qi.EigenSystem.GetEigenVector(j);
                for (int k = 0; k < lengthi; k++)
                    b[k] += cj[k] * initial[j];
            }
            if (guider != null)
                guider.Write(string.Format("B={0:0.000} ", b.EuklideanNorm()));

            Vector u = new Vector(numEVf);
            for (int l = 0; l < numEVf; l++) {
                Vector dl = qf.EigenSystem.GetEigenVector(l);
                for (int k = 0; k < lengthi; k++)
                    u[l] += b[k] * dl[k];
            }

            DickeBasisIndex index = qf.EigenSystem.BasisIndex as DickeBasisIndex;

            Vector peres = null;
            if (type == 0)
                peres = new Vector(index.N);
            else
                peres = new Vector(index.M);

            // Calculation optimalization
            int mini = 0;
            int maxi = -1;
            double precision = (double)arguments[5] / System.Math.Sqrt(System.Math.Abs(peres.MaxAbs()));
            for(int l = 0; l < numEVf; l++) {
                if (System.Math.Abs(u[l]) < precision) {
                    if (maxi < 0)
                        mini = l;
                }
                else
                    maxi = l;                
            }

            Vector r = new Vector(tLength);
            double mean = 0.0;
            for (int l = mini; l <= maxi; l++) {
                Vector dl = qf.EigenSystem.GetEigenVector(l);
                double v = 0.0;
                for (int m = 0; m < lengthf; m++)
                    v += dl[m] * dl[m] * peres[m];
                mean += u[l] * u[l] * v;
            }
            if (guider != null)
                guider.Write(string.Format("Mean={0:0.000}...", mean));

            r += mean;

            if (tLength > 0) {
                for (int l = mini; l <= maxi; l++) {
                    Vector dl = qf.EigenSystem.GetEigenVector(l);
                    for (int lp = System.Math.Max(mini, l + 1); lp <= maxi; lp++) {
                        Vector dlp = qf.EigenSystem.GetEigenVector(lp);
                        double v = 0;
                        for (int m = 0; m < lengthf; m++) {
                            v += dl[m] * dlp[m] * peres[m];
                        }
                        v *= 2.0 * u[l] * u[lp];

                        double de = ef[l] - ef[lp];
                        for (int i = 0; i < tLength; i++)
                            r[i] += v * System.Math.Cos(de * time[i]);
                    }
                }
            }

            if (guider != null)
                guider.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));

            List result = new List();
            result.Add(r);
            result.Add(mean);

            return result;
        }
    }
}

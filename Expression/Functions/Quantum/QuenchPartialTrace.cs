using System;
using System.Collections;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def
{
    /// <summary>
    /// Returns spin partial trace after a quench
    /// </summary>
    public class QuenchPartialTrace : Fnc
    {
        public override string Help { get { return Messages.HelpQuenchPartialTrace; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PInitialSystem, Messages.PInitialSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PFinalSystem, Messages.PFinalSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(2, true, true, false, Messages.PInitialState, Messages.PInitialStateDescription, 0, typeof(int), typeof(Vector));
            this.SetParam(3, true, true, false, Messages.PTime, Messages.PTimeDescription, null, typeof(Vector));
            this.SetParam(4, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 1E-4, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem qi = arguments[0] as IQuantumSystem;
            IQuantumSystem qf = arguments[1] as IQuantumSystem;

            int lengthi = qi.EigenSystem.GetEigenVector(0).Length;
            int numEVi = qi.EigenSystem.NumEV;

            int lengthf = qf.EigenSystem.GetEigenVector(0).Length;
            int numEVf = qf.EigenSystem.NumEV;

            Vector ef = qf.EigenSystem.GetEigenValues() as Vector;

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
            if (guider != null)
                guider.Write(string.Format("U={0:0.000}...", u.EuklideanNorm()));

            // Calculation optimalization
            int mini = 0;
            int maxi = -1;
            double precision = (double)arguments[4];

            for (int l = 0; l < numEVf; l++) {
                if (System.Math.Abs(u[l]) < precision) {
                    if (maxi < 0)
                        mini = l;
                }
                else
                    maxi = l;
            }

            int[] minif = new int[numEVf];
            int[] maxif = new int[numEVf];

            for (int k = 0; k < numEVf; k++) {
                minif[k] = 0;
                maxif[k] = -1;

                Vector dk = qf.EigenSystem.GetEigenVector(k);

                for (int l = 0; l < lengthf; l++) {
                    if (System.Math.Abs(dk[l]) < precision) {
                        if (maxif[k] < 0)
                            minif[k] = l;
                    }
                    else
                        maxif[k] = l;
                }
            }

            DickeBasisIndex index = qf.EigenSystem.BasisIndex as DickeBasisIndex;

            int dimj = index.J + 1;

            Matrix result = new Matrix(dimj);

            for (int l = mini; l < maxi; l++)
                for (int lp = mini; lp < maxi; lp++) {
                    Vector dl = qf.EigenSystem.GetEigenVector(l);
                    Vector dlp = qf.EigenSystem.GetEigenVector(lp);

                    double de = ef[l] - ef[lp];

                    for (int m = minif[l]; m < maxif[l]; m++)
                        for (int mp = minif[lp]; mp < maxif[lp]; mp++) {
                            int i1 = (index.M[m] + index.J) / 2;
                            int i2 = (index.M[mp] + index.J) / 2;

                            double d = u[l] * u[lp] * dl[m] * dlp[mp];
                            result[i1, i2] += d;
                        }
                }

            if (guider != null)
                guider.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));

            return result;
        }
    }
}

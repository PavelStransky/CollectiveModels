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

            this.SetParam(0, true, true, false, Messages.PInitialSystem, Messages.PInitialSystemDescription, null, typeof(QuantumDicke));
            this.SetParam(1, true, true, false, Messages.PFinalSystem, Messages.PFinalSystemDescription, null, typeof(QuantumDicke));
            this.SetParam(2, true, true, false, Messages.PInitialState, Messages.PInitialStateDescription, 0, typeof(int), typeof(Vector));
            this.SetParam(3, true, true, false, Messages.PTime, Messages.PTimeDescription, null, typeof(Vector));
            this.SetParam(4, true, true, false, Messages.PPeresOperatorType, Messages.PPeresOperatorTypeDescription, null, typeof(int));
            this.SetParam(5, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 1E-4, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            QuantumDicke qi = arguments[0] as QuantumDicke;
            QuantumDicke qf = arguments[1] as QuantumDicke;

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
            Vector r = new Vector(tLength);
            double mean = 0.0;

            // Diagonal operators
            if (type <= 1) {     // N
                if (type == 0)
                    peres = new Vector(index.N);
                else            // Jz
                    peres = 0.5 * (new Vector(index.M));

                // Calculation optimalization
                int mini = 0;
                int maxi = -1;
                double precision = (double)arguments[5] / System.Math.Sqrt(System.Math.Abs(peres.MaxAbs()));
                for (int l = 0; l < numEVf; l++) {
                    if (System.Math.Abs(u[l]) < precision) {
                        if (maxi < 0)
                            mini = l;
                    }
                    else
                        maxi = l;
                }

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
            }

            // Nondiagonal operator O = J_{+-} for Jx, Jy
            else if(type <= 3) {
                // Calculation optimalization
                int mini = 0;
                int maxi = -1;
                double precision = (double)arguments[5] / System.Math.Sqrt(qf.ShiftPlus(index.J, 0));
                for (int l = 0; l < numEVf; l++) {
                    if (System.Math.Abs(u[l]) < precision) {
                        if (maxi < 0)
                            mini = l;
                    }
                    else
                        maxi = l;
                }

                for (int l = mini; l <= maxi; l++) {
                    Vector dl = qf.EigenSystem.GetEigenVector(l);
                    double vp = 0.0;
                    double vm = 0.0;

                    for (int m = 0; m < lengthf; m++) {
                        if (index.M[m] < index.J)
                            vp += dl[m + 1] * dl[m] * qf.ShiftPlus(index.J, index.M[m]);
                        if (index.M[m] > -index.J)
                            vm += dl[m - 1] * dl[m] * qf.ShiftMinus(index.J, index.M[m]);
                    }

                    if (type == 2)               // Jx
                        mean += u[l] * u[l] * (vp + vm);
                    else
                        mean += u[l] * u[l] * (vp - vm);

                }
                if (guider != null)
                    guider.Write(string.Format("Mean={0:0.000}...", mean));

                r += mean;

                if (tLength > 0) {
                    for (int l = mini; l <= maxi; l++) {
                        Vector dl = qf.EigenSystem.GetEigenVector(l);
                        for (int lp = System.Math.Max(mini, l + 1); lp <= maxi; lp++) {
                            Vector dlp = qf.EigenSystem.GetEigenVector(lp);

                            double vp = 0.0;
                            double vm = 0.0;

                            for (int m = 0; m < lengthf; m++) {
                                if (index.M[m] < index.J)
                                    vp += dl[m + 1] * dlp[m] * qf.ShiftPlus(index.J, index.M[m]);
                                if (index.M[m] > -index.J)
                                    vm += dl[m - 1] * dlp[m] * qf.ShiftMinus(index.J, index.M[m]);
                            }

                            vp *= 2.0 * u[l] * u[lp];
                            vm *= 2.0 * u[l] * u[lp];

                            double de = ef[l] - ef[lp];

                            if (type == 2) {        // Jx
                                for (int i = 0; i < tLength; i++)
                                    r[i] += (vp + vm) * System.Math.Cos(de * time[i]);
                            }
                            else {                  // Jy
                                for (int i = 0; i < tLength; i++)
                                    r[i] += (vp - vm) * System.Math.Sin(de * time[i]);
                            }
                        }
                    }
                }

                r *= 0.5;
                mean *= 0.5;
            }

            // Nondiagonal operator O = J_{+-}^{2} for Jx^{2}, Jy^{2}
            else {
                // Calculation optimalization
                int mini = 0;
                int maxi = -1;
                double precision = (double)arguments[5] / qf.ShiftPlus(index.J, 0);
                for (int l = 0; l < numEVf; l++) {
                    if (System.Math.Abs(u[l]) < precision) {
                        if (maxi < 0)
                            mini = l;
                    }
                    else
                        maxi = l;
                }

                for (int l = mini; l <= maxi; l++) {
                    Vector dl = qf.EigenSystem.GetEigenVector(l);
                    double vpp = 0.0;
                    double vpm = 0.0;
                    double vmp = 0.0;
                    double vmm = 0.0;

                    for (int m = 0; m < lengthf; m++) {
                        if (index.M[m] < index.J - 2)
                            vpp += dl[m + 2] * dl[m] * qf.ShiftPlus(index.J, index.M[m]) * qf.ShiftPlus(index.J, index.M[m + 1]);
                        if (index.M[m] > -index.J + 2)
                            vmm += dl[m - 2] * dl[m] * qf.ShiftMinus(index.J, index.M[m]) * qf.ShiftMinus(index.J, index.M[m - 1]);
                        if (index.M[m] < index.J)
                            vpm += dl[m] * dl[m] * qf.ShiftPlus(index.J, index.M[m]) * qf.ShiftMinus(index.J, index.M[m + 1]);
                        if (index.M[m] > -index.J)
                            vmp += dl[m] * dl[m] * qf.ShiftMinus(index.J, index.M[m]) * qf.ShiftPlus(index.J, index.M[m - 1]);
                    }

                    if (type == 4)               // Jx^2
                        mean += u[l] * u[l] * (vpp + vmm + vpm + vmp);
                    else
                        mean -= u[l] * u[l] * (vpp + vmm - vpm - vmp);

                }
                if (guider != null)
                    guider.Write(string.Format("Mean={0:0.000}...", mean));

                r += mean;

                if (tLength > 0) {
                    for (int l = mini; l <= maxi; l++) {
                        Vector dl = qf.EigenSystem.GetEigenVector(l);
                        for (int lp = System.Math.Max(mini, l + 1); lp <= maxi; lp++) {
                            Vector dlp = qf.EigenSystem.GetEigenVector(lp);

                            double vpp = 0.0;
                            double vpm = 0.0;
                            double vmp = 0.0;
                            double vmm = 0.0;

                            for (int m = 0; m < lengthf; m++) {
                                if (index.M[m] < index.J - 2)
                                    vpp += dl[m + 2] * dlp[m] * qf.ShiftPlus(index.J, index.M[m]) * qf.ShiftPlus(index.J, index.M[m + 1]);
                                if (index.M[m] > -index.J + 2)
                                    vmm += dl[m - 2] * dlp[m] * qf.ShiftMinus(index.J, index.M[m]) * qf.ShiftMinus(index.J, index.M[m - 1]);
                                if (index.M[m] < index.J)
                                    vpm += dl[m] * dlp[m] * qf.ShiftPlus(index.J, index.M[m]) * qf.ShiftMinus(index.J, index.M[m + 1]);
                                if (index.M[m] > -index.J)
                                    vmp += dl[m] * dlp[m] * qf.ShiftMinus(index.J, index.M[m]) * qf.ShiftPlus(index.J, index.M[m - 1]);
                            }

                            vpp *= 2.0 * u[l] * u[lp];
                            vmm *= 2.0 * u[l] * u[lp];
                            vpm *= 2.0 * u[l] * u[lp];
                            vmp *= 2.0 * u[l] * u[lp];

                            double de = ef[l] - ef[lp];

                            if (type == 4) {        // Jx^2
                                for (int i = 0; i < tLength; i++)
                                    r[i] += (vpp + vmm) * System.Math.Cos(de * time[i]);
                            }
                            else {                  // Jy^2
                                for (int i = 0; i < tLength; i++)
                                    r[i] -= (vpp + vmm) * System.Math.Sin(de * time[i]);
                            }
                        }
                    }
                }

                r *= 0.25;
                mean *= 0.25;
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

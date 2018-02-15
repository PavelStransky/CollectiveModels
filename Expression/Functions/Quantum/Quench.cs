using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the survival probability and LDS of a quench
    /// </summary>
    public class Quench : Fnc {
        public override string Help { get { return Messages.HelpQuench; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PInitialSystem, Messages.PInitialSystemDescription, null, typeof(QuantumDicke));
            this.SetParam(1, true, true, false, Messages.PFinalSystem, Messages.PFinalSystemDescription, null, typeof(QuantumDicke));
            this.SetParam(2, true, true, false, Messages.PInitialState, Messages.PInitialStateDescription, 0, typeof(int), typeof(Vector));
            this.SetParam(3, true, true, false, Messages.PTime, Messages.PTimeDescription, null, typeof(Vector));
            this.SetParam(4, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 1E-4, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            QuantumDicke di = arguments[0] as QuantumDicke;
            QuantumDicke df = arguments[1] as QuantumDicke;

            int lengthi = di.EigenSystem.GetEigenVector(0).Length;
            int numEVi = di.EigenSystem.NumEV;

            int lengthf = df.EigenSystem.GetEigenVector(0).Length;
            int numEVf = df.EigenSystem.NumEV;

            if(guider != null)
                guider.Write(string.Format("L=({0},{1}) ", lengthi, lengthf));

            int length = System.Math.Min(lengthi, lengthf);

            Vector initial = null;
            if(arguments[2] is Vector) {
                initial = arguments[2] as Vector;
                initial /= initial.EuklideanNorm();
            }
            else {
                initial = new Vector(lengthi);
                initial[(int)arguments[2]] = 1;
            }

            Vector time = arguments[3] as Vector;
            int tLength = time.Length;

            Vector b = new Vector(lengthi);
            for(int i = 0; i < numEVi; i++) {
                Vector ci = di.EigenSystem.GetEigenVector(i);
                for(int j = 0; j < lengthi; j++)
                    b[j] += ci[j] * initial[i];
            }
            if(guider != null)
                guider.Write(string.Format("B={0:0.000} ", b.EuklideanNorm()));

            Vector d = new Vector(numEVf);
            for(int i = 0; i < numEVf; i++) {
                Vector cf = df.EigenSystem.GetEigenVector(i);
                for(int j = 0; j < length; j++)
                    d[i] += b[j] * cf[j];
                d[i] *= d[i];
            }

            double precision = (double)arguments[4];

            // Heisenberg time
            Vector es = df.EigenSystem.GetEigenValues() as Vector;
            es /= (df.Omega0 * df.J);

            double heisenberg1 = 0;
            double heisenberg2 = 0;
            for(int i = 1; i < numEVf; i++) {
                heisenberg1 += (d[i] + d[i - 1]) * (es[i] - es[i - 1]);
                heisenberg2 += System.Math.Sqrt(d[i] * d[i - 1]) * (es[i] - es[i - 1]);
            }
            heisenberg1 = 4.0 * System.Math.PI / heisenberg1;
            heisenberg2 = 2.0 * System.Math.PI / heisenberg2;

            // Calculation optimalization
            int mini = -1;
            int maxi = -1;
            double maxd = d.Max() * precision;

            for(int i = 0; i < numEVf; i++) {
                if(d[i] > maxd) {
                    if(mini < 0)
                        mini = i;
                    maxi = i;
                }
            }


            if(guider != null)
                guider.Write(string.Format("D={0:0.000}({1}-{2}) ", d.Sum(), mini, maxi));

            double ipr = d.SquaredEuklideanNorm();
            if(guider != null)
                guider.Write(string.Format("IPR={0:0.000} HT=({1:0},{2:0})", ipr, heisenberg1, heisenberg2));

            Vector r = new Vector(tLength);
            for(int i = 0; i < tLength; i++)
                r[i] = ipr;

            int mi = System.Math.Max(1, (maxi - mini) / 10);

            int l = 0;
            PointVector ac = new PointVector((maxi - mini + 1) * (maxi - mini) / 2);

            for(int j = mini; j <= maxi; j++) {
                if(guider != null && j % mi == 0)
                    guider.Write(".");
                for(int k = j + 1; k <= maxi; k++) {
                    double dx = d[j] * d[k];
                    double de = es[k] - es[j];
                    ac[l++] = new PointD(de, dx);
                    for(int i = 0; i < tLength; i++)
                        r[i] += 2.0 * dx * System.Math.Cos(de * time[i]);
                }
            }

            List result = new List();
            result.Add(r);
            result.Add(d);
            result.Add(ipr);
            result.Add(mini);
            result.Add(maxi);
            result.Add(ac);
            result.Add(new PointD(heisenberg1, heisenberg2));

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class JaynesCummings: IQuantumSystem, IExportable, IEntanglement {
        // Systém s vlastními hodnotami
        protected EigenSystem eigenSystem;
        private Matrix[] qmn = null;

        // parametry
        protected double omega, omega0, lambda;

        /// <summary>
        /// Systém vlastních hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected JaynesCummings() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public JaynesCummings(double omega, double omega0, double lambda) {
            this.omega = omega;
            this.omega0 = omega0;
            this.lambda = lambda;

            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvoøí instanci tøídy s parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public virtual BasisIndex CreateBasisIndex(Vector basisParams) {
            return new JaynesCummingsBasisIndex(basisParams);
        }

        /// <summary>
        /// Naplní Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public virtual void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            JaynesCummingsBasisIndex index = basisIndex as JaynesCummingsBasisIndex;

            int dim = index.Length;
            int j = index.J;
//            double l = this.lambda / System.Math.Sqrt(index.M2);
            double l = this.lambda / System.Math.Sqrt(2 * j);

            for(int i = 0; i < dim; i++) {
                int nb = index.Nb[i];
                int m = index.M[i];

                int i1 = index[nb + 1, m - 1];
                int i2 = index[nb - 1, m + 1];

                matrix[i, i] = this.omega * nb + this.omega0 * m;
                if(i1 >= 0)
                    matrix[i1, i] = l * this.ShiftMinus(j, m) * System.Math.Sqrt(nb + 1);
                if(i2 >= 0)
                    matrix[i2, i] = l * this.ShiftPlus(j, m) * System.Math.Sqrt(nb);
            }
        }

        protected double ShiftPlus(int l, int m) {
            if(m > l || m < -l)
                return 0;
            return System.Math.Sqrt((l - m) * (l + m + 1));
        }
        protected double ShiftMinus(int l, int m) {
            if(m > l || m < -l)
                return 0;
            return System.Math.Sqrt((l + m) * (l - m + 1));
        }

        /// <summary>
        /// Parciální stopa
        /// </summary>
        /// <param name="n">Index vlastní hodnoty</param>
        /// <returns>Matice hustoty s odtraceovanými spiny</returns>
        public Matrix PartialTrace(int n) {
            JaynesCummingsBasisIndex index = this.eigenSystem.BasisIndex as JaynesCummingsBasisIndex;

            int dim = index.Length;
            int nbmin = index.MinNb;
            int nbmax = index.MaxNb;
            int j = index.J;

            Vector ev = this.eigenSystem.GetEigenVector(n);

            Matrix result = new Matrix(nbmax - nbmin + 1);
            for (int i = 0; i < index.Length; i++)
                for (int k = 0; k < index.Length; k++) {
                    if (index.M[i] == index.M[k])
                        result[index.Nb[i] - nbmin, index.Nb[k] - nbmin] += ev[i] * ev[k];
                }

            return result;
        }

        /// <summary>
        /// Druhý invariant
        /// </summary>
        /// <param name="type">Typ Peresova operátoru:
        /// 0 ... N
        /// 1 ... M
        /// 2 ... N + M
        /// </param>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        public Vector PeresInvariant(int type) {
            JaynesCummingsBasisIndex index = this.eigenSystem.BasisIndex as JaynesCummingsBasisIndex;

            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenSystem.GetEigenVector(i);
                int length = ev.Length;

                for(int j = 0; j < length; j++) {
                    double koef = 0.0;
                                              
                    switch(type) {
                        case 0:
                            koef = index.Nb[j];
                            break;
                        case 1:
                            koef = 0.5 * index.M[j];
                            break;
                    }

                    result[i] += ev[j] * ev[j] * koef;
                }
            }

            return result;
        }

        private class Qmn {
            private JaynesCummings[] jcs;
            private Matrix[] qmn;
            private Matrix[] de;

            int minN, maxN, minM, maxM;

            int numEV, size;

            public Qmn(JaynesCummings jc) {
                this.jcs = new JaynesCummings[5];
                JaynesCummingsBasisIndex index = jc.eigenSystem.BasisIndex as JaynesCummingsBasisIndex;

                for(int i = 0; i < 5; i++) {
                    if(i == 2)
                        this.jcs[i] = jc;
                    else {
                        this.jcs[i] = new JaynesCummings(jc.omega, jc.omega0, jc.lambda);
                        Vector v = new Vector(2);
                        v[0] = index.M2 - 2 + i;
                        v[1] = index.J;
                        this.jcs[i].EigenSystem.Diagonalize(v, true, 0, null, ComputeMethod.LAPACKBand);
                    }
                }

                this.minN = System.Math.Max(0, index.M2 - 2 - 2 * index.J);
                this.maxN = index.M2 + 2;
                this.minM = -index.J;
                this.maxM = System.Math.Min(index.M2 + 2 - index.J, index.J);

                this.size = this.maxM - this.minM + 1;

                this.CalculateQmn();
            }

            private void CalculateQmn() {
                this.qmn = new Matrix[4];
                this.de = new Matrix[4];

                double c = 1.0 / System.Math.Sqrt(2);

                for(int l = 0; l < 4; l++) {
                    JaynesCummingsBasisIndex index1 = this.jcs[l].eigenSystem.BasisIndex as JaynesCummingsBasisIndex;
                    JaynesCummingsBasisIndex index2 = this.jcs[l + 1].eigenSystem.BasisIndex as JaynesCummingsBasisIndex;

                    int count1 = index1.Length;
                    int count2 = index2.Length;

                    this.qmn[l] = new Matrix(count1, count2);
                    this.de[l] = new Matrix(count1, count2);

                    Vector e1 = this.jcs[l].eigenSystem.GetEigenValues() as Vector;
                    Vector e2 = this.jcs[l + 1].eigenSystem.GetEigenValues() as Vector;

                    for(int i = 0; i < count1; i++) {
                        Vector ev1 = this.jcs[l].eigenSystem.GetEigenVector(i);
                        for(int j = 0; j < count2; j++) {
                            Vector ev2 = this.jcs[l + 1].eigenSystem.GetEigenVector(j);
                            double d = 0.0;
                            for(int k = 0; k < count2; k++) {
                                int n = index2.Nb[k];
                                int m = index2.M[k];

                                int k0 = index1[n - 1, m];

                                if(k0 >= 0)
                                    d += ev1[k0] * System.Math.Sqrt(n + 1) * ev2[k];
                            }

                            this.qmn[l][i, j] = c * d;
                            this.de[l][i, j] = e2[j] - e1[i];
                        }
                    }
                }
            }


            public Vector CalculateOTOC(int s, Vector time, IOutputWriter writer) {

                int tl = time.Length;
                Vector result = new Vector(tl);

                // M, M
                int count = (this.jcs[2].eigenSystem.BasisIndex as JaynesCummingsBasisIndex).Length;

                for(int i = 0; i < count; i++) {
                    Vector dr = new Vector(tl);
                    Vector di = new Vector(tl);

                    // M, M-1
                    int count1 = this.qmn[1].LengthX;

                    for(int j = 0; j < count1; j++) {
                        double a = this.qmn[1][j, s] * this.qmn[1][j, i];
                        double de1 = a * this.de[1][j, s];
                        double de2 = a * this.de[1][j, i];
                        for(int k = 0; k < tl; k++) {
                            double t = time[k];
                            dr[k] += de2 * System.Math.Cos(de1 * t) - de1 * System.Math.Cos(de2 * t);
                            di[k] += de2 * System.Math.Sin(de1 * t) - de1 * System.Math.Sin(de2 * t);
                        }
                    }

                    // M, M+1
                    count1 = this.qmn[2].LengthY;

                    for(int j = 0; j < count1; j++) {
                        double a = this.qmn[2][s, j] * this.qmn[2][i, j];
                        double de1 = a * this.de[2][s, j];
                        double de2 = a * this.de[2][i, j];
                        for(int k = 0; k < tl; k++) {
                            double t = time[k];
                            dr[k] += de2 * System.Math.Cos(de1 * t) - de1 * System.Math.Cos(de2 * t);
                            di[k] += de2 * System.Math.Sin(de1 * t) - de1 * System.Math.Sin(de2 * t);
                        }
                    }

                    for(int k = 0; k < tl; k++)
                        result[k] += dr[k] * dr[k] + di[k] * di[k];
                }
                
                // M, M-2
                count = (this.jcs[0].eigenSystem.BasisIndex as JaynesCummingsBasisIndex).Length;

                for(int i = 0; i < count; i++) {
                    Vector dr = new Vector(tl);
                    Vector di = new Vector(tl);

                    // M, M-1, M-2
                    int count1 = this.qmn[1].LengthX;

                    for(int j = 0; j < count1; j++) {
                        double a = this.qmn[1][j, s] * this.qmn[0][i, j];
                        double de1 = a * this.de[1][j, s];
                        double de2 = a * this.de[0][i, j];
                        for(int k = 0; k < tl; k++) {
                            double t = time[k];
                            dr[k] += de2 * System.Math.Cos(de1 * t) - de1 * System.Math.Cos(de2 * t);
                            di[k] += de2 * System.Math.Sin(de1 * t) - de1 * System.Math.Sin(de2 * t);
                        }
                    }

                    for(int k = 0; k < tl; k++)
                        result[k] += dr[k] * dr[k] + di[k] * di[k];
                }

                // M, M+2
                count = (this.jcs[4].eigenSystem.BasisIndex as JaynesCummingsBasisIndex).Length;

                for(int i = 0; i < count; i++) {
                    Vector dr = new Vector(tl);
                    Vector di = new Vector(tl);

                    // M, M+1, M+2
                    int count1 = this.qmn[2].LengthY;

                    for(int j = 0; j < count1; j++) {
                        double a = this.qmn[2][s, j] * this.qmn[3][j, i];
                        double de1 = a * this.de[2][s, j];
                        double de2 = a * this.de[3][j, i];
                        for(int k = 0; k < tl; k++) {
                            double t = time[k];
                            dr[k] += de2 * System.Math.Cos(de1 * t) - de1 * System.Math.Cos(de2 * t);
                            di[k] += de2 * System.Math.Sin(de1 * t) - de1 * System.Math.Sin(de2 * t);
                        }
                    }

                    for(int k = 0; k < tl; k++)
                        result[k] += dr[k] * dr[k] + di[k] * di[k];
                }

                return result;
            }
        }

        /// <summary>
        /// Mikrokanonický OTOC pro stav s a èasy t
        /// </summary>
        /// <param name="s">Stav</param>
        /// <param name="time">Èasy</param>
        /// <param name="precision">Pøesnost</param>
        public Vector OTOC(int s, Vector time, double precision, IOutputWriter writer) {
            Qmn qmn = new Qmn(this);

            return qmn.CalculateOTOC(s, time, writer);
        }

        public double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] x) {
            throw new NotImpException(this, "ProbabilityAmplitude");
        }

        public object ProbabilityDensity(int[] n, IOutputWriter writer, params Vector[] interval) {
            throw new NotImpException(this, "ProbabilityDensity");
        }

       #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.eigenSystem, "EigenSystem");
            param.Add(this.omega, "Omega");
            param.Add(this.omega0, "Omega0");
            param.Add(this.lambda, "Lambda");
            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public JaynesCummings(Core.Import import) {
            IEParam param = new IEParam(import);
            this.eigenSystem = (EigenSystem)param.Get();
            this.omega = (double)param.Get(0.0);
            this.omega0 = (double)param.Get(0.0);
            this.lambda = (double)param.Get(0.0);
            this.eigenSystem.SetParrentQuantumSystem(this);
        }
        #endregion
    }
}
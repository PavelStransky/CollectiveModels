using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class QuantumDP: DoublePendulum, IQuantumSystem {
        // Systém s vlastními hodnotami
        private EigenSystem eigenSystem;

        // Parametr nejednoznaènosti kvantování (typicky z rozmezí 0...1)
        private double a;

        /// <summary>
        /// Systém vlastních hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected QuantumDP() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="mu">Pomìr hmotností tìles</param>
        /// <param name="lambda">Pomìr délek</param>
        /// <param name="gamma">Gravitaèní parametr</param>
        /// <param name="a">Parametr nejednoznaènosti kvantování</param>
        public QuantumDP(double mu, double lambda, double gamma, double a)
            : base(mu, lambda, gamma) {
            this.a = a;
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvoøí instanci tøídy s parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public BasisIndex CreateBasisIndex(Vector basisParams) {
            return new DPBasisIndex(basisParams);
        }

        /// <summary>
        /// Naplní Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            DPBasisIndex index = basisIndex as DPBasisIndex;

            int maxm1 = index.MaxM1;
            int maxm2 = index.MaxM2;

            int minm1 = index.Positive ? 0 : -maxm1;

            int dim = index.Length;

            double sqrtmu = System.Math.Sqrt(1.0 + this.Mu);
            double c1 = (1.0 + this.Mu + this.Mu * this.Lambda * this.Lambda) / (this.Mu * this.Lambda * this.Lambda);
            double c2 = (2.0 + this.Mu - 2.0 * sqrtmu) / this.Mu;
            double c3 = (sqrtmu - 1.0) / (this.Mu * this.Lambda);

            for(int m1 = minm1; m1 <= maxm1; m1++) {
                for(int m2 = -maxm2; m2 <= maxm2; m2++) {
                    int i = index[m1, m2];

                    // Kinetic term;
                    for(int m2p = -maxm2; m2p <= maxm2; m2p++) {
                        int j = index[m1, m2p];

                        double m2m2p = 0.5 * this.a * (m2 * m2 + m2p * m2p) + (1.0 - a) * m2 * m2p;

                        if((m2 - m2p) % 2 == 0) {
                            double c2p = System.Math.Pow(c2, System.Math.Abs((m2 - m2p) / 2));
                            matrix[i, j] = 1.0 / sqrtmu * (m1 * (m1 - m2 - m2p) + m2m2p * c1) * c2p;
                        }
                        else {
                            int pn = (m2 - m2p - 1) / 2;
                            if(pn < 0)
                                pn = System.Math.Abs(pn) - 1;
                            double c2p = System.Math.Pow(c2, pn);
                            matrix[i, j] = c3 * (2 * m2m2p - m1 * (m2 + m2p)) * c2p;
                        }
                    }

                    // Potential
                    int im = index[m1 - 1, m2];
                    int ip = index[m1 + 1, m2];

                    double mu1 = this.Mu + 1;
                    matrix[i, i] += this.Gamma * (mu1 + this.Mu * this.Lambda);
                    if(im >= 0)
                        matrix[i, im] += -0.25 * this.Gamma * mu1;
                    if(ip >= 0)
                        matrix[i, ip] += -0.25 * this.Gamma * mu1;

                    im = index[m1 - 1, m2 - 1];
                    ip = index[m1 + 1, m2 + 1];
                    if(im >= 0)
                        matrix[i, im] += -0.25 * this.Gamma * this.Mu * this.Lambda;
                    if(ip >= 0)
                        matrix[i, ip] += -0.25 * this.Gamma * this.Mu * this.Lambda;
                }
            }
        }

        /// <summary>
        /// Peresùv invariant
        /// </summary>
        /// <param name="type">Typ (0 - H0, 1 - L1, 2 - L2, 3 - L1^2, 4 - L2^2)</param>
        public Vector PeresInvariant(int type) {
            if(type == 0)
                return this.PeresInvariantHPrime();

            DPBasisIndex index = this.eigenSystem.BasisIndex as DPBasisIndex;

            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenSystem.GetEigenVector(i);
                int length = ev.Length;

                for(int j = 0; j < length; j++) {
                    double koef = 0.0;
                    switch(type) {
                        case 1:
                            koef = index.M1[j];
                            break;
                        case 2:
                            koef = index.M2[j];
                            break;
                        case 3:
                            koef = index.M1[j]; koef *= koef;
                            break;
                        case 4:
                            koef = index.M2[j]; koef *= koef;
                            break;
                    }

                    result[i] += ev[j] * ev[j] * koef;
                }
            }

            return result;
        }

        /// <summary>
        /// Druhý invariant pro operátor H0
        /// </summary>
        protected Vector PeresInvariantHPrime() {
            DPBasisIndex index = this.eigenSystem.BasisIndex as DPBasisIndex;
            int maxm1 = index.MaxM1;
            int maxm2 = index.MaxM2;
            int dim = index.Length;

            int minm1 = index.Positive ? 0 : -maxm1;

            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            double sqrtmu = System.Math.Sqrt(1.0 + this.Mu);
            double c1 = (1.0 + this.Mu + this.Mu * this.Lambda * this.Lambda) / (this.Mu * this.Lambda * this.Lambda);
            double c2 = (2.0 + this.Mu - 2.0 * sqrtmu) / this.Mu;
            double c3 = (sqrtmu - 1.0) / (this.Mu * this.Lambda);

            for(int k = 0; k < count; k++) {
                Vector ev = this.eigenSystem.GetEigenVector(k);
                int length = ev.Length;

                double sum = 0.0;

                for(int m1 = minm1; m1 <= maxm1; m1++) {
                    for(int m2 = -maxm2; m2 <= maxm2; m2++) {
                        int i = index[m1, m2];

                        // Potential
                        int im = index[m1 - 1, m2];
                        int ip = index[m1 + 1, m2];

                        double mu1 = this.Mu + 1;
                        sum += ev[i] * ev[i] * (mu1 + this.Mu * this.Lambda);
                        if(im >= 0)
                            sum -= ev[i] * ev[im] * 0.25 * mu1;
                        if(ip >= 0)
                            sum -= ev[i] * ev[ip] * 0.25 * mu1;

                        im = index[m1 - 1, m2 - 1];
                        ip = index[m1 + 1, m2 + 1];
                        if(im >= 0)
                            sum -= ev[i] * ev[im] * 0.25 * this.Mu * this.Lambda;
                        if(ip >= 0)
                            sum -= ev[i] * ev[ip] * 0.25 * this.Mu * this.Lambda;
                    }
                }

                result[k] = sum;
            }
            return result;
        }

        public object ProbabilityDensity(int[] n, IOutputWriter writer, params Vector[] interval) {
            DiscreteInterval intx = new DiscreteInterval(interval[0]);
            DiscreteInterval inty = new DiscreteInterval(interval[1]);

            Matrix[] amplitude = this.AmplitudeMatrix(n, writer, intx, inty);

            int numn = amplitude.Length / 2;
            int numx = amplitude[0].LengthX;
            int numy = amplitude[0].LengthY;

            Matrix[] result = new Matrix[numn];

            for(int l = 0; l < numn; l++) {
                result[l] = new Matrix(numx, numy);

                for(int i = 0; i < numx; i++)
                    for(int j = 0; j < numy; j++)
                        result[l][i, j] = amplitude[l][i, j] * amplitude[l][i, j] + amplitude[l + numn][i, j] * amplitude[l + numn][i, j];
            }

            return result;
        }

        /// <summary>
        /// Reálná èást vlnové funkce
        /// </summary>
        /// <param name="phi1">Úhel 1</param>
        /// <param name="phi2">Úhel 2</param>
        /// <param name="n">Index vlnové funkce</param>
        private double PsiR(double phi1, double phi2, int n) {
            DPBasisIndex index = this.eigenSystem.BasisIndex as DPBasisIndex;
            return System.Math.Cos(index.M1[n] * phi1 + index.M2[n] * phi2) / (2.0 * System.Math.PI);
        }

        /// <summary>
        /// Imaginární èást vlnové funkce
        /// </summary>
        /// <param name="phi1">Úhel 1</param>
        /// <param name="phi2">Úhel 2</param>
        /// <param name="n">Index vlnové funkce</param>
        private double PsiI(double phi1, double phi2, int n) {
            DPBasisIndex index = this.eigenSystem.BasisIndex as DPBasisIndex;
            return System.Math.Sin(index.M1[n] * phi1 + index.M2[n] * phi2) / (2.0 * System.Math.PI);
        }

        /// <summary>
        /// Vrátí matici <n|V|n> amplitudy vlastní funkce n
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="rx">Rozmìry ve smìru x</param>
        /// <param name="ry">Rozmìry ve smìru y</param>
        public virtual Matrix[] AmplitudeMatrix(int[] n, IOutputWriter writer, DiscreteInterval intx, DiscreteInterval inty) {
            int numx = intx.Num;
            int numy = inty.Num;

            int numn = n.Length;

            // Reálná a imaginární èást (proto 2 * numn)
            Matrix[] result = new Matrix[2 * numn];
            for(int i = 0; i < 2 * numn; i++) {
                result[i] = new Matrix(numx, numy);
            }

            int length = this.eigenSystem.BasisIndex.Length;
            int length100 = length / 100;

            DateTime startTime = DateTime.Now;

            for(int k = 0; k < length; k++) {
                BasisCache2D cacheR = new BasisCache2D(intx, inty, k, this.PsiR);
                BasisCache2D cacheI = new BasisCache2D(intx, inty, k, this.PsiI);

                for(int l = 0; l < numn; l++) {
                    Vector ev = this.eigenSystem.GetEigenVector(n[l]);

                    for(int i = 0; i < numx; i++)
                        for(int j = 0; j < numy; j++) {
                            result[l][i, j] += ev[k] * cacheR[i, j];
                            result[l + numn][i, j] += ev[k] * cacheI[i, j];
                        }
                }

                if(writer != null)
                    if((k + 1) % length100 == 0) {
                        writer.Write('.');

                        if(((k + 1) / length100) % 10 == 0) {
                            writer.Write((k + 1) / length100);
                            writer.Write("% ");
                            writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));
                            startTime = DateTime.Now;
                        }
                    }
            }

            return result;
        }

        /// <summary>
        /// Action of L1 and L2 operator
        /// </summary>
        /// <param name="ket">Initial ket</param>
        /// <param name="op">Operator (0...L1, 1...L2)</param>
        public PointVector OperatorAction(PointVector ket, int op) {
            DPBasisIndex index = this.eigenSystem.BasisIndex as DPBasisIndex;
            int dim = index.Length;

            Vector re = ket.VectorX;
            Vector im = ket.VectorY;

            for(int i = 0; i < dim; i++) {
                int m = 0;
                switch(op) {
                    case 0: m = index.M1[i]; break;
                    case 1: m = index.M2[i]; break;
                    case 2: m = index.M1[i]; m *= m; break;
                    case 3: m = index.M2[i]; m *= m; break;
                }

                re[i] *= m;
                im[i] *= m;
            }

            return new PointVector(re, im);
        }

        /// <summary>
        /// Expectation value of the diagonal elements <m1 m2|L|m1 m2>
        /// </summary>
        /// <param name="i">Index of the basis vector</param>
        /// <param name="op">Operator (0...L1, 1...L2)</param>
        public double OperatorExpectDiagonal(int i, int op) {
            DPBasisIndex index = this.eigenSystem.BasisIndex as DPBasisIndex;
            int dim = index.Length;
            int numEV = this.eigenSystem.NumEV;

            double result = 0.0;

            for(int j = 0; j < numEV; j++) {
                Vector ev = this.eigenSystem.GetEigenVector(j);
                for(int k = 0; k < dim; k++) {
                    int m = 0;
                    switch(op) {
                        case 0: m = index.M1[k]; break;
                        case 1: m = index.M2[k]; break;
                        case 2: m = index.M1[k]; m *= m; break;
                        case 3: m = index.M2[k]; m *= m; break;
                    }

                    double d1 = ev[i] * ev[i];
                    double d2 = ev[k] * ev[k];
                    result += d1 * d2 * m;
                }
            }

            return result;
        }

        public double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] x) {
            throw new NotImpException(this, "ProbabilityAmplitude");
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public override void Export(Export export) {
            base.Export(export);

            IEParam param = new IEParam();
            param.Add(this.eigenSystem, "EigenSystem");
            param.Add(this.a, "Quantization");
            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public QuantumDP(Core.Import import)
            : base(import) {
            IEParam param = new IEParam(import);
            this.eigenSystem = (EigenSystem)param.Get();
            this.a = (double)param.Get(0.0);
            this.eigenSystem.SetParrentQuantumSystem(this);
        }

        #endregion
    }
}
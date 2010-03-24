using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class QuantumDP: DoublePendulum, IQuantumSystem {
        // Syst�m s vlastn�mi hodnotami
        private EigenSystem eigenSystem;

        /// <summary>
        /// Syst�m vlastn�ch hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected QuantumDP() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="mu">Pom�r hmotnost� t�les</param>
        /// <param name="lambda">Pom�r d�lek</param>
        /// <param name="gamma">Gravita�n� parametr</param>
        public QuantumDP(double mu, double lambda, double gamma)
            : base(mu, lambda, gamma) {
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvo�� instanci t��dy s parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public BasisIndex CreateBasisIndex(Vector basisParams) {
            return new DPBasisIndex(basisParams);
        }

        /// <summary>
        /// Stopa Hamiltonovy matice
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        public double HamiltonianMatrixTrace(BasisIndex basisIndex) {
            return this.HamiltonianSBMatrix(basisIndex, null).Trace();
        }

        /// <summary>
        /// Vypo��t� Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        public Matrix HamiltonianMatrix(BasisIndex basisIndex, IOutputWriter writer) {
            return (Matrix)this.HamiltonianSBMatrix(basisIndex, writer);
        }

        /// <summary>
        /// Vypo��t� symetrickou Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        public SymmetricBandMatrix HamiltonianSBMatrix(BasisIndex basisIndex, IOutputWriter writer) {
            DPBasisIndex index = basisIndex as DPBasisIndex;

            int maxm1 = index.MaxM1;
            int maxm2 = index.MaxM2;

            int minm1 = index.Positive ? 0 : -maxm1;

            int dim = index.Length;
            SymmetricBandMatrix matrix = new SymmetricBandMatrix(dim, 2 * maxm2 + 2);

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

                        if((m2 - m2p) % 2 == 0) {
                            double c2p = System.Math.Pow(c2, System.Math.Abs((m2 - m2p) / 2));
                            matrix[i, j] = 1.0 / sqrtmu * (m1 * (m1 - m2 - m2p) + m2 * m2p * c1) * c2p;
                        }
                        else {
                            int pn = (m2 - m2p - 1) / 2;
                            if(pn < 0)
                                pn = System.Math.Abs(pn) - 1;
                            double c2p = System.Math.Pow(c2, pn);
                            matrix[i, j] = c3 * (2 * m2 * m2p - m1 * (m2 + m2p)) * c2p;
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

            return matrix;
        }

        /// <summary>
        /// Peres�v invariant
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
        /// Druh� invariant pro oper�tor H0
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
            throw new Exception("The method or operation is not implemented.");
        }

        public double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] x) {
            throw new Exception("The method or operation is not implemented.");
        }

        #region Implementace IExportable
        /// <summary>
        /// Ulo�� v�sledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public override void Export(Export export) {
            base.Export(export);

            IEParam param = new IEParam();
            param.Add(this.eigenSystem, "EigenSystem");
            param.Export(export);
        }

        /// <summary>
        /// Na�te v�sledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public QuantumDP(Core.Import import)
            : base(import) {
            IEParam param = new IEParam(import);
            this.eigenSystem = (EigenSystem)param.Get();
            this.eigenSystem.SetParrentQuantumSystem(this);
        }
 
        #endregion
    }
}

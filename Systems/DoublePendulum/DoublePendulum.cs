using System;
using System.IO;
using System.Collections;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Double pendulum system
    /// </summary>
    /// <remarks>Physical Review E 70, 066218 (2004)</remarks>
    public partial class DoublePendulum: IQuantumSystem, IExportable {
        // Parametry modelu (redukovaná hmotnost, redukovaná délka)
        private double mu, lambda;

        // Gravitaèní parametr
        private double gamma;

        protected Vector eigenValues;
        protected Vector[] eigenVectors = new Vector[0];

        // True if it has been calculated
        protected bool isComputed = false;

        // True if the calculation is in progress
        protected bool isComputing = false;

        /// <summary>
        /// True if it has been calculated
        /// </summary>
        public bool IsComputed { get { return this.isComputed; } }

                /// <summary>
        /// Vlastní hodnoty
        /// </summary>
        public Vector GetEigenValues() {
            return this.eigenValues;
        }

        /// <summary>
        /// Vlastní vektor
        /// </summary>
        /// <param name="i">Index vektoru</param>
        public Vector GetEigenVector(int i) {
            return this.eigenVectors[i];
        }

        /// <summary>
        /// Poèet vlastních vektorù
        /// </summary>
        public int NumEV { get { return this.eigenVectors.Length; } }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected DoublePendulum() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="mu">Pomìr hmotností tìles</param>
        /// <param name="lambda">Pomìr délek</param>
        /// <param name="gamma">Gravitaèní parametr</param>
        public DoublePendulum(double mu, double lambda, double gamma) {
            this.mu = mu;
            this.lambda = lambda;
            this.gamma = gamma;
        }

        /// <summary>
        /// Vypoèítá Hamiltonovu matici
        /// </summary>
        /// <param name="maxm1">Nejvyšší hodnota kvantového èísla 1</param>
        /// <param name="maxm2">Nejvyšší hodnota kvantového èísla 2</param>
        public Matrix HamiltonianMatrix(int maxm1, int maxm2, IOutputWriter writer) {
            return (Matrix)this.HamiltonianSBMatrix(maxm1, maxm2, writer);
        }

        /// <summary>
        /// Vypoèítá symetrickou Hamiltonovu matici
        /// </summary>
        /// <param name="maxm1">Nejvyšší hodnota kvantového èísla 1</param>
        /// <param name="maxm2">Nejvyšší hodnota kvantového èísla 2</param>
        public SymmetricBandMatrix HamiltonianSBMatrix(int maxm1, int maxm2, IOutputWriter writer) {
            int dim = (2 * maxm1 + 1) * (2 * maxm2 + 1);
            SymmetricBandMatrix matrix = new SymmetricBandMatrix(dim, 2 * maxm2 + 2);

            double sqrtmu = System.Math.Sqrt(1.0 + this.mu);
            double c1 = (1.0 + this.mu + this.mu * this.lambda * this.lambda) / (this.mu * this.lambda * this.lambda);
            double c2 = (2.0 + this.mu - 2.0 * sqrtmu) / this.mu;
            double c3 = (sqrtmu - 1.0) / (this.mu * this.lambda);

            for(int m1 = -maxm1; m1 <= maxm1; m1++) {
                int koef = (m1 + maxm1) * (2 * maxm2 + 1);
                for(int m2 = -maxm2; m2 <= maxm2; m2++) {
                    int i = koef + m2 + maxm2;

                    // Kinetic term;
                    for(int m2p = -maxm2; m2p <= maxm2; m2p++) {
                        int j = koef + m2p + maxm2;

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
                    int im = i - (2 * maxm2 + 1);
                    int ip = i + (2 * maxm2 + 1);

                    double mu1 = this.mu + 1;
                    matrix[i, i] += this.gamma * (mu1 + this.mu * this.lambda);
                    if(im >= 0)
                        matrix[i, im] += -0.5 * this.gamma * mu1;
                    if(ip < dim)
                        matrix[i, ip] += -0.5 * this.gamma * mu1;

                    im--; ip++;
                    if(im >= 0)
                        matrix[i, im] += -0.5 * this.gamma * this.mu * this.lambda;
                    if(ip < dim)
                        matrix[i, ip] += -0.5 * this.gamma * this.mu * this.lambda;
                }
            }

            return matrix;
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();

            param.Add(this.mu, "Mu");
            param.Add(this.lambda, "Lambda");
            param.Add(this.gamma, "Gamma");
            param.Add(this.isComputed, "IsComputed");

            if(this.isComputed) {
                param.Add(this.eigenValues, "EigenValues");

                int numEV = this.NumEV;
                param.Add(numEV, "EigenVector Number");

                for(int i = 0; i < numEV; i++)
                    param.Add(this.eigenVectors[i]);
            }

            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public DoublePendulum(Core.Import import) {
            IEParam param = new IEParam(import);

            this.mu = (double)param.Get(1.0);
            this.lambda = (double)param.Get(1.0);
            this.gamma = (double)param.Get(1.0);
            this.isComputed = (bool)param.Get(false);

            if(this.isComputed) {
                this.eigenValues = (Vector)param.Get(null);

                int numEV = (int)param.Get(0);
                this.eigenVectors = new Vector[numEV];

                for(int i = 0; i < numEV; i++)
                    this.eigenVectors[i] = (Vector)param.Get();
            }
        }
        #endregion

        /// <summary>
        /// Vypíše parametry modelu
        /// </summary>
        public override string ToString() {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("mu = {0,10:#####0.000}\nlambda = {1,10:#####0.000}\ngamma = {2,10:#####0.000}\n", this.mu, this.lambda, this.gamma));
            return s.ToString();
        }

        #region IQuantumSystem Members


        public object ProbabilityDensity(int[] n, IOutputWriter writer, params Vector[] interval) {
            throw new Exception("The method or operation is not implemented.");
        }

        public double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] x) {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
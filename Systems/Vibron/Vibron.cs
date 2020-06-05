using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems
{
    public class Vibron : IQuantumSystem, IExportable
    {
        // Systém s vlastními hodnotami
        private EigenSystem eigenSystem;

        // parametry alpha, beta
        private double alpha, beta;

        /// <summary>
        /// Systém vlastních hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected Vibron() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Vibron(double alpha, double beta) {
            this.alpha = alpha;
            this.beta = beta;
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvoří instanci třídy s parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public BasisIndex CreateBasisIndex(Vector basisParams) {
            return new VibronBasisIndex(basisParams);
        }

        /// <summary>
        /// Naplní Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            VibronBasisIndex index = basisIndex as VibronBasisIndex;

            int dim = index.Length;
            int n = index.N;
            int l = index.L;

            double a = this.alpha / n;
            double b = this.beta / (n * n);

            for (int i = 0; i < dim; i++) {
                int np = index.Np[i];

                matrix[i, i] = a * np  + b * (n * (2 * np + 3) - 2 * np * (np + 1) + l * (l + 1));

                // +1
                if (i + 1 < dim) {
                    matrix[i + 1, i] = b * System.Math.Sqrt((n - np - 1) * (n - np) * (np - l + 2) * (np + l + 3));
                }
            }
        }

        /// <summary>
        /// Peresův invariant
        /// </summary>
        /// <param name="type">Typ (0 - H0, 1 - L1, 3 - L1^2)</param>
        public Vector PeresInvariant(int type) {
            throw new NotImpException(this, "PeresInvariant");
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
            param.Add(this.alpha, "Alpha");
            param.Add(this.beta, "Beta");
            param.Export(export);
        }

        /// <summary>
        /// Načte výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public Vibron(Core.Import import) {
            IEParam param = new IEParam(import);
            this.eigenSystem = (EigenSystem)param.Get();
            this.alpha = (double)param.Get(1.0);
            this.beta = (double)param.Get(0.0);
        }

        #endregion
    }
}
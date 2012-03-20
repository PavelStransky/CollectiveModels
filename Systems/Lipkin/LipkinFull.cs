using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class LipkinFull: IQuantumSystem, IExportable {
        // Syst�m s vlastn�mi hodnotami
        private EigenSystem eigenSystem;

        // parametry alpha, omega
        private double alpha, omega;

        /// <summary>
        /// Syst�m vlastn�ch hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected LipkinFull() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinFull(double alpha, double omega) {
            this.alpha = alpha;
            this.omega = omega;
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvo�� instanci t��dy s parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LipkinFullBasisIndex(basisParams);
        }

        /// <summary>
        /// Napln� Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        public void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            LipkinFullBasisIndex index = basisIndex as LipkinFullBasisIndex;

            int dim = index.Length;
            int n = index.N;

            double k = -(1.0 - this.alpha) / n;
            for(int i = 0; i < dim; i++) {
                int l = index.L[i];
                int m = index.M[i];

                double c1 = (n + m) / 2.0;
                matrix[i, i] = this.alpha * c1 + k * this.omega * c1 * c1;

                // -1
                if(i - 1 >= 0 && l == index.L[i - 1]) {
                    matrix[i, i] += k * this.ShiftMinus(l, m) * this.ShiftPlus(l, m - 1);
                    matrix[i - 1, i] = k * this.omega * this.ShiftMinus(l, m) * (n + m - 1);
                }
                // +1
                if(i + 1 < dim && l == index.L[i + 1]) {
                    matrix[i, i] += k * this.ShiftPlus(l, m) * this.ShiftMinus(l, m + 1);
                    matrix[i + 1, i] = k * this.omega * this.ShiftPlus(l, m) * (n + m + 1);
                }
                // -2
                if(i - 2 >= 0 && l == index.L[i - 2])
                    matrix[i - 2, i] = k * this.ShiftMinus(l, m) * this.ShiftMinus(l, m - 1);
                // +2
                if(i + 2 < dim && l == index.L[i + 2])
                    matrix[i + 2, i] = k * this.ShiftPlus(l, m) * this.ShiftPlus(l, m + 1);
            }
        }

        private double ShiftPlus(int l, int m) {
            return System.Math.Sqrt((l - m) * (l + m + 2)) / 2.0;
        }
        private double ShiftMinus(int l, int m) {
            return System.Math.Sqrt((l + m) * (l - m + 2)) / 2.0;
        }

        /// <summary>
        /// Peres�v invariant
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
        /// Ulo�� v�sledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.eigenSystem, "EigenSystem");
            param.Add(this.alpha, "Alpha");
            param.Add(this.omega, "Omega");
            param.Export(export);
        }

        /// <summary>
        /// Na�te v�sledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinFull(Core.Import import) {
            IEParam param = new IEParam(import);
            this.eigenSystem = (EigenSystem)param.Get();
            this.alpha = (double)param.Get(0.0);
            this.omega = (double)param.Get(0.0);
            this.eigenSystem.SetParrentQuantumSystem(this);
        }

        #endregion
    }
}
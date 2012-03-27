using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class LipkinFull: Lipkin, IQuantumSystem {
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LipkinFull() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinFull(double alpha, double omega)
            : base(alpha, omega) {
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvoøí instanci tøídy s parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public virtual BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LipkinFullBasisIndex(basisParams);
        }

        /// <summary>
        /// Naplní Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public virtual void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            LipkinFullBasisIndex index = basisIndex as LipkinFullBasisIndex;

            int dim = index.Length;
            int n = index.N;

            double k = -(1.0 - this.alpha) / n;
            for(int i = 0; i < dim; i++) {
                int l = index.L[i];
                int m = index.M[i];

                double c1 = (n + m) / 2.0;
                matrix[i, i] = this.alpha * c1 + k * this.omega * this.omega * c1 * c1; // 1

                // -1
                if(i - 1 >= 0 && l == index.L[i - 1]) {
                    matrix[i, i] += k * this.ShiftMinus(l, m) * this.ShiftPlus(l, m - 2);
                    matrix[i - 1, i] = k * this.omega * this.ShiftMinus(l, m) * (n + m - 1);
                }
                // +1
                if(i + 1 < dim && l == index.L[i + 1]) {
                    matrix[i, i] += k * this.ShiftPlus(l, m) * this.ShiftMinus(l, m + 2);
                    matrix[i + 1, i] = k * this.omega * this.ShiftPlus(l, m) * (n + m + 1);
                }
                // -2
                if(i - 2 >= 0 && l == index.L[i - 2])
                    matrix[i - 2, i] = k * this.ShiftMinus(l, m) * this.ShiftMinus(l, m - 2);
                // +2
                if(i + 2 < dim && l == index.L[i + 2])
                    matrix[i + 2, i] = k * this.ShiftPlus(l, m) * this.ShiftPlus(l, m + 2);
            }
        }

        protected double ShiftPlus(int l, int m) {
            if(m > l || m < -l)
                return 0;
            return System.Math.Sqrt((l - m) * (l + m + 2)) / 2.0;
        }
        protected double ShiftMinus(int l, int m) {
            if(m > l || m < -l)
                return 0;
            return System.Math.Sqrt((l + m) * (l - m + 2)) / 2.0;
        }

        /// <summary>
        /// Peresùv invariant
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
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinFull(Core.Import import) : base(import) {
            this.eigenSystem.SetParrentQuantumSystem(this);
        }
        #endregion
    }
}
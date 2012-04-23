using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class LipkinOne: LipkinFull, IQuantumSystem, IEntanglement {
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LipkinOne() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinOne(double alpha, double omega)
            : base(alpha, omega) {
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvoøí instanci tøídy s parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public override BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LipkinOneBasisIndex(basisParams);
        }

        /// <summary>
        /// Naplní Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public override void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            LipkinOneBasisIndex index = basisIndex as LipkinOneBasisIndex;

            int dim = index.Length;
            int n = index.N;

            double k = -(1.0 - this.alpha) / (n + 1);
            for(int i = 0; i < dim; i++) {
                int l = index.L[i];
                int m = index.M[i];
                int s = index.S[i];

                double c1 = (n + m + 2.0 * s) / 2.0;
                // 1 + 5
                matrix[i, i] = this.alpha * c1 + k * this.omega * this.omega * c1 * c1;

                // 2 S-
                if(i - 2 >= 0 && l == index.L[i - 2]) {
                    matrix[i, i] += k * this.ShiftMinus(l, m) * this.ShiftPlus(l, m - 2);
                    matrix[i - 2, i] = k * this.omega * this.ShiftMinus(l, m) * (n + m - 1 + 2 * s);
                }
                if(i - 4 >= 0 && l == index.L[i - 4])
                    matrix[i - 4, i] = k * this.ShiftMinus(l, m) * this.ShiftMinus(l, m - 2);
                // 2 S+
                if(i + 2 < dim && l == index.L[i + 2]) {
                    matrix[i, i] += k * this.ShiftPlus(l, m) * this.ShiftMinus(l, m + 2);
                    matrix[i + 2, i] = k * this.omega * this.ShiftPlus(l, m) * (n + m + 1 + 2 * s);
                }
                if(i + 4 < dim && l == index.L[i + 4])
                    matrix[i + 4, i] = k * this.ShiftPlus(l, m) * this.ShiftPlus(l, m + 2);

                // 3 s-
                if(s == 1) {
                    matrix[i, i] += k;
                    matrix[i - 1, i] = k * this.omega * (n + m + 1);
                }
                // 3 s+
                if(s == 0) {
                    matrix[i, i] += k;
                    matrix[i + 1, i] = k * this.omega * (n + m + 1);
                }

                // 4ab
                if(i - 2 >= 0 && l == index.L[i - 2]) {
                    if(s == 1)
                        matrix[i - 3, i] = 2 * k * this.ShiftMinus(l, m);
                    else
                        matrix[i - 1, i] = 2 * k * this.ShiftMinus(l, m);
                }
                // 4cd
                if(i + 2 < dim && l == index.L[i + 2]) {
                    if(s == 1)
                        matrix[i + 1, i] = 2 * k * this.ShiftPlus(l, m);
                    else
                        matrix[i + 3, i] = 2 * k * this.ShiftPlus(l, m);
                }
            }
        }

        /// <summary>
        /// Parciální stopa pøes lázeò
        /// </summary>
        /// <param name="n">Index vlastní hodnoty</param>
        /// <returns>Matice hustoty jednotlivého spinu</returns>
        public Matrix PartialTrace(int n) {
            LipkinOneBasisIndex index = this.eigenSystem.BasisIndex as LipkinOneBasisIndex;

            int dim = index.Length;
            Vector ev = this.eigenSystem.GetEigenVector(n);

            Matrix result = new Matrix(2);
            for(int i = 0; i < dim; i += 2) {
                result[0, 0] += ev[i] * ev[i];
                result[1, 0] += ev[i] * ev[i + 1];
                result[0, 1] += ev[i + 1] * ev[i];
                result[1, 1] += ev[i + 1] * ev[i + 1];
            }
            return result;
        }

        #region Implementace IExportable
        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinOne(Core.Import import) : base(import) { }
        #endregion
    }
}
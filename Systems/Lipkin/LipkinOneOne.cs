using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class LipkinOneOne: LipkinFull, IQuantumSystem, IEntanglement {
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LipkinOneOne() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinOneOne(double alpha, double omega)
            : base(alpha, omega) {
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvoøí instanci tøídy s parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public override BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LipkinOneOneBasisIndex(basisParams);
        }

        /// <summary>
        /// Naplní Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public override void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            LipkinOneOneBasisIndex index = basisIndex as LipkinOneOneBasisIndex;

            int dim = index.Length;
            int n = index.N;

            double k = -(1.0 - this.alpha) / (n + 2);
            for(int i = 0; i < dim; i++) {
                int l = index.L[i];
                int m = index.M[i];
                int s1 = index.S1[i];
                int s2 = index.S2[i];

                double c1 = (n + m + 2.0 * s1 + 2.0 * s2) / 2.0;

                // 1 + 5
                matrix[i, i] = this.alpha * c1 + k * this.omega * this.omega * c1 * c1;

                // 2 S-
                if(i - 4 >= 0 && l == index.L[i - 4]) {
                    matrix[i, i] += k * this.ShiftMinus(l, m) * this.ShiftPlus(l, m - 2);
                    matrix[i - 4, i] = k * this.omega * this.ShiftMinus(l, m) * (n + m - 1 + 2 * s1 + 2 * s2);
                }
                if(i - 8 >= 0 && l == index.L[i - 8])
                    matrix[i - 8, i] = k * this.ShiftMinus(l, m) * this.ShiftMinus(l, m - 2);
                // 2 S+
                if(i + 4 < dim && l == index.L[i + 4]) {
                    matrix[i, i] += k * this.ShiftPlus(l, m) * this.ShiftMinus(l, m + 2);
                    matrix[i + 4, i] = k * this.omega * this.ShiftPlus(l, m) * (n + m + 1 + 2 * s1 + 2 * s2);
                }
                if(i + 8 < dim && l == index.L[i + 8])
                    matrix[i + 8, i] = k * this.ShiftPlus(l, m) * this.ShiftPlus(l, m + 2);

                // 3a s-
                if(s1 == 1) {
                    matrix[i, i] += k;
                    matrix[i - 2, i] = k * this.omega * (n + m + 1 + 2 * s2);
                }
                // 3a s+
                if(s1 == 0) {
                    matrix[i, i] += k;
                    matrix[i + 2, i] = k * this.omega * (n + m + 1 + 2 * s2);
                }

                // 3b s-
                if(s2 == 1) {
                    matrix[i, i] += k;
                    matrix[i - 1, i] = k * this.omega * (n + m + 1 + 2 * s1);
                }
                // 3b s+
                if(s2 == 0) {
                    matrix[i, i] += k;
                    matrix[i + 1, i] = k * this.omega * (n + m + 1 + 2 * s1);
                }

                // 4ab
                if(i - 4 >= 0 && l == index.L[i - 4]) {
                    if(s1 == 1)
                        matrix[i - 6, i] = 2 * k * this.ShiftMinus(l, m);
                    else
                        matrix[i - 2, i] = 2 * k * this.ShiftMinus(l, m);

                    if(s2 == 1)
                        matrix[i - 5, i] = 2 * k * this.ShiftMinus(l, m);
                    else
                        matrix[i - 3, i] = 2 * k * this.ShiftMinus(l, m);
                }
                // 4cd
                if(i + 4 < dim && l == index.L[i + 4]) {
                    if(s1 == 1)
                        matrix[i + 2, i] = 2 * k * this.ShiftPlus(l, m);
                    else
                        matrix[i + 6, i] = 2 * k * this.ShiftPlus(l, m);

                    if(s2 == 1)
                        matrix[i + 3, i] = 2 * k * this.ShiftPlus(l, m);
                    else
                        matrix[i + 5, i] = 2 * k * this.ShiftPlus(l, m);
                }

                // 4ef
                if(s1 == 1) {
                    if(s2 == 1)
                        matrix[i - 3, i] = 2 * k;
                    else
                        matrix[i - 1, i] = 2 * k;
                }
                else {
                    if(s2 == 1)
                        matrix[i + 1, i] = 2 * k;
                    else
                        matrix[i + 3, i] = 2 * k;
                }
            }
        }

        /// <summary>
        /// Parciální stopa pøes lázeò
        /// </summary>
        /// <param name="n">Index vlastní hodnoty</param>
        public Matrix PartialTrace(int n) {
            LipkinOneOneBasisIndex index = this.eigenSystem.BasisIndex as LipkinOneOneBasisIndex;

            int dim = index.Length;
            Vector ev = this.eigenSystem.GetEigenVector(n);
            
            Matrix result = new Matrix(4);
            for(int i = 0; i < dim; i += 4)
                for(int j = 0; j < 4; j++)
                    for(int k = 0; k < 4; k++)
                        result[j, k] += ev[i + j] * ev[i + k];
 
            return result;
        }

        #region Implementace IExportable
        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinOneOne(Core.Import import) : base(import) { }
        #endregion
    }
}
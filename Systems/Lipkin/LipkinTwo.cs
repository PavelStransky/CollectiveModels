using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class LipkinTwo: LipkinFull, IQuantumSystem {
        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected LipkinTwo() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinTwo(double alpha, double omega)
            : base(alpha, omega) {
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvo�� instanci t��dy s parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public override BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LipkinTwoBasisIndex(basisParams);
        }

        /// <summary>
        /// Napln� Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        public override void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            LipkinTwoBasisIndex index = basisIndex as LipkinTwoBasisIndex;

            int dim = index.Length;
            int n1 = index.N1;
            int n2 = index.N2;

            int l1 = index.L1;
            int l2 = index.L2;

            double k = -(1.0 - this.alpha) / (n1 + n2);
            for(int i = 0; i < dim; i++) {
                int m1 = index.M1[i];
                int m2 = index.M2[i];

                double c1 = (n1 + n2 + m1 + m2) / 2.0;
                // 1 + 5
                matrix[i, i] = this.alpha * c1 + k * this.omega * this.omega * c1 * c1;

                int j = 0;

                // 2 S+                
                if((j = index[m1 + 4, m2]) >= 0)
                    matrix[j, i] = k * this.ShiftPlus(l1, m1) * this.ShiftPlus(l1, m1 + 2);
                if((j = index[m1 + 2, m2]) >= 0) {
                    matrix[i, i] += k * this.ShiftPlus(l1, m1) * this.ShiftMinus(l1, m1 + 2);
                    matrix[j, i] = k * this.omega * this.ShiftPlus(l1, m1) * (n1 + m1 + n2 + m2 + 1);
                }
                // 2 S-
                if((j = index[m1 - 4, m2]) >= 0)
                    matrix[j, i] = k * this.ShiftMinus(l1, m1) * this.ShiftMinus(l1, m1 - 2);
                if((j = index[m1 - 2, m2]) >= 0) {
                    matrix[i, i] += k * this.ShiftMinus(l1, m1) * this.ShiftPlus(l1, m1 - 2);
                    matrix[j, i] = k * this.omega * this.ShiftMinus(l1, m1) * (n1 + m1 + n2 + m2 - 1);
                }

                // 3 S+                
                if((j = index[m1, m2 + 4]) >= 0)
                    matrix[j, i] = k * this.ShiftPlus(l2, m2) * this.ShiftPlus(l2, m2 + 2);
                if((j = index[m1, m2 + 2]) >= 0) {
                    matrix[i, i] += k * this.ShiftPlus(l2, m2) * this.ShiftMinus(l2, m2 + 2);
                    matrix[j, i] = k * this.omega * this.ShiftPlus(l2, m2) * (n1 + m1 + n2 + m2 + 1);
                }
                // 3 S-
                if((j = index[m1, m2 - 4]) >= 0)
                    matrix[j, i] = k * this.ShiftMinus(l2, m2) * this.ShiftMinus(l2, m2 - 2);
                if((j = index[m1, m2 - 2]) >= 0) {
                    matrix[i, i] += k * this.ShiftMinus(l2, m2) * this.ShiftPlus(l2, m2 - 2);
                    matrix[j, i] = k * this.omega * this.ShiftMinus(l2, m2) * (n1 + m1 + n2 + m2 - 1);
                }

                if((j = index[m1 + 2, m2 + 2]) >= 0)
                    matrix[j, i] = 2 * k * this.ShiftPlus(l1, m1) * this.ShiftPlus(l2, m2);
                if((j = index[m1 + 2, m2 - 2]) >= 0)
                    matrix[j, i] = 2 * k * this.ShiftPlus(l1, m1) * this.ShiftMinus(l2, m2);
                if((j = index[m1 - 2, m2 + 2]) >= 0)
                    matrix[j, i] = 2 * k * this.ShiftMinus(l1, m1) * this.ShiftPlus(l2, m2);
                if((j = index[m1 - 2, m2 - 2]) >= 0)
                    matrix[j, i] = 2 * k * this.ShiftMinus(l1, m1) * this.ShiftMinus(l2, m2);
            }
        }

        /// <summary>
        /// Parci�ln� stopa p�es velk� prostor
        /// </summary>
        /// <param name="n">Index vlastn� hodnoty</param>
        /// <param name="type">Typ parci�ln� stopy</param>
        /// <returns>Matice hustoty jednotliv�ho spinu</returns>
        public Matrix PartialTrace(int n, int type) {
            LipkinTwoBasisIndex index = this.eigenSystem.BasisIndex as LipkinTwoBasisIndex;
            
            int dim = index.Length;
            int l1 = index.L1;
            int l2 = index.L2;

            Matrix result = new Matrix(l2 + 1);

            Vector ev = this.eigenSystem.GetEigenVector(n);

            for(int i = -l2; i <= l2; i += 2)
                for(int j = -l2; j <= l2; j += 2)
                    for(int m1 = -l1; m1 <= l1; m1 += 2)
                        result[(i + l2) / 2, (j + l2) / 2] += ev[index[m1, i]] * ev[index[m1, j]];

            return result;
        }


        #region Implementace IExportable
        /// <summary>
        /// Na�te v�sledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinTwo(Core.Import import) : base(import) { }
        #endregion
    }
}
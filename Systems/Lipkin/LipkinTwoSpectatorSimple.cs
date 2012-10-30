using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class LipkinTwoSpectatorSimple: LipkinFull, IQuantumSystem, IEntanglement {
        private double epsilon, g;

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LipkinTwoSpectatorSimple() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinTwoSpectatorSimple(double alpha, double omega, double epsilon, double g)
            : base(alpha, omega) {
            this.epsilon = epsilon;
            this.g = g;

            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvoøí instanci tøídy s parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public override BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LipkinTwoBasisIndex(basisParams);
        }

        /// <summary>
        /// Naplní Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public override void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            LipkinTwoBasisIndex index = basisIndex as LipkinTwoBasisIndex;

            int dim = index.Length;
            int n1 = index.N1;
            int n2 = index.N2;

            int l1 = index.L1;
            int l2 = index.L2;

            double k = n1 == 0 ? 0.0 : -(1.0 - this.alpha) / n1;

            for(int i = 0; i < dim; i++) {
                int m1 = index.M1[i];
                int m2 = index.M2[i];

                double c1 = (n1 + m1) / 2.0;
                double c1S = (n2 + m2) / 2.0;

                // 1 + 5
                matrix[i, i] = this.alpha * c1 + this.epsilon * c1S + k * this.omega * this.omega * c1 * c1;

                int j = 0;

                // 2 S+                
                if((j = index[m1 + 4, m2]) >= 0)
                    matrix[j, i] = k * this.ShiftPlus(l1, m1) * this.ShiftPlus(l1, m1 + 2);
                if((j = index[m1 + 2, m2]) >= 0) {
                    matrix[i, i] += k * this.ShiftPlus(l1, m1) * this.ShiftMinus(l1, m1 + 2);
                    matrix[j, i] = k * this.omega * this.ShiftPlus(l1, m1) * (n1 + m1 + 1);
                }
                // 2 S-
                if((j = index[m1 - 4, m2]) >= 0)
                    matrix[j, i] = k * this.ShiftMinus(l1, m1) * this.ShiftMinus(l1, m1 - 2);
                if((j = index[m1 - 2, m2]) >= 0) {
                    matrix[i, i] += k * this.ShiftMinus(l1, m1) * this.ShiftPlus(l1, m1 - 2);
                    matrix[j, i] = k * this.omega * this.ShiftMinus(l1, m1) * (n1 + m1 - 1);
                }

                if((j = index[m1 + 2, m2 + 2]) >= 0)
                    matrix[j, i] = this.g * this.ShiftPlus(l1, m1) * this.ShiftPlus(l2, m2);
                if((j = index[m1 + 2, m2 - 2]) >= 0)
                    matrix[j, i] = this.g * this.ShiftPlus(l1, m1) * this.ShiftMinus(l2, m2);
                if((j = index[m1 - 2, m2 + 2]) >= 0)
                    matrix[j, i] = this.g * this.ShiftMinus(l1, m1) * this.ShiftPlus(l2, m2);
                if((j = index[m1 - 2, m2 - 2]) >= 0)
                    matrix[j, i] = this.g * this.ShiftMinus(l1, m1) * this.ShiftMinus(l2, m2);
            }
        }

        /// <summary>
        /// Parciální stopa pøes první prostor
        /// </summary>
        /// <param name="n">Index vlastní hodnoty</param>
        /// <returns>Matice hustoty podsystému</returns>
        public Matrix PartialTrace(int n) {
            LipkinTwoBasisIndex index = this.eigenSystem.BasisIndex as LipkinTwoBasisIndex;

            int dim = index.Length;
            int l1 = index.L1;
            int l2 = index.L2;

            Vector ev = this.eigenSystem.GetEigenVector(n);

            Matrix result = new Matrix(l2 + 1);
            for(int i = -l2; i <= l2; i += 2)
                for(int j = -l2; j <= l2; j += 2)
                    for(int m1 = -l1; m1 <= l1; m1 += 2)
                        result[(i + l2) / 2, (j + l2) / 2] += ev[index[m1, i]] * ev[index[m1, j]];

            return result;
        }


        #region Implementace IExportable
        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinTwoSpectatorSimple(Core.Import import) : base(import) { }

        protected override void Export(IEParam param) {
            base.Export(param);

            param.Add(this.epsilon, "Epsilon");
            param.Add(this.g, "g");
        }

        protected override void Import(IEParam param) {
            base.Import(param);

            this.epsilon = (double)param.Get(0.0);
            this.g = (double)param.Get(0.0);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class LipkinFactorized: Lipkin, IQuantumSystem {
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LipkinFactorized() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinFactorized(double alpha, double omega)
            : base(alpha, omega) {
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvoøí instanci tøídy s parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LipkinFactorizedBasisIndex(basisParams);
        }

        /// <summary>
        /// Naplní Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            LipkinFactorizedBasisIndex index = basisIndex as LipkinFactorizedBasisIndex;

            int dim = index.Length;
            int n = index.N;

            double c = -(1.0 - this.alpha) / n;
            for(int i = 0; i < dim; i++) {
                Vector ind = index[i];

                matrix[i, i] = 0;

                // Term 1
                for(int j = 0; j < n; j++)
                    matrix[i, i] += this.alpha * ind[j];

                // Term 2
                double d = 0.0;
                //d += n * n / 4.0;
                //for(int j = 0; j < n; j++)
                //    d += n / 2.0 * jz[j];
                for(int j = 0; j < n; j++)
                    for(int k = 0; k < n; k++)
                        d += ind[j] * ind[k];
                matrix[i, i] += c * this.omega * this.omega * d;

                // Term 3, 4
                for(int j = 0; j < n; j++) {
                    if(ind[j] == 0) {
                        int i1 = i + (1 << j);
                        Vector ind1 = index[i1];

                        d = 0.0;
                        for(int k = 0; k < n; k++) {
                            d += ind[k] + ind1[k];
                        }

                        if(i > i1)
                            matrix[i1, i] += c * this.omega * d;
                    }
                    if(ind[j] == 1) {
                        int i1 = i - (1 << j);
                        Vector ind1 = index[i1];

                        d = 0.0;
                        for(int k = 0; k < n; k++)
                            d += ind[k] + ind1[k];

                        if(i > i1)
                            matrix[i1, i] += c * this.omega * d;
                    }
                }

                // Term 5,6,7,8
                for(int j = 0; j < n; j++) {
                    if(ind[j] == 0) {
                        int i1 = i + (1 << j);
                        for(int k = 0; k < n; k++) {
                            if(j != k && ind[k] == 0) {
                                int i2 = i1 + (1 << k);
                                if(i >= i2)
                                    matrix[i2, i] += c;
                            }
                            if(j == k || ind[k] == 1) {
                                int i2 = i1 - (1 << k);
                                if(i >= i2)
                                    matrix[i2, i] += c;
                            }
                        }
                    }
                    if(ind[j] == 1) {
                        int i1 = i - (1 << j);
                        for(int k = 0; k < n; k++) {
                            if(j != k && ind[k] == 1) {
                                int i2 = i1 - (1 << k);
                                if(i >= i2)
                                    matrix[i2, i] += c;
                            }
                            if(j == k || ind[k] == 0) {
                                int i2 = i1 + (1 << k);
                                if(i >= i2)
                                    matrix[i2, i] += c;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Vyjádøí vlastní stavy jako lineární kombinaci báze
        /// </summary>
        /// <param name="i">Index vlastního stavu</param>
        /// <param name="limit">Hodnota, která omezí prvky v rozvoji</param>
        public string ExpandBasis(int i, double limit) {
            LipkinFactorizedBasisIndex index = eigenSystem.BasisIndex as LipkinFactorizedBasisIndex;

            Vector ev = eigenSystem.GetEigenVector(i);
            int length = ev.Length;

            string[] ket = new string[length];
            double[] data = new double[length];
            int[] ind = new int[length];

            int limitd = limit > 0 ? (int)System.Math.Round(-System.Math.Log10(limit)) : 0;

            for(int j = 0; j < ev.Length; j++) {
                ket[j] = index.Ket(j);
                data[j] = System.Math.Abs(ev[j]);
                ind[j] = j;
            }

            Array.Sort((double[])data.Clone(), ind); Array.Reverse(ind);
            Array.Sort((double[])data.Clone(), ket); Array.Reverse(ket);
            Array.Sort(data); Array.Reverse(data);

            StringBuilder result = new StringBuilder();
            for(int j = 0; j < ev.Length; j++) {
                int k = ind[j];
                double d = data[j];
                if(d > limit) {
                    if(ev[k] >= 0)
                        result.Append("+");
                    else
                        result.Append("-");

                    if(limit > 0)
                        d = System.Math.Round(d, limitd);

                    result.Append(d);
                    result.Append(ket[j]);
                    result.Append(Environment.NewLine);
                }
            }

            return result.ToString();
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
        public LipkinFactorized(Core.Import import)
            : base(import) {
            this.eigenSystem.SetParrentQuantumSystem(this);
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class LipkinFullLinear : Lipkin, IQuantumSystem {
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LipkinFullLinear() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinFullLinear(double alpha, double omega)
            : base(alpha, omega) {
            this.eigenSystem = new EigenSystem(this);
        }

        public LipkinFullLinear(double alpha, double omega, double alphaIm, double omegaIm)
            : base(alpha, omega, alphaIm, omegaIm) {
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvoří instanci třídy s parametry báze
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

            if(matrix is CMatrix) {
                Complex k = new Complex(this.alpha, this.alphaIm) / n;
                Complex o = new Complex(this.omega, this.omegaIm);
                Complex g = k * o;
                for(int i = 0; i < dim; i++) {
                    int l = index.L[i];
                    int m = index.M[i];

                    double c1 = (n + m) / 2.0;
                    matrix[i, 2 * i] = c1;
                    matrix[i, 2 * i + 1] = 0;

                    // -1
                    if(i - 1 >= 0 && l == index.L[i - 1]) {
                        matrix[i, 2 * i] += k.Real * this.ShiftMinus(l, m) * this.ShiftPlus(l, m - 2);
                        matrix[i, 2 * i + 1] += k.Imaginary * this.ShiftMinus(l, m) * this.ShiftPlus(l, m - 2);
                        matrix[i - 1, 2 * i] = g.Real * this.ShiftMinus(l, m) * (n + m - 1);
                        matrix[i - 1, 2 * i + 1] = g.Imaginary * this.ShiftMinus(l, m) * (n + m - 1);
                    }
                    // +1
                    if(i + 1 < dim && l == index.L[i + 1]) {
                        matrix[i, 2 * i] += k.Real * this.ShiftPlus(l, m) * this.ShiftMinus(l, m + 2);
                        matrix[i, 2 * i + 1] += k.Imaginary * this.ShiftPlus(l, m) * this.ShiftMinus(l, m + 2);
                        matrix[i + 1, 2 * i] = g.Real * this.ShiftPlus(l, m) * (n + m + 1);
                        matrix[i + 1, 2 * i + 1] = g.Imaginary * this.ShiftPlus(l, m) * (n + m + 1);
                    }
                    // -2
                    if(i - 2 >= 0 && l == index.L[i - 2]) {
                        matrix[i - 2, 2 * i] = k.Real * this.ShiftMinus(l, m) * this.ShiftMinus(l, m - 2);
                        matrix[i - 2, 2 * i + 1] = k.Imaginary * this.ShiftMinus(l, m) * this.ShiftMinus(l, m - 2);
                    }
                    // +2
                    if(i + 2 < dim && l == index.L[i + 2]) {
                        matrix[i + 2, 2 * i] = k.Real * this.ShiftPlus(l, m) * this.ShiftPlus(l, m + 2);
                        matrix[i + 2, 2 * i + 1] = k.Imaginary * this.ShiftPlus(l, m) * this.ShiftPlus(l, m + 2);
                    }
                }
            }
            else {
                double k = this.alpha / n;
                for(int i = 0; i < dim; i++) {
                    int l = index.L[i];
                    int m = index.M[i];

                    double c1 = (n + m) / 2.0;
                    matrix[i, i] = c1; // 1

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
        }

        public Matrix[] ParameterMatrix(Vector basisParams) {
            LipkinFullBasisIndex index = this.CreateBasisIndex(basisParams) as LipkinFullBasisIndex;

            Matrix[] result = new Matrix[4];

            int dim = index.Length;
            int n = index.N;

            result[0] = new Matrix(dim);        // H0
            result[1] = new Matrix(dim);        // + omega H1

            double k = this.alpha / n;
            for(int i = 0; i < dim; i++) {
                int l = index.L[i];
                int m = index.M[i];

                double c1 = (n + m) / 2.0;
                result[0][i, i] = c1;

                // -1
                if(i - 1 >= 0 && l == index.L[i - 1]) {
                    result[0][i, i] += k * this.ShiftMinus(l, m) * this.ShiftPlus(l, m - 2);
                    result[1][i - 1, i] = k * this.ShiftMinus(l, m) * (n + m - 1);
                }
                // +1
                if(i + 1 < dim && l == index.L[i + 1]) {
                    result[0][i, i] += k * this.ShiftPlus(l, m) * this.ShiftMinus(l, m + 2);
                    result[1][i + 1, i] = k * this.ShiftPlus(l, m) * (n + m + 1);
                }
                // -2
                if(i - 2 >= 0 && l == index.L[i - 2])
                    result[0][i - 2, i] = k * this.ShiftMinus(l, m) * this.ShiftMinus(l, m - 2);
                // +2
                if(i + 2 < dim && l == index.L[i + 2])
                    result[0][i + 2, i] = k * this.ShiftPlus(l, m) * this.ShiftPlus(l, m + 2);
            }

            // Part for alpha as variable;
            result[2] = new Matrix(dim);        // H0'
            result[3] = new Matrix(dim);        // + alpha H1

            double k3 = 1.0 / n;
            for(int i = 0; i < dim; i++) {
                int l = index.L[i];
                int m = index.M[i];

                double c1 = (n + m) / 2.0;
                result[2][i, i] = c1;
                result[3][i, i] = 0;

                // -1
                if(i - 1 >= 0 && l == index.L[i - 1]) {
                    result[3][i, i] += k3 * this.ShiftMinus(l, m) * this.ShiftPlus(l, m - 2);
                    result[3][i - 1, i] = k3 * this.omega * this.ShiftMinus(l, m) * (n + m - 1);
                }
                // +1
                if(i + 1 < dim && l == index.L[i + 1]) {
                    result[3][i, i] += k3 * this.ShiftPlus(l, m) * this.ShiftMinus(l, m + 2);
                    result[3][i + 1, i] = k3 * this.omega * this.ShiftPlus(l, m) * (n + m + 1);
                }
                // -2
                if(i - 2 >= 0 && l == index.L[i - 2]) {
                    result[3][i - 2, i] = k3 * this.ShiftMinus(l, m) * this.ShiftMinus(l, m - 2);
                }
                // +2
                if(i + 2 < dim && l == index.L[i + 2]) {
                    result[3][i + 2, i] = k3 * this.ShiftPlus(l, m) * this.ShiftPlus(l, m + 2);
                }
            }

            return result;
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
        /// Načte výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinFullLinear(Core.Import import)
            : base(import) {
            this.eigenSystem.SetParrentQuantumSystem(this);
        }
        #endregion
    }
}
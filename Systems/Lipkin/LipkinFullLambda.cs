using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Lipkin model according to the paper PRE 2017
    /// </summary>
    public class LipkinFullLambda : Lipkin, IQuantumSystem {
        private bool isLinear = false;

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LipkinFullLambda() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public LipkinFullLambda(double alpha, double omega, bool isLinear)
            : base(alpha, omega) {
            this.isLinear = isLinear;
            this.eigenSystem = new EigenSystem(this);
        }

        public LipkinFullLambda(double alpha, double omega, double alphaIm, double omegaIm, bool isLinear)
            : base(alpha, omega, alphaIm, omegaIm) {
            this.isLinear = isLinear;
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
                Complex k = new Complex(-this.alpha, -this.alphaIm) / n;
                Complex o = new Complex(this.omega, this.omegaIm);
                Complex h = this.isLinear ? Complex.Zero : k * o * o;
                Complex g = k * o;
                for(int i = 0; i < dim; i++) {
                    int l = index.L[i];
                    int m = index.M[i];

                    double c1 = (n + m) / 2.0;
                    matrix[i, 2 * i] = c1 + h.Real * c1 * c1;
                    matrix[i, 2 * i + 1] = h.Imaginary * c1 * c1;

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
                double k = -this.alpha / n;
                for(int i = 0; i < dim; i++) {
                    int l = index.L[i];
                    int m = index.M[i];

                    double c1 = (n + m) / 2.0;
                    matrix[i, i] = c1 + (this.isLinear ? 0.0 : k * this.omega * this.omega * c1 * c1); // 1

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

            Matrix[] result = new Matrix[5];

            int dim = index.Length;
            int n = index.N;

            result[0] = new Matrix(dim);        // H0
            result[1] = new Matrix(dim);        // + omega H1
            result[2] = new Matrix(dim);        // + omega^2 H2

            double k = -this.alpha / n;
            for(int i = 0; i < dim; i++) {
                int l = index.L[i];
                int m = index.M[i];

                double c1 = (n + m) / 2.0;
                result[0][i, i] = c1;
                result[2][i, i] = k * c1 * c1;

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
            result[3] = new Matrix(dim);        // H0'
            result[4] = new Matrix(dim);        // + alpha H1

            double k3 = -1.0 / n;
            for(int i = 0; i < dim; i++) {
                int l = index.L[i];
                int m = index.M[i];

                double c1 = (n + m) / 2.0;
                result[3][i, i] = c1;
                result[4][i, i] = this.isLinear ? 0 : k3 * this.omega * this.omega * c1 * c1;

                // -1
                if(i - 1 >= 0 && l == index.L[i - 1]) {
                    result[4][i, i] += k3 * this.ShiftMinus(l, m) * this.ShiftPlus(l, m - 2);
                    result[4][i - 1, i] = k3 * this.omega * this.ShiftMinus(l, m) * (n + m - 1);
                }
                // +1
                if(i + 1 < dim && l == index.L[i + 1]) {
                    result[4][i, i] += k3 * this.ShiftPlus(l, m) * this.ShiftMinus(l, m + 2);
                    result[4][i + 1, i] = k3 * this.omega * this.ShiftPlus(l, m) * (n + m + 1);
                }
                // -2
                if(i - 2 >= 0 && l == index.L[i - 2]) {
                    result[4][i - 2, i] = k3 * this.ShiftMinus(l, m) * this.ShiftMinus(l, m - 2);
                }
                // +2
                if(i + 2 < dim && l == index.L[i + 2]) {
                    result[4][i + 2, i] = k3 * this.ShiftPlus(l, m) * this.ShiftPlus(l, m + 2);
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
        /// <param name="type">Typ (0 - H0, 1 - L1, 2 - L2, 3 - l3)</param>
        public Vector PeresInvariant(int type) {
            LipkinFullBasisIndex index = this.eigenSystem.BasisIndex as LipkinFullBasisIndex;

            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            switch (type) {
                case 0: {
                        Vector e = this.eigenSystem.GetEigenValues() as Vector;
                        for (int i = 0; i < count; i++) {
                            Vector ev = this.eigenSystem.GetEigenVector(i);
                            int length = ev.Length;

                            result[i] = e[i] - 0.5 * index.N;

                            for (int j = 0; j < length; j++)
                                result[i] -= 0.5 * ev[j] * ev[j] * index.M[j];
                        }
                        break;
                    }
                case 1: {
                        for (int i = 0; i < count; i++) {
                            Vector ev = this.eigenSystem.GetEigenVector(i);
                            int length = ev.Length;

                            for (int j = 0; j < length; j++) {
                                if (j > 0)
                                    result[i] += 0.5 * ev[j] * ev[j - 1] * this.ShiftPlus(index.L[j], index.M[j] - 2);
                                if (j < length - 1)
                                    result[i] += 0.5 * ev[j] * ev[j + 1] * this.ShiftMinus(index.L[j], index.M[j] + 2);
                            }
                        }
                        break;
                    }
                case 2: {
                        for (int i = 0; i < count; i++) {
                            Vector ev = this.eigenSystem.GetEigenVector(i);
                            int length = ev.Length;

                            for (int j = 0; j < length; j++) {
                                if (j > 0)
                                    result[i] += 0.5 * ev[j] * ev[j - 1] * this.ShiftPlus(index.L[j], index.M[j] - 2);
                                if (j < length - 1)
                                    result[i] -= 0.5 * ev[j] * ev[j + 1] * this.ShiftMinus(index.L[j], index.M[j] + 2);
                            }
                        }
                        break;
                    }
                case 3: {
                        for (int i = 0; i < count; i++) {
                            Vector ev = this.eigenSystem.GetEigenVector(i);
                            int length = ev.Length;

                            for (int j = 0; j < length; j++)
                                result[i] += 0.5 * ev[j] * ev[j] * index.M[j];

                        }
                        break;
                    }
                case 4: {
                        for (int i = 0; i < count; i++) {
                            Vector ev = this.eigenSystem.GetEigenVector(i);
                            int length = ev.Length;

                            for (int j = 0; j < length; j++)
                                result[i] += 0.25 * ev[j] * ev[j] * index.M[j] * index.M[j];

                        }
                        break;
                    }
            }

            return result;
        }

        public double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] x) {
            throw new NotImpException(this, "ProbabilityAmplitude");
        }

        /// <summary>
        /// Vrátí matici <n|V|n> amplitudy vlastní funkce n
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="rx">Rozměry ve směru x</param>
        /// <param name="ry">Rozměry ve směru y</param>
        public virtual Matrix[] AmplitudeMatrix(int[] n, IOutputWriter writer, DiscreteInterval intx, DiscreteInterval inty) {
            int numx = intx.Num;
            int numy = inty.Num;

            int numn = n.Length;

            // Reálná a imaginární část (proto 2 * numn)
            Matrix[] result = new Matrix[2 * numn];
            for (int i = 0; i < 2 * numn; i++) {
                result[i] = new Matrix(numx, numy);
            }

            int length = this.eigenSystem.BasisIndex.Length;
            int length100 = System.Math.Max(length / 100, 1);

            DateTime startTime = DateTime.Now;

            for (int k = 0; k < length; k++) {
                BasisCache2D cache = new BasisCache2D(intx, inty, k, this.Psi);
                BasisCache2D cacheR = new BasisCache2D(intx, inty, k, this.PsiR);
                BasisCache2D cacheI = new BasisCache2D(intx, inty, k, this.PsiI);

                for (int l = 0; l < numn; l++) {
                    Vector ev = this.eigenSystem.GetEigenVector(n[l]);

                    for (int i = 0; i < numx; i++)
                        for (int j = 0; j < numy; j++) {
                            result[l][i, j] += ev[k] * cacheR[i, j] * cache[i, j];
                            result[l + numn][i, j] += ev[k] * cacheI[i, j] * cache[i, j];
                        }
                }

                if (writer != null)
                    if ((k + 1) % length100 == 0) {
                        writer.Write('.');

                        if (((k + 1) / length100) % 10 == 0) {
                            writer.Write((k + 1) / length100);
                            writer.Write("% ");
                            writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));
                            startTime = DateTime.Now;
                        }
                    }
            }

            return result;
        }

        /// <summary>
        /// Vlnová funkce x - HO
        /// </summary>
        /// <param name="x">Souřadnice x</param>
        /// <param name="phi">Souřadnice phi</param>
        /// <param name="j">Index vlnové funkce</param>
        private double Psi(double x, double phi, int j) {
            LipkinFullBasisIndex index = this.eigenSystem.BasisIndex as LipkinFullBasisIndex;
            int m = System.Math.Abs(index.M[j]);
            int l = index.L[j];

            double coef = System.Math.Log((2 * l + 1) / (4 * System.Math.PI)) + SpecialFunctions.FactorialILog(l - m) - SpecialFunctions.FactorialILog(l + m);

            double r = SpecialFunctions.Legendre(System.Math.Cos(x), l, m);

            if (r == 0.0)
                return 0.0;

            double result = 0.5 * coef + System.Math.Log(System.Math.Abs(r));
            
            
            int sign = System.Math.Sign(r);
            if (index.M[j] < 0 && (m % 4) > 0)
                sign = -sign;
            return sign * System.Math.Exp(result);
        }

        /// <summary>
        /// Vlnová funkce phi - rotor
        /// </summary>
        /// <param name="x">Souřadnice x</param>
        /// <param name="phi">Souřadnice phi</param>
        /// <param name="j">Index vlnové funkce</param>
        private double PsiR(double x, double phi, int j) {
            LipkinFullBasisIndex index = this.eigenSystem.BasisIndex as LipkinFullBasisIndex;
            int m = index.M[j];
            return System.Math.Cos(0.5 * m * phi);
        }

        /// <summary>
        /// Vlnová funkce phi - rotor
        /// </summary>
        /// <param name="x">Souřadnice x</param>
        /// <param name="phi">Souřadnice phi</param>
        /// <param name="j">Index vlnové funkce</param>
        private double PsiI(double x, double phi, int j) {
            LipkinFullBasisIndex index = this.eigenSystem.BasisIndex as LipkinFullBasisIndex;
            int m = index.M[j];
            return System.Math.Sin(0.5 * m * phi);
        }

        /// <summary>
        /// Vrátí matici hustot pro vlastní funkce
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="interval">Rozměry v jednotlivých směrech (uspořádané ve tvaru [minx, maxx,] numx, ...)</param>
        public object ProbabilityDensity(int[] n, IOutputWriter writer, params Vector[] interval) {
            DiscreteInterval intx = new DiscreteInterval(interval[0]);
            DiscreteInterval inty = new DiscreteInterval(interval[1]);

            Matrix[] amplitude = this.AmplitudeMatrix(n, writer, intx, inty);

            int numn = amplitude.Length / 2;
            int numx = amplitude[0].LengthX;
            int numy = amplitude[0].LengthY;

            Matrix[] m = new Matrix[numn];
            Vector[] v = new Vector[numn];

            for (int l = 0; l < numn; l++) {
                m[l] = new Matrix(numx, numy);
                v[l] = new Vector(numx);

                for (int i = 0; i < numx; i++) {
                    Complex x = 0;
                    for (int j = 0; j < numy; j++) {
                        m[l][i, j] = amplitude[l][i, j] * amplitude[l][i, j] + amplitude[l + numn][i, j] * amplitude[l + numn][i, j];
                        x += new Complex(amplitude[l][i, j], amplitude[l + numn][i, j]);
                    }
                    v[l][i] = x.Magnitude;
                }
            }

            return m;
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        protected override void Export(IEParam param) {
            base.Export(param);
            param.Add(this.isLinear, "IsLinear");
        }

        protected override void Import(IEParam param) {            
            base.Import(param);
            this.isLinear = (bool)param.Get(false);
        }

        /// <summary>
        /// Načte výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinFullLambda(Core.Import import)
            : base(import) {
            this.eigenSystem.SetParrentQuantumSystem(this);
        }
        #endregion
    }
}
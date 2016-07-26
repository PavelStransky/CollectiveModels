﻿using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    public class QuantumDicke : Dicke, IQuantumSystem, IEntanglement {
        private int type;
        /// <summary>
        /// Konstruktor
        /// </summary>
        public QuantumDicke(double omega0, double omega, double gamma, double j, double delta, int type)
            : base(omega0, omega, gamma, j, delta) {
            this.type = type;
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        protected QuantumDicke() { }

        #region Implementace IQuantumSystem
        // Systém s vlastními hodnotami
        protected EigenSystem eigenSystem;

        /// <summary>
        /// Systém vlastních hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Druhý invariant
        /// </summary>
        /// <param name="type">Typ Peresova operátoru</param>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        public Vector PeresInvariant(int type) {
            return null;
        }

        /// <summary>
        /// Matice s hodnotami vlastních čísel seřazené podle kvantových čísel
        /// </summary>
        /// <param name="n">Pořadí vlastní hodnoty</param>
        public Matrix EigenMatrix(int n) {
            int num = this.eigenSystem.BasisIndex.Length;

            int maxq1 = this.eigenSystem.BasisIndex.BasisQuantumNumberLength(0);
            int maxq2 = this.eigenSystem.BasisIndex.BasisQuantumNumberLength(1);

            Matrix result = new Matrix(maxq1, maxq2);

            for(int i = 0; i < num; i++) {
                int q1 = this.eigenSystem.BasisIndex.GetBasisQuantumNumber(0, i);
                int q2 = this.eigenSystem.BasisIndex.GetBasisQuantumNumber(1, i);

                result[q1, q2] = this.eigenSystem.GetEigenVector(n)[i];
            }

            return result;
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
            for(int i = 0; i < 2 * numn; i++) {
                result[i] = new Matrix(numx, numy);
            }

            int length = this.eigenSystem.BasisIndex.Length;
            int length100 = length / 100;

            DateTime startTime = DateTime.Now;

            double coef = System.Math.Pow(this.Omega / System.Math.PI, 0.25) / (System.Math.Sqrt(2.0 * System.Math.PI));

            for(int k = 0; k < length; k++) {
                BasisCache2D cache = new BasisCache2D(intx, inty, k, this.Psi);
                BasisCache2D cacheR = new BasisCache2D(intx, inty, k, this.PsiR);
                BasisCache2D cacheI = new BasisCache2D(intx, inty, k, this.PsiI);

                for(int l = 0; l < numn; l++) {
                    Vector ev = this.eigenSystem.GetEigenVector(n[l]);

                    for(int i = 0; i < numx; i++)
                        for(int j = 0; j < numy; j++) {
                            result[l][i, j] += coef * ev[k] * cacheR[i, j] * cache[i, j];
                            result[l + numn][i, j] += coef * ev[k] * cacheI[i, j] * cache[i, j];
                        }
                }

                if(writer != null)
                    if((k + 1) % length100 == 0) {
                        writer.Write('.');

                        if(((k + 1) / length100) % 10 == 0) {
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
            DickeBasisIndex index = this.eigenSystem.BasisIndex as DickeBasisIndex;
            int n = index.N[j];

            double coef = -0.5 * (n * System.Math.Log(2.0) + SpecialFunctions.FactorialILog(n));

            double r = 0.0;
            double e = 0.0;
            SpecialFunctions.Hermite(out r, out e, System.Math.Sqrt(this.Omega) * x, n);


            if(r == 0.0)
                return 0.0;

            double result = coef + System.Math.Log(System.Math.Abs(r)) - 0.5 * this.Omega * x * x + e;

            return r > 0 ? System.Math.Exp(result) : -System.Math.Exp(result);
        }

        /// <summary>
        /// Vlnová funkce phi - rotor
        /// </summary>
        /// <param name="x">Souřadnice x</param>
        /// <param name="phi">Souřadnice phi</param>
        /// <param name="j">Index vlnové funkce</param>
        private double PsiR(double x, double phi, int j) {
            DickeBasisIndex index = this.eigenSystem.BasisIndex as DickeBasisIndex;
            int m = index.M[j];
            return System.Math.Cos(m * phi);
        }

        /// <summary>
        /// Vlnová funkce phi - rotor
        /// </summary>
        /// <param name="x">Souřadnice x</param>
        /// <param name="phi">Souřadnice phi</param>
        /// <param name="j">Index vlnové funkce</param>
        private double PsiI(double x, double phi, int j) {
            DickeBasisIndex index = this.eigenSystem.BasisIndex as DickeBasisIndex;
            int m = index.M[j];
            return System.Math.Sin(m * phi);
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

            Matrix[] result = new Matrix[numn];

            for(int l = 0; l < numn; l++) {
                result[l] = new Matrix(numx, numy);

                for(int i = 0; i < numx; i++)
                    for(int j = 0; j < numy; j++)
                        result[l][i, j] = amplitude[l][i, j] * amplitude[l][i, j] + amplitude[l + numn][i, j] * amplitude[l + numn][i, j];
            }

            return result;
        }

        /// <summary>
        /// Vrátí hustotu vlnové funkce v daném bodě
        /// </summary>
        /// <param name="n">Index vlastní funkce</param>
        /// <param name="x">Bod</param>
        public double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] x) {
            return 0;
        }

        /// <summary>
        /// Vytvoří instanci třídy LHOPolarIndex
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public BasisIndex CreateBasisIndex(Vector basisParams) {
            basisParams.Length = 3;
            basisParams[1] = this.J;
            basisParams[2] = this.type;
            return new DickeBasisIndex(basisParams);
        }

        /// <summary>
        /// Napočítá Hamiltonovu matici v dané bázi
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="basisIndex">Parametry báze</param>
        public void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            DickeBasisIndex index = basisIndex as DickeBasisIndex;

            int length = index.Length;
            int bandWidth = index.BandWidth;
            int j = index.J;

            double gamman = this.Gamma / System.Math.Sqrt(2 * j);

            DateTime startTime = DateTime.Now;

            if(writer != null)
                writer.Write(string.Format("Příprava H ({0} x {1})...", length, length));

            for(int i = 0; i < length; i++) {
                int n = index.N[i];
                int m = index.M[i];

                int i1 = index[n + 1, m - 1];
                int i2 = index[n - 1, m + 1];
                int i3 = index[n + 1, m + 1];
                int i4 = index[n - 1, m - 1];

                matrix[i, i] = this.Omega * n + this.Omega0 * m;

                if(i1 >= 0)
                    matrix[i1, i] = gamman * this.ShiftMinus(j, m) * System.Math.Sqrt(n + 1);
                if(i2 >= 0)
                    matrix[i2, i] = gamman * this.ShiftPlus(j, m) * System.Math.Sqrt(n);
                if(i3 >= 0)
                    matrix[i3, i] = gamman * this.Delta * this.ShiftPlus(j, m) * System.Math.Sqrt(n + 1);
                if(i4 >= 0)
                    matrix[i4, i] = gamman * this.Delta * this.ShiftMinus(j, m) * System.Math.Sqrt(n);

                /*
        // Výběrové pravidlo
            if(nj == ni && mj == mi) {
                sum += this.Omega * ni;
                sum += this.Omega0 * mi;
            }

            if(mi + 1 == mj && ni - 1 == nj)
                sum += gamman * this.ShiftPlus(index.J, mj) * System.Math.Sqrt(nj);

            if(mi - 1 == mj && ni + 1 == nj)
                sum += gamman * this.ShiftMinus(index.J, mj) * System.Math.Sqrt(nj + 1);

            if(mi + 1 == mj && ni + 1 == nj)
                sum += gamman * this.Delta * this.ShiftPlus(index.J, mj) * System.Math.Sqrt(nj + 1);

            if(mi - 1 == mj && ni - 1 == nj)
                sum += gamman * this.Delta * this.ShiftMinus(index.J, mj) * System.Math.Sqrt(nj);

            if(sum != 0.0) {
                matrix[i, j] = sum;
                matrix[j, i] = sum;
            }
        }
                 **/
            }

            if(writer != null)
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime));
        }

        protected double ShiftPlus(int l, int m) {
            if(m > l || m < -l)
                return 0;
            return System.Math.Sqrt((l - m) * (l + m + 1));
        }

        protected double ShiftMinus(int l, int m) {
            if(m > l || m < -l)
                return 0;
            return System.Math.Sqrt((l + m) * (l - m + 1));
        }
        #endregion

        /// <summary>
        /// Parciální stopa přes první prostor
        /// </summary>
        /// <param name="n">Index vlastní hodnoty</param>
        /// <returns>Matice hustoty podsystému</returns>
        public Matrix PartialTrace(int n) {
            DickeBasisIndex index = this.eigenSystem.BasisIndex as DickeBasisIndex;

            int dim = index.Length;
            int m = index.MaxN;
            int j = index.J;

            Vector ev = this.eigenSystem.GetEigenVector(n);

            Matrix result = new Matrix(2 * j + 1);
            for(int i = -j; i <= j; i++)
                for(int k = -j; k <= j; k++)
                    for(int l = 0; l <= m; l++)
                        if(index[l, i] >= 0 && index[l, k] >= 0) {
                            result[i + j, k + j] += ev[index[l, i]] * ev[index[l, k]];
                        }

            /*
            Matrix result = new Matrix(m + 1);
            for(int i = 0; i <= m; i++)
                for(int k = 0; k <= m; k++)
                    for(int l = -j; l <= j; l++)
                        result[i, k] += ev[index[i, l]] * ev[index[k, l]];
            */
            return result;
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public override void Export(Export export) {
            base.Export(export);

            IEParam param = new IEParam();
            param.Add(this.eigenSystem, "EigenSystem");
            param.Add(this.type, "Type");
            param.Export(export);
        }

        /// <summary>
        /// Načte výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public QuantumDicke(Core.Import import)
            : base(import) {
            IEParam param = new IEParam(import);
            this.eigenSystem = (EigenSystem)param.Get();
            this.type = (int)param.Get(0);
            this.eigenSystem.SetParrentQuantumSystem(this);
        }
        #endregion
    }
}
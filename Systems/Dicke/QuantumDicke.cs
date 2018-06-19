using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    public class QuantumDicke : Dicke, IQuantumSystem, IEntanglement {
        private int type;
        private Matrix qmn = null;
        private Matrix pmn = null;

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
        /// <param name="type">Typ Peresova operátoru:
        /// 0 ... N
        /// 1 ... M
        /// 2 ... N + M
        /// </param>
        /// <remarks>L. E. Reichl, 5.4 Time Average as an Invariant</remarks>
        public Vector PeresInvariant(int type) {
            DickeBasisIndex index = this.eigenSystem.BasisIndex as DickeBasisIndex;

            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            for (int i = 0; i < count; i++) {
                Vector ev = this.eigenSystem.GetEigenVector(i);
                int length = ev.Length;

                for (int j = 0; j < length; j++) {
                    double koef = 0.0;

                    switch (type) {
                        case 0:
                            koef = index.N[j];
                            break;
                        case 1:
                            koef = 0.5 * index.M[j];
                            break;
                        case 2:
                            koef = 0.5 * index.M[j] + index.N[j];
                            break;
                    }

                    result[i] += ev[j] * ev[j] * koef;
                }
            }

            return result;
        }

        /// <summary>
        /// Matice s hodnotami vlastních vektorů seřazených podle kvantových čísel
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
            int length100 = System.Math.Max(length / 100, 1);

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
            return System.Math.Cos(0.5 * m * phi);
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

            double gamman = this.Gamma / System.Math.Sqrt(j);

            DateTime startTime = DateTime.Now;

            if(writer != null)
                writer.Write(string.Format("Příprava H ({0} x {1})...", length, length));

            for(int i = 0; i < length; i++) {
                int n = index.N[i];
                int m = index.M[i];

                int i1 = index[n + 1, m - 2];
                int i2 = index[n - 1, m + 2];
                int i3 = index[n + 1, m + 2];
                int i4 = index[n - 1, m - 2];

                matrix[i, i] = this.Omega * n + this.Omega0 * m / 2.0;

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

        /// <remarks>
        /// Pozor, počítá pro l dvojnásobné (abychom zvládli i poločíselné spiny)
        /// </remarks>
        protected double ShiftPlus(int l, int m) {
            if(m > l || m < -l)
                return 0;
            return 0.5 * System.Math.Sqrt((l - m) * (l + m + 2));
        }

        /// <remarks>
        /// Pozor, počítá pro l dvojnásobné (abychom zvládli i poločíselné spiny)
        /// </remarks>
        protected double ShiftMinus(int l, int m) {
            if(m > l || m < -l)
                return 0;
            return 0.5 * System.Math.Sqrt((l + m) * (l - m + 2));
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

            Matrix result = new Matrix(j + 1);
            for (int i = -j; i <= j; i += 2)
                for (int k = -j; k <= j; k += 2)
                    for (int l = 0; l <= m; l++)
                        if (index[l, i] >= 0 && index[l, k] >= 0) {
                            result[(i + j) / 2, (k + j) / 2] += ev[index[l, i]] * ev[index[l, k]];
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

        public Matrix ExpectationValuePositionOperator(IOutputWriter writer) {
            this.ExpectationValuePQOperators(writer);
            return this.pmn;
        }

        /// <summary>
        /// Střední hodnota operátorů
        /// q = (a+ + a) / (sqrt(2))
        /// p = (a+ - a) / (i sqrt(2))
        /// </summary>
        private void ExpectationValuePQOperators(IOutputWriter writer) {
            if(this.qmn != null & this.pmn != null)
                return;

            if(writer != null)
                writer.Write("QPmn");

            DickeBasisIndex index = this.eigenSystem.BasisIndex as DickeBasisIndex;

            int count = this.eigenSystem.NumEV;
            int length = index.Length;

            this.pmn = new Matrix(count);
            this.qmn = new Matrix(count);

            double c = 1.0 / System.Math.Sqrt(2);

            for(int i = 0; i < count; i++) {
                if(writer != null && i % 100 == 0)
                    writer.Write(".");
                Vector ev1 = this.eigenSystem.GetEigenVector(i);
                for(int j = i; j < count; j++) {
                    Vector ev2 = this.eigenSystem.GetEigenVector(j);
                    double q = 0.0;
                    double p = 0.0;
                    for(int k = 0; k < length; k++) {
                        int n = index.N[k];
                        int m = index.M[k];

                        int k1 = index[n - 1, m];
                        int k2 = index[n + 1, m];

                        if(k1 >= 0) {
                            double x = ev1[k] * System.Math.Sqrt(n) * ev2[k1];
                            q += x;
                            p -= x;
                        }
                        if(k2 >= 0) {
                            double x = ev1[k] * System.Math.Sqrt(n + 1) * ev2[k2];
                            q += x;
                            p += x;
                        }
                    }

                    this.qmn[i, j] = c * q;
                    this.qmn[j, i] = c * q;
                    this.pmn[i, j] = c * p;
                    this.pmn[j, i] = -c * p;
                }
            }
        }

        /// <summary>
        /// Mikrokanonický OTOC pro stav s a časy t
        /// </summary>
        /// <param name="s">Stav</param>
        /// <param name="time">Časy</param>
        /// <param name="precision">Přesnost</param>
        public Vector OTOC(int s, Vector time, double precision, IOutputWriter writer) {
            if(writer != null)
                writer.Write(string.Format("{0}...", s));

            this.ExpectationValuePQOperators(writer);

            int count = this.qmn.Length;

            int[] limit = new int[count];
            for(int i = 0; i < count; i++)
                for(int j = i; j < count; j++)
                    if(this.qmn[i, j] > precision || this.pmn[i, j] > precision)
                        limit[i] = j - i;

            // Iterations
            int delta = limit[s];
            int delta0 = -1;
            while(delta0 != delta) {
                delta0 = delta;
                for(int j = System.Math.Max(0, s - delta0); j < System.Math.Min(count, s + delta0 + 1); j++)
                    delta = System.Math.Max(delta, limit[j]);
            }

            int imin1 = System.Math.Max(0, s - delta);
            int imax1 = System.Math.Min(count, s + delta + 1);

            int imin2 = System.Math.Max(0, s - 2 * delta);
            int imax2 = System.Math.Min(count, s + 2 * delta + 1);

            if(writer != null)
                writer.Write(string.Format("({0},{1})", count, delta));

            int length1 = imax1 - imin1;
            int length2 = imax2 - imin2;

            Vector ev = this.eigenSystem.GetEigenValues() as Vector;

            int tl = time.Length;
            Vector result = new Vector(tl);

            int li = 0;
            for(int i = imin2; i < imax2; i++) {

                if(100 * (i - imin2) / length2 > li) {
                    if(writer != null)
                        writer.Write('.');
                    li = 100 * (i - imin2) / length2;
                }

                Vector dr = new Vector(tl);
                Vector di = new Vector(tl);
                for(int j = imin1; j < imax1; j++) {
                    double a = this.qmn[s, j] * this.pmn[j, i];
                    double b = this.pmn[s, j] * this.qmn[j, i];
                    double de1 = ev[s] - ev[j];
                    double de2 = ev[j] - ev[i];
                    for(int k = 0; k < tl; k++) {
                        double t = time[k];
                        dr[k] += a * System.Math.Cos(de1 * t) - b * System.Math.Cos(de2 * t);
                        di[k] += a * System.Math.Sin(de1 * t) - b * System.Math.Sin(de2 * t);
                    }
                }

                for(int k = 0; k < tl; k++)
                    result[k] += dr[k] * dr[k] + di[k] * di[k];
            }

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
            param.Add(this.qmn, "Qmn");
            param.Add(this.pmn, "Pmn");
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
            this.qmn = (Matrix)param.Get();
            this.pmn = (Matrix)param.Get();
            this.eigenSystem.SetParrentQuantumSystem(this);
        }
        #endregion
    }
}
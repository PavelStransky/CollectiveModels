using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// V(x) = (a + b x + c x^2 + d x^3)exp(-r^2/10)
    /// </summary>
    public class ComplexScatteringExplicit : IQuantumSystem, IExportable {
        // Systém s vlastními hodnotami
        private EigenSystem eigenSystem;

        private double a, b, c, d;     // Parametry potenciálu
        private double theta;          // Komplexní škálování

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected ComplexScatteringExplicit() { }

        /// <summary>
        /// Systém vlastních hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComplexScatteringExplicit(double theta, double a, double b, double c, double d) {
            this.eigenSystem = new EigenSystem(this);
            this.theta = theta;
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        /// <summary>
        /// Vytvoří instanci třídy s parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public virtual BasisIndex CreateBasisIndex(Vector basisParams) {
            return new ComplexScatteringBasisIndex(basisParams);
        }

        public Complex V(Complex r) {
            return (this.a + r * (this.b + r * (this.c + r * this.d))) * Complex.Exp(-0.1 * r * r);
        }

        /// <summary>
        /// Naplní Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public virtual void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            ComplexScatteringBasisIndex index = basisIndex as ComplexScatteringBasisIndex;

            int dim = index.Length;

            double l = index.L;
            double hbar = index.Hbar;

            DateTime timeStart = DateTime.Now;
            int step = dim / 100;
            int numElements = dim * (dim + 1) / 2;

            int k = 0;

            Complex a = Complex.Exp(new Complex(0, -this.theta));
            Complex b = -5.0 * System.Math.PI / l * Complex.Exp(new Complex(0, -2.0 * this.theta));
            Complex c = 5.0 / (l * l) * Complex.Exp(new Complex(0, -3.0 * this.theta));
            Complex d = -25.0 * System.Math.PI / (l * l * l) * Complex.Exp(new Complex(0, -4.0 * this.theta));

            Complex e = l * l * Complex.Exp(new Complex(0, 2.0 * this.theta));
            Complex f = -5.0 * System.Math.PI * System.Math.PI;

            double x = 2.0 / l * System.Math.Sqrt(2.5 * System.Math.PI);
            Complex y = -2.5 * (System.Math.PI * System.Math.PI) / (l * l) * Complex.Exp(new Complex(0, -2.0 * this.theta));
            Complex z = -4.0 * y;

            double pi2 = 0.5 * System.Math.PI;

            for (int i = 0; i < dim; i++) {
                int m = i + 1;

                // Kinetic term
                Complex t = hbar * (i + 1) * System.Math.PI / l;
                t = 0.5 * t * t * Complex.Exp(new Complex(0, -2.0 * this.theta));

                for (int j = i; j < dim; j++) {
                    int n = j + 1;

                    int mnp = m + n;
                    int mnm = m - n;

                    Complex alpha = x * Complex.Exp(mnp * mnp * y);
                    Complex beta = Complex.Exp(m * n * z);

                    double cm = System.Math.Cos(pi2 * mnm);
                    double cp = System.Math.Cos(pi2 * mnp);

                    double sm = System.Math.Sin(pi2 * mnm);
                    double sp = System.Math.Sin(pi2 * mnp);

                    Complex m0 = alpha * a * (beta * cm - cp);
                    Complex m1 = alpha * b * (beta * mnm * sm - mnp * sp);
                    Complex m2 = alpha * c * (beta * (e + f * mnm * mnm) * cm - (e + f * mnp) * cp);
                    Complex m3 = alpha * d * (beta * mnm * (3.0 * e + f * mnm * mnm) * sm - mnp * (3.0 * e + f * mnp * mnp) * sp);

                    Complex r = this.a * m0; // + this.b * m1 * this.c * m2 + this.d * m3;
                    if (double.IsNaN(r.Real) || double.IsNaN(r.Imaginary))
                        r = 0;

                    if (i == j)
                        r += t;

                    if (double.IsInfinity(r.Real) || double.IsInfinity(r.Imaginary) || double.IsNaN(r.Real) || double.IsNaN(r.Imaginary))
                        break;

                    if (matrix is CMatrix) {
                        matrix[i, 2 * j] = r.Real;
                        matrix[i, 2 * j + 1] = r.Imaginary;
                        matrix[j, 2 * i] = r.Real;
                        matrix[j, 2 * i + 1] = r.Imaginary;
                    }
                    else {
                        if (index.Real) {
                            matrix[i, j] = r.Real;
                            matrix[j, i] = r.Real;
                        }
                        else {
                            matrix[i, j] = r.Imaginary;
                            matrix[j, i] = r.Imaginary;
                        }
                    }

                    k++;
                }
                if (writer != null) {
                    if ((i + 1) % step == 0)
                        writer.Write(".");
                    if ((i + 1) % (10 * step) == 0) {
                        writer.Write(100 * k / numElements);
                        writer.Write("%...");
                        writer.WriteLine(SpecialFormat.Format(DateTime.Now - timeStart));
                    }
                }           
            }
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
        public ComplexScatteringExplicit(Core.Import import) {
            IEParam param = new IEParam(import);
            this.eigenSystem = (EigenSystem)param.Get();
            this.theta = (double)param.Get(0.0);
            this.a = (double)param.Get(-1.0);
            this.b = (double)param.Get(0.0);
            this.c = (double)param.Get(0.5);
            this.d = (double)param.Get(0.0);

            this.eigenSystem.SetParrentQuantumSystem(this);
        }

        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.eigenSystem, "EigenSystem");
            param.Add(this.theta, "Theta");
            param.Add(this.a, "A");
            param.Add(this.b, "B");
            param.Add(this.c, "C");
            param.Add(this.d, "D");
            param.Export(export);
        }

        #endregion
    }
}
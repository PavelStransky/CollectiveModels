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
    public class ComplexScatteringExplicit : IQuantumSystem, IExportable, IDynamicalSystem {
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

        public int DegreesOfFreedom { get { return 1; } }

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

            double eta = 0.1;

            DateTime timeStart = DateTime.Now;
            int step = dim / 20;

            double x = 2.0 / l * System.Math.Sqrt(0.25 * System.Math.PI / eta);

            Complex z = 2 * eta * l;

            Complex a = Complex.Exp(new Complex(0, -this.theta)) * x;
            Complex b = -System.Math.PI / z * Complex.Exp(new Complex(0, -2.0 * this.theta)) * x;
            Complex c = 1.0 / (z * z) * Complex.Exp(new Complex(0, -3.0 * this.theta)) * x;
            Complex d = -System.Math.PI / (z * z * z) * Complex.Exp(new Complex(0, -4.0 * this.theta)) * x;

            Complex e = 2 * eta * l * l * Complex.Exp(new Complex(0, 2.0 * this.theta));
            Complex f = -System.Math.PI * System.Math.PI;

            Complex y = -(System.Math.PI * System.Math.PI) / (4 * eta * l * l) * Complex.Exp(new Complex(0, -2.0 * this.theta));

            for (int i = 0; i < dim; i++) {
                int m = i + 1;

                // Kinetic term
                Complex t = hbar * (i + 1) * System.Math.PI / l;
                t = 0.5 * t * t * Complex.Exp(new Complex(0, -2.0 * this.theta));

                for (int j = i; j < dim; j++) {
                    int n = j + 1;

                    int mnp = m + n;
                    int mnm = m - n;

                    Complex alpha = Complex.Exp(y * mnm * mnm);
                    Complex beta = Complex.Exp(y * mnp * mnp);

                    int cm = mnm % 2 != 0 ? 0 : (mnm % 4 == 0 ? 1 : -1);
                    int cp = mnp % 2 != 0 ? 0 : (mnp % 4 == 0 ? 1 : -1);

                    int sm = mnm % 2 == 0 ? 0 : ((mnm - 1) % 4 == 0 ? 1 : -1);
                    int sp = mnp % 2 == 0 ? 0 : ((mnp - 1) % 4 == 0 ? 1 : -1);

                    Complex m0 = cm * alpha - cp * beta;
                    Complex m1 = -(sm * alpha * mnm - sp * beta * mnp);
                    Complex m2 = cm * alpha * (e + f * mnm * mnm) - cp * beta * (e + f * mnp * mnp);
                    Complex m3 = -(sm * alpha * mnm * (3.0 * e + f * mnm * mnm) - sp * beta * mnp * (3.0 * e + f * mnp * mnp));

                    Complex r = this.a * a * m0 + this.b * b * m1 + this.c * c * m2 + this.d * d * m3;

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
                }
                if (writer != null && i % step == 0)
                    writer.Write(".");
            }

            if (writer != null)
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - timeStart));
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

        public double T(double p) {
            return 0.5 * p * p;
        }

        public double V(double x) {
            return (this.a + x * (this.b + x * (this.c + this.d * x))) * System.Math.Exp(-0.1 * x * x);
        }

        public double E(Vector x) {
            return this.T(x[1]) + this.V(x[0]);
        }

        public Matrix Jacobian(Vector x) {
            throw new NotImplementedException();
        }

        public Vector Equation(Vector x) {
            throw new NotImplementedException();
        }

        public Vector IC(double e) {
            throw new NotImplementedException();
        }

        public Vector IC(double e, double l) {
            throw new NotImplementedException();
        }

        public bool IC(Vector ic, double e) {
            throw new NotImplementedException();
        }

        public Vector Bounds(double e) {
            throw new NotImplementedException();
        }

        public Vector CheckBounds(Vector bounds) {
            throw new NotImplementedException();
        }

        public double PeresInvariant(Vector x) {
            throw new NotImplementedException();
        }

        public bool PostProcess(Vector x) {
            throw new NotImplementedException();
        }

        public double[] SALIDecisionPoints() {
            throw new NotImplementedException();
        }

        #endregion
    }
}
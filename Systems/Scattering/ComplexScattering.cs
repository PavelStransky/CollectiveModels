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
    public class ComplexScattering : IQuantumSystem, IExportable, IDynamicalSystem {
        // Systém s vlastními hodnotami
        private EigenSystem eigenSystem;

        private double a, b, c, d;     // Parametry potenciálu
        private double theta;          // Komplexní škálování

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected ComplexScattering() { }

        /// <summary>
        /// Systém vlastních hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        public int DegreesOfFreedom { get { return 1; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComplexScattering(double theta, double a, double b, double c, double d) { 
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
            int division = index.Division;

            Vector xCache = new Vector(division);
            for (int i = 0; i < division; i++)
                xCache[i] = ((double)i / (division - 1) - 0.5) * l;

            if (writer != null) 
                writer.WriteLine("Preparing potential cache...");
            
            ComplexVector vCache = this.VCache(xCache);

            if (writer != null)
                writer.WriteLine("Preparing basis cache...");
            Vector[] sCache = this.SCache(dim, xCache);

            DateTime timeStart = DateTime.Now;
            int step = dim / 100;
            int numElements = dim * (dim + 1) / 2;

            int k = 0;

            for (int i = 0; i < dim; i++) {
                Complex z = hbar * (i + 1) * System.Math.PI / l;
                z = 0.5 * z * z * Complex.Exp(new Complex(0, -2.0 * this.theta));

                for (int j = i; j < dim; j++) {
                    Complex r = this.IntegrateMatrixElement(sCache[i], sCache[j], vCache);
                    if (i == j)
                        r += z;

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

        private Vector [] SCache(int dim, Vector xCache) {
            Vector[] result = new Vector[dim];

            int length = xCache.Length;
            double coef1 = System.Math.PI / (length - 1);
            double coef2 = System.Math.Sqrt(2.0 / length);

            for (int m = 0; m < dim; m++) {
                Vector v = new Vector(length);
                for (int i = 0; i < length; i++)
                    v[i] = coef2 * System.Math.Sin(coef1 * i * (m + 1));
                result[m] = v;
            }

            return result;
        }

        private Complex IntegrateMatrixElement(Vector sCache1, Vector sCache2, ComplexVector vCache) {
            Complex result = new Complex();

            int length = vCache.Length;
            for (int i = 0; i < length; i++) 
                result += sCache1[i] * sCache2[i] * vCache[i];
            
            return result;
        }

        private ComplexVector VCache(Vector xCache) {
            ComplexVector result = new ComplexVector(xCache.Length);
            Complex thcexp = Complex.Exp(new Complex(0, this.theta));

            for (int i = 0; i < xCache.Length; i++) {
                result[i] = this.V(xCache[i] * thcexp);
            }

            return result;
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
        public ComplexScattering(Core.Import import) {
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
            Vector result = new Vector(2);

            result[0] = x[1];
        
            double q = x[0];
            double q2 = q * q;

            result[1] = -0.2 * System.Math.Exp(-0.1 * q2) * (this.b * (q2 - 5.0) - q * (this.a + this.d * q * (q2 - 15.0) + this.c * (q2 - 10)));

            return result;
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
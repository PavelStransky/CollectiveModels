using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;
                     
namespace PavelStransky.Systems {
    public class ComplexScattering : IQuantumSystem, IExportable {
        // Systém s vlastními hodnotami
        private EigenSystem eigenSystem;

        private double z;       // Parametr potenciálu
        private double theta;   // Komplexní škálování

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected ComplexScattering() { }

        /// <summary>
        /// Systém vlastních hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComplexScattering(double z, double theta) { 
            this.eigenSystem = new EigenSystem(this);
            this.z = z;
            this.theta = theta;
        }

        /// <summary>
        /// Vytvoří instanci třídy s parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public virtual BasisIndex CreateBasisIndex(Vector basisParams) {
            return new ComplexScatteringBasisIndex(basisParams);
        }

        public Complex V(Complex r) {
            return (0.5 * r * (r + this.z) - 1) * Complex.Exp(-0.1 * r * r);
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

            ComplexVector vCache = this.VCache(xCache);

            if (matrix is CMatrix) {
                int step = dim / 50;

                for (int i = 0; i < dim; i++) {
                    for (int j = i; j < dim; j++) {
                        Complex r = this.IntegrateMatrixElement(i+1, j+1, vCache);
                        if (i == j) {
                            double b = hbar * (i + 1) * System.Math.PI / l;
                            r += 0.5 * b * b * Complex.Exp(new Complex(0, -2.0 * this.theta));
                        }
                        matrix[i, 2 * j] = r.Real;
                        matrix[i, 2 * j + 1] = r.Imaginary;
                        matrix[j, 2 * i] = r.Real;
                        matrix[j, 2 * i + 1] = r.Imaginary;
                    }
                    if (writer != null && i % step == 0) {
                        writer.Write(".");
                    }
                }
            }
        }

        private Complex IntegrateMatrixElement(int m, int n, ComplexVector vCache) {
            Complex result = new Complex();

            int length = vCache.Length;

            for (int i = 0; i < length; i++) {
                double r = i * System.Math.PI / (length - 1);
                result += System.Math.Sin(m * r) * System.Math.Sin(n * r) * vCache[i];
            }

            return 2.0 * result / length;
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
            this.z = (double)param.Get(0.0);
            this.theta = (double)param.Get(0.0);

            this.eigenSystem.SetParrentQuantumSystem(this);
        }

        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.eigenSystem, "EigenSystem");
            param.Add(this.z, "Z");
            param.Add(this.theta, "Theta");
            param.Export(export);
        }

        #endregion
    }
}                      
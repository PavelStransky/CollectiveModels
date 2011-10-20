using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Sturm-Coulomb problem
    /// </summary>
    /// <remarks>M. Moshinsky et al., Physics of Atomic Nuclei 65, 976 (2002)</remarks>
    public class SturmCoulomb: IQuantumSystem, IExportable {
        // Systém s vlastními hodnotami
        private EigenSystem eigenSystem;

        // Intenzita magnetického pole H
        private double h;

        /// <summary>
        /// Systém vlastních hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected SturmCoulomb() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="h">Intenzita magnetického pole</param>
        public SturmCoulomb(double h) {
            this.h = h;
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvoøí instanci tøídy s parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public BasisIndex CreateBasisIndex(Vector basisParams) {
            return new SCBasisIndex(basisParams);
        }

        /// <summary>
        /// Funkce B (podle èlánku)
        /// </summary>
        private double B(int n1, int l1, int n2, int l2, int k) {
            double coefLog = 0.5 * (SpecialFunctions.FactorialILog(n1) + SpecialFunctions.FactorialILog(n2)
                - SpecialFunctions.FactorialILog(n1 + 2 * l1 + 1) - SpecialFunctions.FactorialILog(n2 + 2 * l2 + 1))
                + SpecialFunctions.FactorialILog(l2 - l1 + k - 1) + SpecialFunctions.FactorialILog(l1 - l2 + k - 1);

            int smin = System.Math.Min(l2 - l1 + k - 1 - n1, l1 - l2 + k - 1 - n2);
            smin = System.Math.Max(0, -smin);

            int smax = System.Math.Min(n1, n2);
            
            double result = 0.0;
            for(int s = smin; s <= smax; s++)
                result += System.Math.Exp(SpecialFunctions.FactorialILog(l1 + l2 + k + s)
                    - SpecialFunctions.FactorialILog(l2 - l1 + k - 1 - n1 + s) - SpecialFunctions.FactorialILog(n1 - s)
                    - SpecialFunctions.FactorialILog(l1 - l2 + k - 1 - n2 + s) - SpecialFunctions.FactorialILog(n2 - s)
                    - SpecialFunctions.FactorialILog(s) + coefLog);

            if((n1 + n2) % 2 != 0)
                result = -result;

            return result;
        }

        /// <summary>
        /// Funkce u podle èlánku
        /// </summary>
        private double U(int l, int m) {
            return System.Math.Sqrt((l - m + 2.0) * (l - m + 1.0) * (l + m + 2.0) * (l + m + 1.0) / ((2.0 * l + 5.0) * (2.0 * l + 1.0))) / (2.0 * l + 3);
        }

        /// <summary>
        /// Funkce v podle èlánku
        /// </summary>
        private double V(int l, int m) {
            return 2.0 / 3.0 * (1.0 + (3.0 * m * m - l * (l + 1.0)) / ((2.0 * l - 1.0) * (2.0 * l + 3)));
        }

        /// <summary>
        /// Funkce w podle èlánku
        /// </summary>
        private double W(int l, int m) {
            return System.Math.Sqrt((l - m) * (l - m - 1.0) * (l + m) * (l + m - 1.0) / ((2.0 * l + 1.0) * (2.0 * l - 3.0))) / (2.0 * l - 1);
        }
        
        /// <summary>
        /// Vypoèítá symetrickou Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            SCBasisIndex index = basisIndex as SCBasisIndex;

            int dim = index.Length;
            int m = index.M;
            double coef = this.h * this.h / 4.0;

            for(int i1 = 0; i1 < dim; i1++) {
                int n1 = index.N[i1];
                int l1 = index.L[i1];

                for(int i2 = i1; i2 < dim; i2++) {
                    int n2 = index.N[i2];
                    int l2 = index.L[i2];

                    if(l1 < m || l2 < m)
                        continue;

                    double result = 0.0;
                    if(n1 == n2 && l1 == l2)
                        result += n1 + l1 + 1.0;

                    if(l1 == l2) {
                        result -= this.h * m * this.B(n1, l1, n2, l1, 2);
                        result += coef * this.B(n1, l1, n2, l1, 4) * this.V(l1, m);
                    }

                    else if(l2 == l1 + 2)
                        result -= coef * this.B(n1, l1, n2, l2, 4) * this.W(l2, m);

                    else if(l1 == l2 + 2)
                        result -= coef * this.B(n1, l1, n2, l2, 4) * this.U(l2, m);

                    if(result != 0.0) {
                        matrix[i1, i2] = result;
                        matrix[i2, i1] = result;
                    }
                }
            }
        }

        /// <summary>
        /// Peresùv invariant
        /// </summary>
        /// <param name="type">Typ (0 - L(L + 1), 1 - H0)</param>
        public Vector PeresInvariant(int type) {
            SCBasisIndex index = this.eigenSystem.BasisIndex as SCBasisIndex;

            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenSystem.GetEigenVector(i);
                int length = ev.Length;

                for(int j = 0; j < length; j++) {
                    double koef = 0.0;
                    switch(type) {
                        case 0:
                            koef = index.L[j]; koef *= koef + 1;
                            break;
                        case 1:
                            koef = index.L[j] + index.N[j] + 1;
                            break;
                    }

                    result[i] += ev[j] * ev[j] * koef;
                }
            }

            return result;
        }

        public object ProbabilityDensity(int[] n, IOutputWriter writer, params Vector[] interval) {
            throw new NotImpException(this, "ProbabilityDensity");
        }

        public double ProbabilityAmplitude(int n, IOutputWriter writer, params double[] x) {
            throw new NotImpException(this, "ProbabilityAmplitude");
        }

        #region Implementace IExportable
        /// <summary>
        /// Uloží výsledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.h, "MagneticField");
            param.Add(this.eigenSystem, "EigenSystem");
            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public SturmCoulomb(Core.Import import) {
            IEParam param = new IEParam(import);
            this.h = (double)param.Get(0.0);
            this.eigenSystem = (EigenSystem)param.Get();
            this.eigenSystem.SetParrentQuantumSystem(this);
        }
        #endregion
    }
}
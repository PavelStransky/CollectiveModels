using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class Spheroid: IQuantumSystem, IExportable {
        // Systém s vlastními hodnotami
        private EigenSystem eigenSystem;

        // Parametr deformace delta
        private double delta;

        /// <summary>
        /// Systém vlastních hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected Spheroid() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="delta">Deformace</param>
        public Spheroid(double delta){
            this.delta = delta;
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvoøí instanci tøídy s parametry báze
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public BasisIndex CreateBasisIndex(Vector basisParams) {
            return new SpheroidBasisIndex(basisParams);
        }

        /// <summary>
        /// Vypoèítá symetrickou Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            SpheroidBasisIndex index = basisIndex as SpheroidBasisIndex;

            int dim = index.Length;

            double koef1 = (2.0 * System.Math.Exp(2.0 * this.delta) + System.Math.Exp(-4.0 * this.delta)) / 3.0;
            double koef2 = -2.0 * (System.Math.Exp(2.0 * this.delta) - System.Math.Exp(-4.0 * this.delta)) / 3.0;

            for(int i1 = 0; i1 < dim; i1++) {
                int n1 = index.N[i1];
                int l1 = index.L[i1];
                int m1 = index.M[i1];
                double e1 = index.E[i1];

                for(int i2 = i1; i2 < dim; i2++) {
                    int n2 = index.N[i2];
                    int l2 = index.L[i2];
                    int m2 = index.M[i2];
                    double e2 = index.E[i2];

                    double p2 = i1 == i2 ? e1 * e1 : 0.0;
                    double T2 = 0.0;

                    if(l1 == l2)
                        T2 = (n1 == n2 && m1 == m2)
                            ? e1 * e1 * (l1 * (l1 + 1) - 3.0 * m1 * m1) / ((2.0 * l1 - 1) * (2.0 * l1 + 3))
                            : 0.0;
                    else if(l2 == l1 + 2)
                        T2 = (m1 == m2)
                            ? e1 * e2 / (e2 * e2 - e1 * e1) * 3.0 * System.Math.Sqrt((l1 + 1.0 + m1) * (l1 + 2.0 + m1) * (l1 + 1.0 - m1) * (l1 + 2.0 - m1) / ((2.0 * l1 + 1) * (2.0 * l1 + 5)))
                            : 0;
                    else if(l1 == l2 + 2)
                        T2 = (m1 == m2)
                            ? e1 * e2 / (e1 * e1 - e2 * e2) * 3.0 * System.Math.Sqrt((l2 + 1.0 + m2) * (l2 + 2.0 + m2) * (l2 + 1.0 - m2) * (l2 + 2.0 - m2) / ((2.0 * l2 + 1) * (2.0 * l2 + 5)))
                            : 0;

                    matrix[i1, i2] = koef1 * p2 + koef2 * T2;
                    matrix[i2, i1] = matrix[i1, i2];
                }
            }
        }

        /// <summary>
        /// Peresùv invariant
        /// </summary>
        /// <param name="type">Typ (0 - N, 1 - L, 2 - L(L + 1))</param>
        public Vector PeresInvariant(int type) {
            SpheroidBasisIndex index = this.eigenSystem.BasisIndex as SpheroidBasisIndex;

            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenSystem.GetEigenVector(i);
                int length = ev.Length;

                for(int j = 0; j < length; j++) {
                    double koef = 0.0;
                    switch(type) {
                        case 0:
                            koef = index.N[j];
                            break;
                        case 1:
                            koef = index.L[j];
                            break;
                        case 2:
                            koef = index.L[j]; koef *= koef + 1;
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
            param.Add(this.delta, "Deformation");
            param.Add(this.eigenSystem, "EigenSystem");
            param.Export(export);
        }

        /// <summary>
        /// Naète výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public Spheroid(Core.Import import) {
            IEParam param = new IEParam(import);
            this.delta = (double)param.Get(0.0);
            this.eigenSystem = (EigenSystem)param.Get();
            this.eigenSystem.SetParrentQuantumSystem(this);
        }

        #endregion
    }
}
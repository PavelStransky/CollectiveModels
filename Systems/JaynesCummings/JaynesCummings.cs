using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class JaynesCummings: IQuantumSystem, IExportable {
        // Syst�m s vlastn�mi hodnotami
        protected EigenSystem eigenSystem;

        // parametry
        protected double omega, omega0, lambda;

        /// <summary>
        /// Syst�m vlastn�ch hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected JaynesCummings() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public JaynesCummings(double omega, double omega0, double lambda) {
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvo�� instanci t��dy s parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public virtual BasisIndex CreateBasisIndex(Vector basisParams) {
            return new JaynesCummingsBasisIndex(basisParams);
        }

        /// <summary>
        /// Napln� Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        public virtual void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            JaynesCummingsBasisIndex index = basisIndex as JaynesCummingsBasisIndex;

            int dim = index.Length;
            double l = this.lambda / System.Math.Sqrt(index.M2);
            int j = index.J;

            for(int i = 0; i < dim; i++) {
                int nb = index.Nb[i];
                int m = index.M[i];

                int i1 = index[nb + 1, m - 1];
                int i2 = index[nb - 1, m + 1];

                matrix[i, i] = this.omega * nb + this.omega0 * m;
                if(i1 >= 0)
                    matrix[i1, i] = l * this.ShiftMinus(j, m) * System.Math.Sqrt(nb + 1);
                if(i2 >= 0)
                    matrix[i2, i] = l * this.ShiftPlus(j, m) * System.Math.Sqrt(nb);
            }
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

        /// <summary>
        /// Peres�v invariant
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
        /// Ulo�� v�sledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.eigenSystem, "EigenSystem");
            param.Add(this.omega, "Omega");
            param.Add(this.omega0, "Omega0");
            param.Add(this.lambda, "Lambda");
            param.Export(export);
        }

        /// <summary>
        /// Na�te v�sledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public JaynesCummings(Core.Import import) {
            IEParam param = new IEParam(import);
            this.eigenSystem = (EigenSystem)param.Get();
            this.omega = (double)param.Get(0.0);
            this.omega0 = (double)param.Get(0.0);
            this.lambda = (double)param.Get(0.0);
        }
        #endregion
    }
}
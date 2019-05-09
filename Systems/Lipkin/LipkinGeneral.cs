using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Lipkin model in its most general two-body case
    /// </summary>
    public class LipkinGeneral : IQuantumSystem, IExportable {
        protected EigenSystem eigenSystem;
                                          
        /// <summary>
        /// Eigensystem
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        private readonly double jx, jx2, jz, jz2, jxz;
                             
        /// <summary>
        /// Empty constructor
        /// </summary>
        protected LipkinGeneral() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="jx">Parameter of the Jx term</param>
        /// <param name="jz">Parameter of the Jz term</param>
        /// <param name="jx2">Parameter of the Jx^2 term</param>
        /// <param name="jz2">Parameter of the Jz^2 term</param>
        /// <param name="jxz">Parameter of the mixed Jx*Jz term</param>
        public LipkinGeneral(double jx, double jz, double jx2, double jz2, double jxz) {
            this.jx = jx;
            this.jz = jz;
            this.jx2 = jx2;
            this.jz2 = jz2;
            this.jxz = jxz;

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

            for (int i = 0; i < dim; i++) {
                int l = index.L[i];
                int m = index.M[i];

                matrix[i, i] = this.jz * m + (this.jz2 - 0.5 * this.jx2) * m * m;

                // -1
                if (i - 1 >= 0 && l == index.L[i - 1])
                    matrix[i - 1, i] = 0.5 * this.ShiftMinus(l, m) * (this.jx + this.jxz * (m - 1));
                // +1
                if (i + 1 < dim && l == index.L[i + 1]) 
                    matrix[i + 1, i] = 0.5 * this.ShiftMinus(l, m) * (this.jx + this.jxz * (m + 1));
                // -2
                if (i - 2 >= 0 && l == index.L[i - 2])
                    matrix[i - 2, i] = 0.25 * this.jx2 * this.ShiftMinus(l, m) * this.ShiftMinus(l, m - 2);
                // +2
                if (i + 2 < dim && l == index.L[i + 2])
                    matrix[i + 2, i] = 0.25 * this.jx2 * this.ShiftPlus(l, m) * this.ShiftPlus(l, m + 2);
            }
        }

        protected double ShiftPlus(int l, int m) {
            if (m > l || m < -l)
                return 0;
            return System.Math.Sqrt((l - m) * (l + m + 2)) / 2.0;
        }
        protected double ShiftMinus(int l, int m) {
            if (m > l || m < -l)
                return 0;
            return System.Math.Sqrt((l + m) * (l - m + 2)) / 2.0;
        }

        /// <summary>
        /// Peresův invariant
        /// </summary>
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
        /// Save parameters of the system
        /// </summary>
        /// <param name="export">Export object</param>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.eigenSystem, "EigenSystem");
            param.Add(this.jx, "A");
            param.Add(this.jz, "B");
            param.Add(this.jx2, "C");
            param.Add(this.jz2, "D");
            param.Add(this.jxz, "E");

            param.Export(export);
        }

        /// <summary>
        /// Načte výsledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public LipkinGeneral(Core.Import import) {
            IEParam param = new IEParam(import);
            this.eigenSystem = (EigenSystem)param.Get();
            this.jx = (double)param.Get(0.0);
            this.jz = (double)param.Get(0.0);
            this.jx2 = (double)param.Get(0.0);
            this.jz2 = (double)param.Get(0.0);
            this.jxz = (double)param.Get(0.0);
        }

        #endregion
    }
}
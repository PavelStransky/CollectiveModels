using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.DLLWrapper;
using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Obsahuje vlastn� ��sla a vlastn� hodnoty
    /// </summary>
    public class EigenSystem: IExportable {
        private IQuantumSystem parrentQuantumSystem;

        private Vector eigenValues;
        private Vector[] eigenVectors = new Vector[0];

        // True if it has been calculated
        private bool isComputed = false;

        // True if the calculation is in progress
        private bool isComputing = false;

        /// <summary>
        /// True if it has been calculated
        /// </summary>
        public bool IsComputed { get { return this.isComputed; } }

        // T��da s parametry b�ze
        private BasisIndex basisIndex;

        /// <summary>
        /// T��da s parametry b�ze
        /// </summary>
        public BasisIndex BasisIndex { get { return this.basisIndex; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="system">Kvantov� syst�m (vlastn�k)</param>
        public EigenSystem(IQuantumSystem system) {
            this.parrentQuantumSystem = system;
        }

        /// <summary>
        /// Pokud syst�m nebyl vypo��t�n, vyhod� v�jimku
        /// </summary>
        private void CheckComputed() {
            if(!this.isComputed)
                throw new SystemsException(Messages.EMNotComputed);
        }

        /// <summary>
        /// Vlastn� hodnoty
        /// </summary>
        public Vector GetEigenValues() {
            this.CheckComputed();
            return this.eigenValues;
        }

        /// <summary>
        /// Vlastn� vektor
        /// </summary>
        /// <param name="i">Index vektoru</param>
        public Vector GetEigenVector(int i) {
            this.CheckComputed();
            if(this.eigenVectors.Length == 0)
                throw new SystemsException(Messages.EMNoEigenVectors);

            return this.eigenVectors[i];
        }

        /// <summary>
        /// Po�et vlastn�ch vektor�
        /// </summary>
        public int NumEV { get { this.CheckComputed();  return this.eigenVectors.Length; } }

        /// <summary>
        /// Nastav� kvantov� syst�m kv�li v�po�t�m
        /// </summary>
        /// <param name="system">Kvantov� syst�m (vlastn�k)</param>
        public void SetParrentQuantumSystem(IQuantumSystem system) {
            this.parrentQuantumSystem = system;
        }

        /// <summary>
        /// Kv�li kompatibilit� s p�edchoz�m ukl�d�n�m
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public void SetBasisParams(Vector basisParams) {
            this.basisIndex = this.parrentQuantumSystem.CreateBasisIndex(basisParams);
        }

        /// <summary>
        /// Provede v�po�et (diagonalizaci)
        /// </summary>
        /// <param name="basisParams">Parametry b�ze (nejvy��� energie b�zov�ch funkc�)</param>
        /// <param name="ev">True, pokud budeme po��tat i vlastn� vektory</param>
        /// <param name="numEV">Po�et vlastn�ch hodnot, men�� �i rovn� 0 vypo��t� v�echny</param>
        /// <param name="writer">Writer</param>
        /// <param name="method">Metoda v�po�tu</param>
        public void Diagonalize(Vector basisParams, bool ev, int numEV, IOutputWriter writer, ComputeMethod method) {
            this.basisIndex = this.parrentQuantumSystem.CreateBasisIndex(basisParams);

            if(writer != null) {
                writer.WriteLine(string.Format(Messages.MDiagonalizationStart, DateTime.Now, this.parrentQuantumSystem.GetType().Name));
                writer.WriteLine(Messages.MHMCalculation);
            }

            if(method == ComputeMethod.Jacobi) {
                Matrix m = this.parrentQuantumSystem.HamiltonianMatrix(this.basisIndex, writer);
                this.Diagonalize(m, ev, numEV, writer);
            }

            else if(method == ComputeMethod.LAPACKBand) {
                SymmetricBandMatrix m = this.parrentQuantumSystem.HamiltonianSBMatrix(this.basisIndex, writer);
                this.Diagonalize(m, ev, numEV, writer);
            }
        }

        /// <summary>
        /// Napo��t� Hamiltonovu matici
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        public Matrix HamiltonianMatrix(Vector basisParams, IOutputWriter writer) {
            if(writer != null) {
                writer.WriteLine(Messages.MHMCalculation);
            }

            return this.parrentQuantumSystem.HamiltonianMatrix(this.parrentQuantumSystem.CreateBasisIndex(basisParams), writer);
        }

        /// <summary>
        /// Napo��t� stopu Hamiltonovy matice
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public double HamiltonianMatrixTrace(Vector basisParams) {
            return this.parrentQuantumSystem.HamiltonianMatrixTrace(this.parrentQuantumSystem.CreateBasisIndex(basisParams));
        }

        /// <summary>
        /// Napo��t� rozm�ry Hamiltonovy matice
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public int HamiltonianMatrixSize(Vector basisParams) {
            return this.parrentQuantumSystem.CreateBasisIndex(basisParams).Length;
        }
        
        /// <summary>
        /// Provede v�po�et (diagonalizaci)
        /// </summary>
        /// <param name="maxn">Nejvy��� energie b�zov�ch funkc�</param>
        /// <param name="ev">True, pokud budeme po��tat i vlastn� vektory</param>
        /// <param name="numEV">Po�et vlastn�ch hodnot, men�� �i rovn� 0 vypo��t� v�echny</param>
        /// <param name="writer">Writer</param>
        /// <param name="method">Metoda v�po�tu</param>
        public void Diagonalize(int maxn, bool ev, int numEV, IOutputWriter writer, ComputeMethod method) {
            Vector basisParams = new Vector(1);
            basisParams[0] = maxn;

            this.Diagonalize(basisParams, ev, numEV, writer, method);
        }

        /// <summary>
        /// Provede v�po�et (diagonalizaci)
        /// </summary>
        /// <param name="maxn">Nejvy��� energie b�zov�ch funkc�</param>
        /// <param name="ev">True, pokud budeme po��tat i vlastn� vektory</param>
        /// <param name="numEV">Po�et vlastn�ch hodnot, men�� �i rovn� 0 vypo��t� v�echny</param>
        /// <param name="writer">Writer</param>
        public void Diagonalize(int maxn, bool ev, int numEV, IOutputWriter writer) {
            this.Diagonalize(maxn, ev, numEV, writer, ComputeMethod.LAPACKBand);
        }

        /// <summary>
        /// Diagonalizace u�it�m Jacobi metody
        /// </summary>
        /// <param name="matrix">Symetrick� matice</param>
        /// <param name="ev">True, pokud budeme po��tat i vlastn� vektory</param>
        /// <param name="numev">Po�et vlastn�ch hodnot, men�� �i rovn� 0 vypo��t� v�echny</param>
        /// <param name="writer">Writer</param>
        public void Diagonalize(Matrix matrix, bool ev, int numEV, IOutputWriter writer) {
            if(this.isComputing)
                throw new SystemsException(Messages.EMComputing);

            this.isComputing = true; 
            
            try {
                if(numEV <= 0)
                    numEV = matrix.Length;

                if(writer != null) {
                    writer.WriteLine(string.Format(Messages.MSMatrixDimension, matrix.Length));
                    writer.WriteLine(string.Format(Messages.MTrace, matrix.Trace()));
                    writer.WriteLine(string.Format(Messages.MNonzeroElements, matrix.NumNonzeroItems(), matrix.NumItems()));
                    writer.Write(string.Format(Messages.MDiagonalizationJacobi, numEV, ev ? Messages.MDiagonalizationEVYes : Messages.MDiagonalizationEVNo));
                }

                Jacobi jacobi = new Jacobi(matrix, writer);
                jacobi.SortAsc();

                this.eigenValues = new Vector(jacobi.EigenValue);
                this.eigenValues.Length = numEV;

                if(ev) {
                    this.eigenVectors = new Vector[numEV];
                    for(int i = 0; i < numEV; i++)
                        this.eigenVectors[i] = jacobi.EigenVector[i];
                }
                else
                    this.eigenVectors = new Vector[0];

                this.isComputed = true;
                this.isComputing = false;
            }
            finally {
                this.isComputing = false;
            }
        }

        /// <summary>
        /// Diagonalizace u�it�m LAPACK knihovny
        /// </summary>
        /// <param name="matrix">Symetrick� p�sov� matice</param>
        /// <param name="ev">True, pokud budeme po��tat i vlastn� vektory</param>
        /// <param name="numev">Po�et vlastn�ch hodnot, men�� �i rovn� 0 vypo��t� v�echny</param>
        /// <param name="writer">Writer</param>
        public void Diagonalize(SymmetricBandMatrix matrix, bool ev, int numEV, IOutputWriter writer) {
            if(this.isComputing)
                throw new SystemsException(Messages.EMComputing);

            this.isComputing = true;

            try {
                if(numEV <= 0)
                    numEV = matrix.Length;

                if(writer != null) {
                    writer.WriteLine(string.Format(Messages.MSBMatrixDimension, matrix.Length, matrix.NumSD));
                    writer.WriteLine(string.Format(Messages.MTrace, matrix.Trace()));
                    writer.Write(string.Format(Messages.MDiagonalizationDSBEVX, numEV, ev ? Messages.MDiagonalizationEVYes : Messages.MDiagonalizationEVNo));
                }

                GC.Collect();

                DateTime startTime1 = DateTime.Now;

                Vector[] eigenSystem = LAPackDLL.dsbevx(matrix, ev, 0, numEV);
                matrix.Dispose();

                GC.Collect();

                if(writer != null)
                    writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime1));

                this.eigenValues = eigenSystem[0];
                this.eigenValues.Length = numEV;

                if(ev) {
                    this.eigenVectors = new Vector[numEV];
                    for(int i = 0; i < numEV; i++)
                        this.eigenVectors[i] = eigenSystem[i + 1];
                }
                else
                    this.eigenVectors = new Vector[0];

                this.isComputing = false;
                this.isComputed = true;
            }
            finally {
                this.isComputing = false;
            }
        }

        /// <summary>
        /// Time evolution of the n-th basis vector
        /// </summary>
        /// <param name="bi">Index of the basis vector</param>
        /// <param name="t">Time of the evolution</param>
        public PointVector TimeEvolution(Vector bi, double t) {
            int i = this.basisIndex[bi];

            int dim = this.basisIndex.Length;
            int numEV = this.NumEV;

            Vector re = new Vector(dim);
            Vector im = new Vector(dim);

            Vector evalues = this.eigenValues;

            for(int k = 0; k < numEV; k++) {
                Vector ev = this.eigenVectors[k];
                for(int j = 0; j < dim; j++) {
                    double c = ev[i] * ev[j];
                    re[j] += c * System.Math.Cos(-evalues[k] * t);
                    im[j] += c * System.Math.Sin(-evalues[k] * t);
                }
            }

            return new PointVector(re, im);
        }

        /// <summary>
        /// Action of a Hamiltonian operator to a given ket
        /// </summary>
        /// <param name="ket">Ket</param>
        public PointVector HamiltonianAction(PointVector ket) {
            int dim = this.basisIndex.Length;
            int numEV = this.NumEV;

            Vector re = new Vector(dim);
            Vector im = new Vector(dim);

            Vector rei = ket.VectorX;
            Vector imi = ket.VectorY;

            Vector evalues = this.eigenValues;

            for(int i = 0; i < dim; i++) {
                for(int j = 0; j < numEV; j++) {
                    Vector ev = this.eigenVectors[j];
                    double c = evalues[j] * ev[i];
                    for(int k = 0; k < dim; k++) {
                        double d = c * ev[k];
                        re[i] += d * rei[k];
                        im[i] += d * imi[k];
                    }
                }
            }

            return new PointVector(re, im);
        }

        /// <summary>
        /// Konstruktor pro na�ten� star� verze dat (6 a star��)
        /// </summary>
        /// <param name="param">Parametr</param>
        public EigenSystem(IEParam param) {
            this.isComputed = (bool)param.Get(false);

            if(this.isComputed) {
                this.eigenValues = (Vector)param.Get(null);

                int numEV = (int)param.Get(0);
                this.eigenVectors = new Vector[numEV];

                for(int i = 0; i < numEV; i++)
                    this.eigenVectors[i] = (Vector)param.Get();
            }
        }

        #region Implementace IExportable

        /// <summary>
        /// Ulo�� v�sledky do souboru
        /// </summary>
        /// <param name="export">Export</param>
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.isComputed, "IsComputed");

            if(this.isComputed) {
                param.Add(this.basisIndex, "Basis Parameters");
                param.Add(this.eigenValues, "EigenValues");

                int numEV = this.NumEV;
                param.Add(numEV, "EigenVector Number");

                for(int i = 0; i < numEV; i++)
                    param.Add(this.eigenVectors[i]);
            }
            param.Export(export);
        }

        /// <summary>
        /// Na�te v�sledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public EigenSystem(Core.Import import) {
            IEParam param = new IEParam(import);

            this.isComputed = (bool)param.Get(false);

            if(this.isComputed) {
                if(import.VersionNumber >= 8)
                    this.basisIndex = (BasisIndex)param.Get(null);

                this.eigenValues = (Vector)param.Get(null);

                int numEV = (int)param.Get(0);
                this.eigenVectors = new Vector[numEV];

                for(int i = 0; i < numEV; i++)
                    this.eigenVectors[i] = (Vector)param.Get();
            }
        }
        #endregion
    }
}

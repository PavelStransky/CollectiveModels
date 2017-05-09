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
        private Vector eigenValuesIm;
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
        public object GetEigenValues() {
            this.CheckComputed();
            if(this.eigenValuesIm != null)
                return new PointVector(this.eigenValues, this.eigenValuesIm);
            else
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

            IMatrix matrix = null;
            if(method == ComputeMethod.Jacobi)
                matrix = new Matrix(this.basisIndex.Length);
            else if(method == ComputeMethod.LAPACKBand)
                matrix = new SymmetricBandMatrix(this.basisIndex.Length, this.basisIndex.BandWidth);
            else if(method == ComputeMethod.LAPACK)
                matrix = new MMatrix(this.basisIndex.Length);
            else if(method == ComputeMethod.LAPACKComplex)
                matrix = new CMatrix(this.basisIndex.Length);
            else if(method == ComputeMethod.ARPACK)
                matrix = new SparseMatrix(this.basisIndex.Length);

            this.parrentQuantumSystem.HamiltonianMatrix(matrix, this.basisIndex, writer);

            // Za��tek v�po�tu (matice p�ipravena)
            if(this.isComputing)
                throw new SystemsException(Messages.EMComputing);

            this.isComputing = true;

            try {
                if(numEV <= 0)
                    numEV = matrix.Length;

                if(writer != null) {
                    if(matrix is Matrix || matrix is MMatrix || matrix is CMatrix)
                        writer.WriteLine(string.Format(Messages.MSMatrixDimension, matrix.Length));
                    else if(matrix is SymmetricBandMatrix)
                        writer.WriteLine(string.Format(Messages.MSBMatrixDimension, matrix.Length, (matrix as SymmetricBandMatrix).NumSD));
                    else if(matrix is SparseMatrix)
                        writer.WriteLine(string.Format(Messages.MSSMatrixDimension, matrix.Length, (matrix as SparseMatrix).NonzeroElements));

                    writer.WriteLine(string.Format(Messages.MTrace, matrix.Trace()));

                    if(matrix is Matrix) {
                        writer.WriteLine(string.Format(Messages.MNonzeroElements, (matrix as Matrix).NumNonzeroItems(), (matrix as Matrix).NumItems()));
                        writer.Write(Messages.MDiagonalizationJacobi);
                    }
                    else if(matrix is MMatrix)
                        writer.Write(Messages.MDiagonalizationDSYEV);
                    else if(matrix is CMatrix)
                        writer.Write(Messages.MDiagonalizationZGEEV);
                    else if(matrix is SymmetricBandMatrix)
                        writer.Write(Messages.MDiagonalizationDSBEVX);
                    else if(matrix is SparseMatrix)
                        writer.Write(Messages.MDiagonalizationARPACK);

                    writer.Write(string.Format(Messages.MNumEV, numEV, ev ? Messages.MDiagonalizationEVYes : Messages.MDiagonalizationEVNo));
                }

                GC.Collect();
                DateTime startTime1 = DateTime.Now;

                Vector[] eigenSystem = matrix.EigenSystem(ev, numEV, writer);

                if(matrix is IDisposable)
                    (matrix as IDisposable).Dispose();

                GC.Collect();

                if(writer != null)
                    writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime1));

                this.eigenValues = eigenSystem[0];
                if(matrix is CMatrix)
                    this.eigenValuesIm = eigenSystem[1];
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
        /// Napo��t� Hamiltonovu matici
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        public Matrix HamiltonianMatrix(Vector basisParams, IOutputWriter writer) {
            if(writer != null) {
                writer.WriteLine(Messages.MHMCalculation);
            }
            BasisIndex b = this.parrentQuantumSystem.CreateBasisIndex(basisParams);
            Matrix result = new Matrix(b.Length);
            this.parrentQuantumSystem.HamiltonianMatrix(result, b, writer);
            return result;
        }

        /// <summary>
        /// Napo��t� stopu Hamiltonovy matice
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public double HamiltonianMatrixTrace(Vector basisParams) {
            BasisIndex b = this.parrentQuantumSystem.CreateBasisIndex(basisParams);
            SymmetricBandMatrix m = new SymmetricBandMatrix(b.Length, b.BandWidth);
            this.parrentQuantumSystem.HamiltonianMatrix(m, b, null);
            return m.Trace();
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
        /// Time evolution of the given ket
        /// </summary>
        /// <param name="ket">Ket</param>
        /// <param name="t">Time of the evolution</param>
        public PointVector TimeEvolution(PointVector ket, double t) {
            int dim = this.basisIndex.Length;
            int numEV = this.NumEV;

            Vector re = new Vector(dim);
            Vector im = new Vector(dim);

            Vector rei = ket.VectorX;
            Vector imi = ket.VectorY;

            Vector evalues = this.eigenValues;

            for(int i = 0; i < numEV; i++) {
                Vector ev = this.eigenVectors[i];
                double cos = System.Math.Cos(evalues[i] * t);
                double sin = System.Math.Sin(evalues[i] * t);

                for(int j = 0; j < dim; j++) {
                    if(rei[j] == 0 && imi[j] == 0)
                        continue;
                    double re1 = rei[j] * cos + imi[j] * sin;
                    double im1 = -rei[j] * sin + imi[j] * cos;
                    for(int k = 0; k < dim; k++) {
                        double c = ev[j] * ev[k];
                        re[k] += re1 * c;
                        im[k] += im1 * c;
                    }
                }
            }

            return new PointVector(re, im);
        }

        /// <summary>
        /// Action of a Hamiltonian operator to a given ket
        /// </summary>
        /// <param name="ket">Ket</param>
        /// <param name="squared">True if the square of the Hamiltonian is to be calculated</param>
        public PointVector HamiltonianAction(PointVector ket, bool squared) {
            int dim = this.basisIndex.Length;
            int numEV = this.NumEV;

            Vector re = new Vector(dim);
            Vector im = new Vector(dim);

            Vector rei = ket.VectorX;
            Vector imi = ket.VectorY;

            Vector evalues = this.eigenValues;

            for(int i = 0; i < numEV; i++) {
                Vector ev = this.eigenVectors[i];
                for(int j = 0; j < dim; j++) {
                    if(rei[j] == 0 && imi[j] == 0)
                        continue;
                    double c = evalues[i] * ev[j];
                    if(squared)
                        c *= evalues[i];
                    for(int k = 0; k < dim; k++) {
                        double d = c * ev[k];
                        re[k] += d * rei[j];
                        im[k] += d * imi[j];
                    }
                }
            }

            return new PointVector(re, im);
        }

        /// <summary>
        /// Na z�klad� kvantov�ch ��sel vr�t� index dan�ho b�zov�ho vektoru
        /// </summary>
        /// <param name="bi">Kvantov� ��sla</param>
        public int BasisQuantumNumber(Vector bi) {
            return this.basisIndex[bi];
        }

        /// <summary>
        /// Na z�klad� indexu b�zov�ho vektoru vr�t� jeho vlastn� ��sla
        /// </summary>
        /// <param name="i">Index b�zov�ho vektoru</param>
        public Vector BasisQuantumNumber(int i) {
            return this.basisIndex[i];
        }

        /// <summary>
        /// Vr�t� b�zov� vektor
        /// </summary>
        /// <param name="i">Index b�zov�ho vektoru</param>
        public Vector BasisVector(int i) {
            Vector result = new Vector(this.basisIndex.Length);
            result[i] = 1.0;
            return result;
        }

        /// <summary>
        /// P�evede dan� stavov� vektor obsahuj�c� komponenty b�ze na komponenty vlastn�ch vektor�
        /// </summary>
        /// <param name="state">Stavov� vektor vyj�d�en� v rozvoji do b�ze</param>
        public PointVector BasisToEV(PointVector state) {
            int dim = this.basisIndex.Length;
            int numEV = this.NumEV;

            PointVector result = new PointVector(numEV);

            for(int i = 0; i < numEV; i++) {
                double x = 0.0;
                double y = 0.0;

                Vector ev = this.eigenVectors[i];

                for(int j = 0; j < dim; j++) {
                    x += state[j].X * ev[j];
                    y += state[j].Y * ev[j];
                }

                result[i] = new PointD(x, y);
            }

            return result;
        }

        /// <summary>
        /// P�evede dan� stavov� vektor obsahuj�c� komponenty vlastn�ch vektor� na komponenty b�ze
        /// </summary>
        /// <param name="state">Stavov� vektor vyj�d�en� v rozvoji do b�ze</param>
        public PointVector EVToBasis(PointVector state) {
            int dim = this.basisIndex.Length;
            int numEV = this.NumEV;

            PointVector result = new PointVector(dim);

            for(int i = 0; i < numEV; i++) {
                double x = state[i].X;
                double y = state[i].Y;

                Vector ev = this.eigenVectors[i];

                for(int j = 0; j < dim; j++) {
                    result[j].X += x * ev[j];
                    result[j].Y += x * ev[j];
                }
            }

            return result;
        }

        /// <summary>
        /// Partition function
        /// </summary>
        /// <param name="T">Temperature</param>
        public double PartitionFunction(double T) {
            int numEV = this.NumEV;
            double result = 0.0;

            for(int i = 0; i < numEV; i++) 
                result += System.Math.Exp(-this.eigenValues[i] / T);

            return result;
        }

        /// <summary>
        /// Mean energy
        /// </summary>
        /// <param name="T">Temperature</param>
        public double MeanEnergy(double T) {
            int numEV = this.NumEV;
            double result = 0.0;

            for(int i = 0; i < numEV; i++)
                result += System.Math.Exp(-this.eigenValues[i] / T) * this.eigenValues[i];

            double pf = this.PartitionFunction(T);
            // Tady na to bacha!!!
            if(pf == 0.0)
                return 0.0;
            else
                return result / this.PartitionFunction(T);
        }

        /// <summary>
        /// Thermodynamical mean value of a Peres operator
        /// </summary>
        /// <param name="T">Temperature</param>
        /// <param name="type">Type of an operator</param>
        public double MeanOperator(double T, int type) {
            int numEV = this.NumEV;
            double result = 0.0;

            Vector p = this.parrentQuantumSystem.PeresInvariant(type);

            for(int i = 0; i < numEV; i++)
                result += System.Math.Exp(-this.eigenValues[i] / T) * p[i];

            return result / this.PartitionFunction(T);
        }

        /// <summary>
        /// Class for the mean energy bisection
        /// </summary>
        private class MeanEnergyBisection {
            private double e;
            private RealFunction function;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="e">Energy</param>
            public MeanEnergyBisection(RealFunction function, double e) {
                this.function = function;
                this.e = e;
            }

            public double BisectionFunction(double T) {
                return this.function(T) - e;
            }
        }

        /// <summary>
        /// Calculates a temperature using expression Tr(Ro H) = K
        /// </summary>
        /// <param name="e">Mean energy</param>
        /// <returns></returns>
        public double QuantumTemperature(double e) {
            double mine = 0.0;
            double maxe = this.eigenValues.Sum() / this.NumEV;

            if(e < mine || e > maxe) {
                return double.NaN;
                // Tady je taky RENONC!!!
                throw new SystemsException(Messages.EMBadMeanEnergy,
                    string.Format(Messages.EMBadMeanEnergyDetail, e, maxe));
            }

            MeanEnergyBisection mbf = new MeanEnergyBisection(this.MeanEnergy, e);
            Bisection b = new Bisection(mbf.BisectionFunction);
            return b.Solve(1E-3, 1E3);
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

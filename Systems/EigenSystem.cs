using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.DLLWrapper;
using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Obsahuje vlastní èísla a vlastní hodnoty
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

        // Tøída s parametry báze
        private BasisIndex basisIndex;

        /// <summary>
        /// Tøída s parametry báze
        /// </summary>
        public BasisIndex BasisIndex { get { return this.basisIndex; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="system">Kvantový systém (vlastník)</param>
        public EigenSystem(IQuantumSystem system) {
            this.parrentQuantumSystem = system;
        }

        /// <summary>
        /// Pokud systém nebyl vypoèítán, vyhodí výjimku
        /// </summary>
        private void CheckComputed() {
            if(!this.isComputed)
                throw new SystemsException(Messages.EMNotComputed);
        }

        /// <summary>
        /// Vlastní hodnoty
        /// </summary>
        public Vector GetEigenValues() {
            this.CheckComputed();
            return this.eigenValues;
        }

        /// <summary>
        /// Vlastní vektor
        /// </summary>
        /// <param name="i">Index vektoru</param>
        public Vector GetEigenVector(int i) {
            this.CheckComputed();
            if(this.eigenVectors.Length == 0)
                throw new SystemsException(Messages.EMNoEigenVectors);

            return this.eigenVectors[i];
        }

        /// <summary>
        /// Poèet vlastních vektorù
        /// </summary>
        public int NumEV { get { this.CheckComputed();  return this.eigenVectors.Length; } }

        /// <summary>
        /// Nastaví kvantový systém kvùli výpoètùm
        /// </summary>
        /// <param name="system">Kvantový systém (vlastník)</param>
        public void SetParrentQuantumSystem(IQuantumSystem system) {
            this.parrentQuantumSystem = system;
        }

        /// <summary>
        /// Kvùli kompatibilitì s pøedchozím ukládáním
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public void SetBasisParams(Vector basisParams) {
            this.basisIndex = this.parrentQuantumSystem.CreateBasisIndex(basisParams);
        }

        /// <summary>
        /// Provede výpoèet (diagonalizaci)
        /// </summary>
        /// <param name="basisParams">Parametry báze (nejvyšší energie bázových funkcí)</param>
        /// <param name="ev">True, pokud budeme poèítat i vlastní vektory</param>
        /// <param name="numEV">Poèet vlastních hodnot, menší èi rovné 0 vypoèítá všechny</param>
        /// <param name="writer">Writer</param>
        /// <param name="method">Metoda výpoètu</param>
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
        /// Napoèítá Hamiltonovu matici
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        /// <param name="writer">Writer</param>
        public Matrix HamiltonianMatrix(Vector basisParams, IOutputWriter writer) {
            if(writer != null) {
                writer.WriteLine(Messages.MHMCalculation);
            }

            return this.parrentQuantumSystem.HamiltonianMatrix(this.parrentQuantumSystem.CreateBasisIndex(basisParams), writer);
        }

        /// <summary>
        /// Napoèítá stopu Hamiltonovy matice
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public double HamiltonianMatrixTrace(Vector basisParams) {
            return this.parrentQuantumSystem.HamiltonianMatrixTrace(this.parrentQuantumSystem.CreateBasisIndex(basisParams));
        }

        /// <summary>
        /// Napoèítá rozmìry Hamiltonovy matice
        /// </summary>
        /// <param name="basisParams">Parametry báze</param>
        public int HamiltonianMatrixSize(Vector basisParams) {
            return this.parrentQuantumSystem.CreateBasisIndex(basisParams).Length;
        }
        
        /// <summary>
        /// Provede výpoèet (diagonalizaci)
        /// </summary>
        /// <param name="maxn">Nejvyšší energie bázových funkcí</param>
        /// <param name="ev">True, pokud budeme poèítat i vlastní vektory</param>
        /// <param name="numEV">Poèet vlastních hodnot, menší èi rovné 0 vypoèítá všechny</param>
        /// <param name="writer">Writer</param>
        /// <param name="method">Metoda výpoètu</param>
        public void Diagonalize(int maxn, bool ev, int numEV, IOutputWriter writer, ComputeMethod method) {
            Vector basisParams = new Vector(1);
            basisParams[0] = maxn;

            this.Diagonalize(basisParams, ev, numEV, writer, method);
        }

        /// <summary>
        /// Provede výpoèet (diagonalizaci)
        /// </summary>
        /// <param name="maxn">Nejvyšší energie bázových funkcí</param>
        /// <param name="ev">True, pokud budeme poèítat i vlastní vektory</param>
        /// <param name="numEV">Poèet vlastních hodnot, menší èi rovné 0 vypoèítá všechny</param>
        /// <param name="writer">Writer</param>
        public void Diagonalize(int maxn, bool ev, int numEV, IOutputWriter writer) {
            this.Diagonalize(maxn, ev, numEV, writer, ComputeMethod.LAPACKBand);
        }

        /// <summary>
        /// Diagonalizace užitím Jacobi metody
        /// </summary>
        /// <param name="matrix">Symetrická matice</param>
        /// <param name="ev">True, pokud budeme poèítat i vlastní vektory</param>
        /// <param name="numev">Poèet vlastních hodnot, menší èi rovné 0 vypoèítá všechny</param>
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
        /// Diagonalizace užitím LAPACK knihovny
        /// </summary>
        /// <param name="matrix">Symetrická pásová matice</param>
        /// <param name="ev">True, pokud budeme poèítat i vlastní vektory</param>
        /// <param name="numev">Poèet vlastních hodnot, menší èi rovné 0 vypoèítá všechny</param>
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
        /// Na základì kvantových èísel vrátí index daného bázového vektoru
        /// </summary>
        /// <param name="bi">Kvantová èísla</param>
        public int BasisQuantumNumber(Vector bi) {
            return this.basisIndex[bi];
        }

        /// <summary>
        /// Na základì indexu bázového vektoru vrátí jeho vlastní èísla
        /// </summary>
        /// <param name="i">Index bázového vektoru</param>
        public Vector BasisQuantumNumber(int i) {
            return this.basisIndex[i];
        }

        /// <summary>
        /// Vrátí bázový vektor
        /// </summary>
        /// <param name="i">Index bázového vektoru</param>
        public Vector BasisVector(int i) {
            Vector result = new Vector(this.basisIndex.Length);
            result[i] = 1.0;
            return result;
        }

        /// <summary>
        /// Pøevede daný stavový vektor obsahující komponenty báze na komponenty vlastních vektorù
        /// </summary>
        /// <param name="state">Stavový vektor vyjádøený v rozvoji do báze</param>
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
        /// Pøevede daný stavový vektor obsahující komponenty vlastních vektorù na komponenty báze
        /// </summary>
        /// <param name="state">Stavový vektor vyjádøený v rozvoji do báze</param>
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

            if(e < mine || e > maxe)
                throw new SystemsException(Messages.EMBadMeanEnergy,
                    string.Format(Messages.EMBadMeanEnergyDetail, e, maxe));

            MeanEnergyBisection mbf = new MeanEnergyBisection(this.MeanEnergy, e);
            Bisection b = new Bisection(mbf.BisectionFunction);
            return b.Solve(1E-3, 1E3);
        }

        /// <summary>
        /// Konstruktor pro naètení staré verze dat (6 a starší)
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
        /// Uloží výsledky do souboru
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
        /// Naète výsledky ze souboru
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

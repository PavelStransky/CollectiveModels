using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Quantum GCM calculated in the basis of the 5D linear harmonic oscilator
    /// (nonrotating case)
    /// </summary>
    public class LHOQuantumGCMI5D: LHOQuantumGCMI {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMI5D(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        /// <summary>
        /// Konstruktor pro Import
        /// </summary>
        /// <param name="import">Import</param>
        public LHOQuantumGCMI5D(Core.Import import) : base(import) { }

        protected override int MaximalNumNodes { get { return (this.eigenSystem.BasisIndex as LHO5DIndex).MaxMu; } }
        protected override double MaximalRange { get { return System.Math.Sqrt(this.Hbar * this.Omega * (this.eigenSystem.BasisIndex as LHO5DIndex).MaxE / this.A0); } }
        protected override double PsiRange(double range) {
            return this.Psi5D(range, (this.eigenSystem.BasisIndex as LHO5DIndex).MaxL, (this.eigenSystem.BasisIndex as LHO5DIndex).MaxMu);
        }

        /// <summary>
        /// Vytvo�� instanci t��dy LHO5DIndex
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public override BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LHO5DIndexI(basisParams);
        }

        /// <summary>
        /// Stopa Hamiltonovy matice
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        public override double HamiltonianMatrixTrace(BasisIndex basisIndex) {
            return this.HamiltonianSBMatrix(basisIndex, null, true).Trace();
        }

        /// <summary>
        /// Vypo��t� Hamiltonovu matici do tvaru p�sov� matice
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        public override SymmetricBandMatrix HamiltonianSBMatrix(BasisIndex basisIndex, IOutputWriter writer) {
            return this.HamiltonianSBMatrix(basisIndex, writer, false);
        }

        /// <summary>
        /// Vypo��t� Hamiltonovu matici do tvaru p�sov� matice
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        /// <param name="trace">Calculates only trace of the matrix</param>
        public virtual SymmetricBandMatrix HamiltonianSBMatrix(BasisIndex basisIndex, IOutputWriter writer, bool trace) {
            LHO5DIndexI index = basisIndex as LHO5DIndexI;

            int maxE = index.MaxE;
            int numSteps = index.NumSteps;

            DateTime startTime = DateTime.Now;

            if(writer != null) {
                writer.WriteLine("Hamiltonova matice");
                writer.Indent(1);
                writer.WriteLine(string.Format("P�ipravuji cache potenci�lu ({0})...", numSteps));
            }

            double omega = this.Omega;
            double range = this.GetRange();

            // Cache psi hodnot (Bazove vlnove funkce)
            DiscreteInterval interval = new DiscreteInterval(0.0, range, numSteps);

            double step = interval.Step;

            // Cache hodnot potencialu
            double[] vCache1 = new double[numSteps];
            double[] vCache2 = new double[numSteps];
            for(int sb = 0; sb < numSteps; sb++) {
                double beta = interval.GetX(sb);
                double beta2 = beta * beta;
                vCache1[sb] = beta2 * beta2 * beta2 * ((this.A - this.A0) + this.C * beta2);
                vCache2[sb] = this.B * beta2 * beta2 * beta2 * beta;
            }

            int length = index.Length;
            int bandWidth = trace ? 0 : index.BandWidth;
            SymmetricBandMatrix m = new SymmetricBandMatrix(length, bandWidth);

            int blockSize = bandWidth + 1;
            int blockNum = length / blockSize + 1;

            if(writer != null) {
                writer.WriteLine(string.Format("P��prava p�sov� matice H ({0} x {1})", length, maxE - 2));
                writer.WriteLine(string.Format("Po�et blok�: {0}, velikost bloku: {1}", blockNum, blockSize));
            }

            DateTime startTime1 = DateTime.Now;

            BasisCache cache2 = new BasisCache(interval, 0, System.Math.Min(blockSize, index.Length), this.Psi);
            BasisCache cache1 = cache2;

            for(int k = 0; k < blockNum; k++) {
                int i0 = k * blockSize;
                int i1 = System.Math.Min((k + 1) * blockSize, index.Length);
                int i2 = System.Math.Min((k + 2) * blockSize, index.Length);

                if(writer != null) {
                    writer.Write(k);
                    writer.Write(" D");
                }

                // Diagonal block
                for(int i = i0; i < i1; i++) {
                    int li = index.L[i];
                    int mui = index.Mu[i];

                    for(int j = i; j < i1; j++) {
                        int lj = index.L[j];
                        int muj = index.Mu[j];

                        // Selection rule
                        if(mui != muj && System.Math.Abs(mui - muj) != 1)
                            continue;

                        // If only trace
                        if(trace && i != j)
                            continue;

                        double sum = 0;

                        double[] vCache = vCache2;
                        if(mui == muj)
                            vCache = vCache1;

                        for(int sb = 0; sb < numSteps; sb++)
                            sum += cache1[i, sb] * vCache[sb] * cache1[j, sb];

                        sum *= step;

                        if(mui != muj) {
                            int mu = System.Math.Max(mui, muj);
                            sum *= mu / System.Math.Sqrt((2.0 * mu - 1.0) * (2.0 * mu + 1.0));
                        }

                        if(mui == muj && li == lj)
                            sum += this.Hbar * omega * (2.5 + li + li + 3 * mui);

                        // Ji� je symetrick�
                        m[i, j] = sum;
                    }
                }

                if(writer != null)
                    writer.Write('C');

                cache2 = new BasisCache(interval, i1, i2, this.Psi);

                // If not only trace
                if(!trace) {
                    if(writer != null)
                        writer.Write("N ");

                    // Nediagon�ln� blok
                    for(int i = i0; i < i1; i++) {
                        int li = index.L[i];
                        int mui = index.Mu[i];

                        for(int j = i1; j < i2; j++) {
                            int lj = index.L[j];
                            int muj = index.Mu[j];

                            // Selection rule
                            if(mui != muj && System.Math.Abs(mui - muj) != 1)
                                continue;

                            double sum = 0;

                            double[] vCache = vCache2;
                            if(mui == muj)
                                vCache = vCache1;

                            for(int sb = 0; sb < numSteps; sb++)
                                sum += cache1[i, sb] * vCache[sb] * cache2[j, sb];

                            sum *= step;

                            if(mui != muj) {
                                int mu = System.Math.Max(mui, muj);
                                sum *= mu / System.Math.Sqrt((2.0 * mu - 1.0) * (2.0 * mu + 1.0));
                            }

                            // Ji� je symetrick�
                            m[i, j] = sum;
                        }
                    }

                    if(writer != null) {
                        writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime1));
                        startTime1 = DateTime.Now;
                    }
                }

                cache1 = cache2;
            }

            if(writer != null) {
                writer.Indent(-1);
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));
            }

            return m;
        }

        /// <summary>
        /// Radi�ln� ��st vlnov� funkce
        /// </summary>
        /// <param name="i">Index (kvantov� ��sla zjist�me podle uchovan� cache index�)</param>
        /// <param name="x">Sou�adnice</param>
        protected double Psi(double x, int i) {
            LHO5DIndexI index = this.eigenSystem.BasisIndex as LHO5DIndexI;
            return this.Psi5D(x, index.L[i], index.Mu[i]);
        }

        /// <summary>
        /// Angular part of the wave function
        /// </summary>
        /// <param name="g">Angle gamma</param>
        /// <param name="i">Index</param>
        protected double Phi(double g, int i) {
            return this.Phi5D(g, (this.eigenSystem.BasisIndex as LHO5DIndexI).Mu[i]);
        }

        /// <summary>
        /// Vlnov� funkce ve 2D
        /// </summary>
        protected override double PsiBG(double beta, double gamma, int n) {
            LHO5DIndexI index = this.eigenSystem.BasisIndex as LHO5DIndexI;
            
            int l = index.L[n];
            int mu = index.Mu[n];

            double beta3 = beta * beta * beta;
            double sin3g = System.Math.Abs(System.Math.Sin(3.0 * gamma));

            return beta3 * sin3g * this.Psi5D(beta, l, mu) * this.Phi5D(gamma, mu);
        }

        /// <summary>
        /// �asov� st�edn� hodnota druh�ho integr�lu - Casimir�v oper�tor SO(5)
        /// </summary>
        protected override double PeresInvariantCoef(int n) {
            int lambda = 3 * (this.eigenSystem.BasisIndex as LHO5DIndexI).Mu[n];
            return lambda * (lambda + 3) * this.Hbar * this.Hbar;
        }
    }
}
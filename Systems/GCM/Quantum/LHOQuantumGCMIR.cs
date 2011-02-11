using System;
using System.IO;

using PavelStransky.Math;
using PavelStransky.Core;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Systems {
    /// <summary>
    /// Kvantov� GCM v b�zi 2D line�rn�ho harmonick�ho oscil�toru
    /// </summary>
    public class LHOQuantumGCMIR: LHOQuantumGCMI {
        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected LHOQuantumGCMIR() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMIR(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        /// <summary>
        /// Konstruktor pro Import
        /// </summary>
        /// <param name="import">Import</param>
        public LHOQuantumGCMIR(Import import) : base(import) { }

        protected override int MaximalNumNodes { get { return (this.eigenSystem.BasisIndex as LHOPolarIndexI).MaxM; } }
        protected override double MaximalRange { get { return System.Math.Sqrt(this.Hbar * this.Omega * (this.eigenSystem.BasisIndex as LHOPolarIndexI).MaxE / this.A0); } }
        protected override double PsiRange(double range) {
            LHOPolarIndexI index = this.eigenSystem.BasisIndex as LHOPolarIndexI;
            return this.Psi2D(range, index.MaxN, index.MaxM);
        }

        /// <summary>
        /// Vytvo�� instanci t��dy LHO5DIndex
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public override BasisIndex CreateBasisIndex(Vector basisParams) {
            return new LHOPolarIndexI(basisParams);
        }

        /// <summary>
        /// Napo��t� Hamiltonovu matici v dan� b�zi
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        public void HamiltonianMatrixOld(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            LHOPolarIndexI index = basisIndex as LHOPolarIndexI;

            int maxE = index.MaxE;
            int numSteps = index.NumSteps;

            if(writer != null) {
                writer.WriteLine(string.Format("Maxim�ln� (n, m) = ({0}, {1})", index.MaxN, index.MaxM));
                writer.WriteLine(string.Format("Velikost b�ze: {0}", index.Length));
                writer.WriteLine(string.Format("P�ipravuji cache ({0})...", numSteps));
            }

            double omega = this.Omega;
            double range = this.GetRange();

            // Cache psi hodnot (Bazove vlnove funkce)
            BasisCache psiCache = new BasisCache(new DiscreteInterval(0.0, range, numSteps), index.Length, this.Psi);

            int[] psiCacheLowerLimits = psiCache.GetLowerLimits(epsilon);
            int[] psiCacheUpperLimits = psiCache.GetUpperLimits(epsilon);

            double step = psiCache.Step;

            // Cache hodnot potencialu
            double[] vCache1 = new double[numSteps];
            double[] vCache2 = new double[numSteps];
            for(int sb = 0; sb < numSteps; sb++) {
                double beta = psiCache.GetX(sb);
                double beta2 = beta * beta;
                vCache1[sb] = beta2 * beta * ((this.A - this.A0) + this.C * beta2);
                vCache2[sb] = 0.5 * this.B * beta2 * beta2;
            }

            int length = index.Length;

            if(writer != null)
                writer.WriteLine(string.Format("P��prava H ({0} x {1})", length, length));

            for(int i = 0; i < length; i++) {
                int ni = index.N[i];
                int mi = index.M[i];

                for(int j = i; j < length; j++) {
                    int nj = index.N[j];
                    int mj = index.M[j];

                    // V�b�rov� pravidlo
                    if(mi != mj && System.Math.Abs(mi - mj) != 3)
                        continue;

                    double sum = 0;

                    double[] vCache = vCache2;
                    if(mi == mj)
                        vCache = vCache1;

                    for(int sb = 0; sb < numSteps; sb++)
                        sum += psiCache[i, sb] * vCache[sb] * psiCache[j, sb];

                    sum *= step;

                    if(mi == mj && ni == nj)
                        sum += this.Hbar * omega * (1.0 + ni + ni + System.Math.Abs(mi));

                    matrix[i, j] = sum;
                    matrix[j, i] = sum;
                }

                // V�pis te�ky na konzoli
                if(writer != null) {
                    if(i != 0 && index.M[i - 1] != index.M[i])
                        writer.Write(".");
                }
            }
        }

        /// <summary>
        /// Vr�t� indexy vlastn�ch ��sel, kter� maj� sudou paritu
        /// </summary>
        public Vector Parity() {
            LHOPolarIndexI index = this.eigenSystem.BasisIndex as LHOPolarIndexI;

            int length = index.Length;
            int num = this.eigenSystem.NumEV;

            Vector result = new Vector(num);

            for(int i = 0; i < length; i++) {
                int ni = index.N[i];
                int mi = index.M[i];

                if(mi > 0)
                    continue;

                for(int j = i; j < length; j++) {
                    int nj = index.N[j];
                    int mj = index.M[j];

                    if(ni == nj && mi == -mj) {
                        for(int k = 0; k < num; k++)
                            result[k] += System.Math.Abs(this.eigenSystem.GetEigenVector(k)[i] + this.eigenSystem.GetEigenVector(k)[j]);
                        break;
                    }
                }
            }

            for(int k = 0; k < num; k++)
                if(result[k] > 1.0)
                    result[k] = 1.0;
                else
                    result[k] = -1.0;

            return result;
        }

        /// Vypo��t� Hamiltonovu matici 
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        public override void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            this.HamiltonianMatrix(matrix, basisIndex, writer, false);
        }

        /// <summary>
        /// Vypo��t� Hamiltonovu matici 
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        /// <param name="trace">Calculates only trace of the matrix</param>
        protected virtual void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer, bool trace) {
            LHOPolarIndexI index = basisIndex as LHOPolarIndexI;

            int maxE = index.MaxE; ;
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
                vCache1[sb] = beta2 * beta * ((this.A - this.A0) + this.C * beta2);
                vCache2[sb] = 0.5 * this.B * beta2 * beta2;
            }

            int length = index.Length;
            int bandWidth = index.BandWidth;

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

                // Diagon�ln� blok
                for(int i = i0; i < i1; i++) {
                    int ni = index.N[i];
                    int mi = index.M[i];

                    for(int j = i; j < i1; j++) {
                        int nj = index.N[j];
                        int mj = index.M[j];

                        // V�b�rov� pravidlo
                        if(mi != mj && System.Math.Abs(mi - mj) != 3)
                            continue;

                        // Only trace
                        if(trace && i != j)
                            continue;

                        double sum = 0;

                        double[] vCache = vCache2;
                        if(mi == mj)
                            vCache = vCache1;

                        for(int sb = 0; sb < numSteps; sb++)
                            sum += cache1[i, sb] * vCache[sb] * cache1[j, sb];

                        sum *= step;

                        if(mi == mj && ni == nj)
                            sum += this.Hbar * omega * (1.0 + ni + ni + System.Math.Abs(mi));

                        // Ji� je symetrick�
                        matrix[i, j] = sum;
                    }
                }

                if(writer != null)
                    writer.Write('C');

                cache2 = new BasisCache(interval, i1, i2, this.Psi);

                if(!trace) {
                    if(writer != null)
                        writer.Write("N ");

                    // Nediagon�ln� blok
                    for(int i = i0; i < i1; i++) {
                        int ni = index.N[i];
                        int mi = index.M[i];

                        for(int j = i1; j < i2; j++) {
                            int nj = index.N[j];
                            int mj = index.M[j];

                            // V�b�rov� pravidlo
                            if(mi != mj && System.Math.Abs(mi - mj) != 3)
                                continue;

                            double sum = 0;

                            double[] vCache = vCache2;
                            if(mi == mj)
                                vCache = vCache1;

                            for(int sb = 0; sb < numSteps; sb++)
                                sum += cache1[i, sb] * vCache[sb] * cache2[j, sb];

                            sum *= step;

                            // Ji� je symetrick�
                            matrix[i, j] = sum;
                        }
                    }
                }

                cache1 = cache2;

                if(writer != null) {
                    writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime1));
                    startTime1 = DateTime.Now;
                }
            }

            if(writer != null) {
                writer.Indent(-1);
                writer.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));
            }
        }

        /// <summary>
        /// Radi�ln� ��st vlnov� funkce
        /// </summary>
        /// <param name="i">Index (kvantov� ��sla zjist�me podle uchovan� cache index�)</param>
        /// <param name="x">Sou�adnice</param>
        protected double Psi(double x, int i) {
            LHOPolarIndexI index = this.eigenSystem.BasisIndex as LHOPolarIndexI;
            return this.Psi2D(x, index.N[i], index.M[i]);
        }

        protected override double PsiXY(double x, double y, int n) {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// �asov� st�edn� hodnota druh�ho integr�lu - Casimir�v oper�tor SO(2) hbar^2 * (d / d phi)^2
        /// </summary>
        protected override double PeresInvariantCoef(int n) {
            double d = (this.eigenSystem.BasisIndex as LHOPolarIndexI).M[n] * this.Hbar;
            return d * d;
        }
    }
}
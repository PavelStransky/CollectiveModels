using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.DLLWrapper;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    public class QuantumEP: ExtensiblePendulum, IQuantumSystem {
        // Syst�m s vlastn�mi hodnotami
        private EigenSystem eigenSystem;

        // Parametr b�ze harmonick�ho oscil�toru (typicky z rozmez� 0...1)
        private double a;

        // Hodnota Planckovy konstanty
        private double hbar;

        /// <summary>
        /// Planckova konstanta [Js]
        /// </summary>
        public double Hbar { get { return this.hbar; } }

        /// <summary>
        /// Syst�m vlastn�ch hodnot
        /// </summary>
        public EigenSystem EigenSystem { get { return this.eigenSystem; } }

        /// <summary>
        /// Pr�zdn� konstruktor
        /// </summary>
        protected QuantumEP() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="nu">Parametr kyvadla</param>
        /// <param name="a">Parametr b�ze harmonick�ho oscil�toru</param>
        /// <param name="hbar">Planckova konstanta</param>
        public QuantumEP(double nu, double a, double hbar)
            : base(nu) {
            this.a = a;
            this.hbar = hbar;
            this.eigenSystem = new EigenSystem(this);
        }

        /// <summary>
        /// Vytvo�� instanci t��dy s parametry b�ze
        /// </summary>
        /// <param name="basisParams">Parametry b�ze</param>
        public BasisIndex CreateBasisIndex(Vector basisParams) {
            return new EPBasisIndex(basisParams);
        }

        /// <summary>
        /// Napln� Hamiltonovu matici
        /// </summary>
        /// <param name="basisIndex">Parametry b�ze</param>
        /// <param name="writer">Writer</param>
        public void HamiltonianMatrix(IMatrix matrix, BasisIndex basisIndex, IOutputWriter writer) {
            EPBasisIndex index = basisIndex as EPBasisIndex;

            int dim = index.Length;

            double omega = System.Math.Sqrt(this.a);
            double alpha = System.Math.Sqrt(this.a) / this.hbar;
            double c1 = (1 - this.a) / (2.0 * alpha);
            double c2 = this.Nu / (2.0 * System.Math.Sqrt(alpha));
            double c3 = System.Math.PI / (4.0 * System.Math.Sqrt(alpha));

            for(int i = 0; i < dim; i++) {
                int n = index.N[i];
                int m = index.M[i];
                int l = System.Math.Abs(m);

                // rho element
                for(int j = i; j < dim; j++) {
                    if(index.M[j] != m)
                        continue;

                    int n2 = index.N[j];

                    double norm = 0.5 * (SpecialFunctions.FactorialILog(n) + SpecialFunctions.FactorialILog(n2)
                        - SpecialFunctions.FactorialILog(n + l) - SpecialFunctions.FactorialILog(n2 + l));

                    double sum = 0.0;

                    int minn = System.Math.Min(n, n2);
                    for(int s = 0; s <= minn; s++) {
                        int sign = 1;
                        if(s - n + 1 < 0 && (n - s) % 2 == 0)
                            sign = -sign;
                        if(s - n2 + 1 < 0 && (n2 - s) % 2 == 0)
                            sign = -sign;

                        sum += sign * System.Math.Exp(norm + SpecialFunctions.HalfFactorialILog(l + s + 1) 
                            - SpecialFunctions.FactorialILog(s)
                            - SpecialFunctions.FactorialILog(n - s) - SpecialFunctions.FactorialILog(n2 - s)
                            - SpecialFunctions.HalfFactorialILog(s - n + 1) - SpecialFunctions.HalfFactorialILog(s - n2 + 1));
                    }

                    matrix[i, j] = (n + n2) % 2 == 0 ? (-c3 * sum) : (c3 * sum);
                    matrix[j, i] = matrix[i, j];
                }

                // Diagonal element
                matrix[i, i] += (this.hbar * omega + c1) * (2 * n + System.Math.Abs(m) + 1) + 0.5;

                // rho^2 element
                int ip = index[n + 1, m];
                if(ip > 0) {
                    matrix[i, ip] -= c1 * System.Math.Sqrt((n + 1) * (n + System.Math.Abs(m) + 1));
                    matrix[ip, i] = matrix[i, ip];
                }

                // different m elements
                if(index.Positive)
                    ip = index[n, System.Math.Abs(m - 1)];
                else if(m > 0)
                    ip = index[n, m - 1];
                else
                    ip = index[n, m + 1];
                if(ip > 0) {
                    matrix[i, ip] = -c2 * System.Math.Sqrt(n + System.Math.Max(l, System.Math.Abs(index.M[ip])));
                    matrix[ip, i] = matrix[i, ip];
                }
                if(m > 0)
                    ip = index[n + 1, m - 1];
                else
                    ip = index[n + 1, m + 1];
                if(ip > 0 && m != 0) {
                    matrix[i, ip] = c2 * System.Math.Sqrt(n + 1);
                    matrix[ip, i] = matrix[i, ip];
                }
            }
        }

        /// <summary>
        /// Peres�v invariant
        /// </summary>
        /// <param name="type">Typ (0 - H0, 1 - L1, 3 - L1^2)</param>
        public Vector PeresInvariant(int type) {
            EPBasisIndex index = this.eigenSystem.BasisIndex as EPBasisIndex;

            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenSystem.GetEigenVector(i);
                int length = ev.Length;

                for(int j = 0; j < length; j++) {
                    double koef = 0.0;
                    switch(type) {
                        case 1:
                            koef = index.M[j];
                            break;
                        case 3:
                            koef = index.M[j]; koef *= koef;
                            break;
                    }

                    result[i] += ev[j] * ev[j] * koef;
                }
            }

            return result;
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
        public override void Export(Export export) {
            base.Export(export);

            IEParam param = new IEParam();
            param.Add(this.eigenSystem, "EigenSystem");
            param.Add(this.a, "Quantization");
            param.Add(this.hbar, "PlackConstant");
            param.Export(export);
        }

        /// <summary>
        /// Na�te v�sledky ze souboru
        /// </summary>
        /// <param name="import">Import</param>
        public QuantumEP(Core.Import import)
            : base(import) {
            IEParam param = new IEParam(import);
            this.eigenSystem = (EigenSystem)param.Get();
            this.a = (double)param.Get(1.0);
            this.hbar = (double)param.Get(1.0);
            this.eigenSystem.SetParrentQuantumSystem(this);
        }

        #endregion
    }
}
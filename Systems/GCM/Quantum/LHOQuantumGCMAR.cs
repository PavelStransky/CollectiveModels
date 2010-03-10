using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Systems {
    public abstract class LHOQuantumGCMAR: LHOQuantumGCM {
        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected LHOQuantumGCMAR() { }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="a0">Parametr LHO</param>
        /// <param name="a">Parametr A</param>
        /// <param name="b">Parametr B</param>
        /// <param name="c">Parametr C</param>
        /// <param name="k">Parametr D</param>
        /// <param name="hbar">Planckova konstanta</param>
        public LHOQuantumGCMAR(double a, double b, double c, double k, double a0, double hbar)
            : base(a, b, c, k, a0, hbar) { }

        protected LHOQuantumGCMAR(Core.Import import) : base(import) { }

        /// <summary>
        /// Èasová støední hodnota druhého integrálu - Casimirùv operátor SO(2) hbar^2 * (d / d phi)^2
        /// </summary>
        protected override double PeresInvariantCoef(int n) {
            double d = (this.eigenSystem.BasisIndex as LHOPolarIndex).M[n] * this.Hbar;
            return d * d;
        }

        /// <summary>
        /// Druhý invariant pro operátor H0
        /// </summary>
        protected override Vector PeresInvariantHPrime() {
            LHOPolarIndex index = this.eigenSystem.BasisIndex as LHOPolarIndex;

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;
            double alpha32 = alpha * this.s;

            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                Vector ev = this.eigenSystem.GetEigenVector(i);
                int length = ev.Length;

                double sum = 0.0;

                for(int j = 0; j < length; j++) {
                    int n = index.N[j];
                    int m = index.M[j];

                    int l = System.Math.Abs(m);

                    sum += ev[j] * ev[j] *
                            (this.Hbar * omega * (2.0 * n + l + 1.0)
                                + (this.A - this.A0) * (2.0 * n + l + 1.0) / alpha
                                + this.C * (n * (n - 1.0) + (n + l + 1.0) * (5.0 * n + l + 2.0)) / alpha2);

                    if(j < length - 1 && index.N[j + 1] == n + 1)
                        sum -= 2.0 * ev[j] * ev[j + 1] *
                                ((this.A - this.A0) * System.Math.Sqrt((n + 1.0) * (n + l + 1.0)) / alpha
                                    + 2.0 * this.C * System.Math.Sqrt((n + 1.0) * (n + l + 1.0)) * (2.0 * n + l + 2.0) / alpha2);

                    if(j < length - 2 && index.N[j + 2] == n + 2) 
                        sum += 2.0 * ev[j] * ev[j + 2] * this.C * System.Math.Sqrt((n + l + 2.0) * (n + l + 1.0) * (n + 2.0) * (n + 1.0)) / alpha2;
                }

                result[i] = sum;
            }

            return result;
        }

        /// <summary>
        /// Druhý invariant pro operátor H v oscilátorové bázi
        /// </summary>
        protected override Vector PeresInvariantHOscillator() {
            LHOPolarIndex index = this.eigenSystem.BasisIndex as LHOPolarIndex;

            double omega = this.Omega;
            double alpha = this.s * this.s;
            double alpha2 = alpha * alpha;
            double alpha32 = alpha * this.s;

            int count = this.eigenSystem.NumEV;
            Vector result = new Vector(count);

            for(int i = 0; i < count; i++) {
                int n = index.N[i];
                int m = index.M[i];

                int l = System.Math.Abs(m);

                result[i] += this.Hbar * omega * (2 * n + l + 1);
                result[i] += (this.A - this.A0) * (2 * n + l + 1) / alpha;
                result[i] += this.C * (n * (n - 1) + (n + l + 1) * (5 * n + l + 2)) / alpha2;
            }

            return result;
        }

        /// <summary>
        /// Radiální èást vlnové funkce
        /// </summary>
        /// <param name="n">Hlavní kvantové èíslo</param>
        /// <param name="m">Spin</param>
        /// <param name="r">Radiální souøadnice</param>
        public double Psi(double r, int n, int m) {
            double xi2 = this.s * r; xi2 *= xi2;
            m = System.Math.Abs(m);

            double normLog = 0.5 * (System.Math.Log(2.0) + SpecialFunctions.FactorialILog(n) - SpecialFunctions.FactorialILog(n + m)) + (m + 1) * System.Math.Log(this.s);
            double l = 0.0;
            double e = 0.0;
            SpecialFunctions.Laguerre(out l, out e, xi2, n, m);

            if(l == 0.0 || r == 0.0)
                return 0.0;

            double lLog = System.Math.Log(System.Math.Abs(l));
            double result = normLog + m * System.Math.Log(r) - xi2 / 2.0 + lLog + e;
            result = l < 0.0 ? -System.Math.Exp(result) : System.Math.Exp(result);

            return result;
        }

        /// <summary>
        /// Radiální èást vlnové funkce
        /// </summary>
        /// <param name="i">Index (kvantová èísla zjistíme podle uchované cache indexù)</param>
        /// <param name="x">Souøadnice</param>
        protected double Psi(double x, int i) {
            LHOPolarIndex index = this.eigenSystem.BasisIndex as LHOPolarIndex;
            return this.Psi(x, index.N[i], index.M[i]);
        }
    }
}

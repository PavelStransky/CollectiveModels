using System;
using System.Collections;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.GCM {
    /// <summary>
    /// Základní tøída GCM modelu, obsahující a obsluhující základní parametry
    /// </summary>
    public class GCMParameters {
        // Parametry GCM
        private double a, b, c, k;

        public double A { get { return this.a; } set { this.a = value; } }
        public double B { get { return this.b; } set { this.b = value; } }
        public double C { get { return this.c; } set { this.c = value; } }
        public double K { get { return this.k; } set { this.k = value; } }

        public double C2 { get { return this.a * System.Math.Sqrt(5.0); } set { this.a = value / System.Math.Sqrt(5.0); } }
        public double C3 { get { return -this.b * System.Math.Sqrt(35.0 / 2.0); } set { this.b = -value * System.Math.Sqrt(2.0 / 35.0); } }
        public double C4 { get { return this.c * 5.0; } set { this.c = value / 5.0; } }
        public double B2 { get { return this.k * System.Math.Sqrt(5.0) / 2.0; } set { this.k = value * 2.0 / System.Math.Sqrt(5.0); } }

        public double Invariant { get { return this.A * this.C / (this.B * this.B); } }

        public double Dp { get { return 112.0 / 9.0 / System.Math.Sqrt(5.0) * this.C2 * this.C4 / (this.C3 * this.C3); } }
        public double Ep { get { return this.C3 / this.C4; } }
        public double Fp { get { return System.Math.Pow(this.C3, 4) / System.Math.Pow(this.C4, 3); } }

        /// <summary>
        /// Strukturní konstanta (pouze pro NonGamma-soft case)
        /// </summary>
        public double Sp { get { return 1.0 / (this.B2 * this.Ep * this.Ep * this.Fp); } }

        /// <summary>
        /// Strukturní konstanta pro gamma-soft pøípad
        /// </summary>
        public double SGammaSoft { get { return -this.B2 * System.Math.Pow(this.C3, 3) / System.Math.Pow(this.C4, 2); } }

        public bool IsGammaSoft { get { return this.B == 0; } }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        protected GCMParameters() { }

        /// <summary>
        /// Kostruktor GCM modelu
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <param name="c">C</param>
        /// <param name="k">D</param>
        public GCMParameters(double a, double b, double c, double k) {
            this.a = a;
            this.b = b;
            this.c = c;
            this.k = k;

            if(!(this.c > 0 || (this.a > 0 && this.b == 0.0 && this.c == 0.0)))
                throw new Exception("Program umí poèítat pouze pøípady s C > 0");
        }
   }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Matrix elements of the general Harmonic Oscillator
    /// !!!DOESN'T WORK!!!
    /// </summary>
    public class HOMatrixElement {
        public double alpha;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="alpha">Coefficient of the Harmonic oscillator</param>
        public HOMatrixElement(double alpha) {
            this.alpha = alpha;
        }

        /// <summary>
        /// Matrix element of the 2D Harmonic oscillator
        /// </summary>
        /// <param name="n1">Radial quantum number</param>
        /// <param name="m1">Orbital quantum number</param>
        /// <param name="n2">Radial quantum number</param>
        /// <param name="m2">Orbital quantum number</param>
        /// <param name="a">Power of r</param>
        /// <param name="b">Multiplier of phi</param>
        public double HO2D(int n1, int n2, int m1, int m2, int a, int b) {
            int l1 = System.Math.Abs(m1);
            int l2 = System.Math.Abs(m2);
            int dl = l2 - l1;

            if(m1 + b != m2 && m2 + b != m1)
                return 0;

            double norm = System.Math.Exp(0.5 * (SpecialFunctions.FactorialILog(n1) + SpecialFunctions.FactorialILog(n2)
                - SpecialFunctions.FactorialILog(n1 + l1) - SpecialFunctions.FactorialILog(n2 + l2)));

            int smin = System.Math.Max((2 * n1 - (a - dl) + 1) / 2, (2 * n2 - (a + dl) + 1) / 2);
            smin = System.Math.Max(0, smin);

            int smax = System.Math.Min(n1, n2);

            int i1 = a + l1 + l2;
            int i2 = a - dl;
            int i3 = a + dl;
            bool even = i1 % 2 == 0;

            if(even) {
                i1 = i1 / 2; i2 = i2 / 2; i3 = i3 / 2;
            }
            else {
                smin = 0;
                i1 = (i1 + 1) / 2; i2 = (i2 + 1) / 2; i3 = (i3 + 1) / 2;
            }

            double sum = 0.0;
            for(int s = smin; s <= smax; s++) {
                double d = -SpecialFunctions.FactorialILog(s) - SpecialFunctions.FactorialILog(n1 - s)
                    - SpecialFunctions.FactorialILog(n2 - s);

                double sign = 1;

                if(even)
                    d += SpecialFunctions.FactorialILog(i1 + s)
                    + SpecialFunctions.FactorialILog(i2) + SpecialFunctions.FactorialILog(i3)
                    - SpecialFunctions.FactorialILog(i2 - n1 + s) - SpecialFunctions.FactorialILog(i3 - n2 + s);
                else {
                    d += SpecialFunctions.HalfFactorialILog(i1 + s)
                    + SpecialFunctions.HalfFactorialILog(i2) + SpecialFunctions.HalfFactorialILog(i3)
                    - SpecialFunctions.HalfFactorialILog(i2 - n1 + s) - SpecialFunctions.HalfFactorialILog(i3 - n2 + s);

                    if(i2 < 0 && -i2 % 2 == 1)
                        sign = -sign;

                    if(i2 - n1 + s < 0 && (-i2 + n1 - s) % 2 == 1)
                        sign = -sign;

                    if(i3 - n2 + s < 0 && (-i3 + n2 - s) % 2 == 1)
                        sign = -sign;
                }

                sum += sign * System.Math.Exp(d);
            }

            double result = norm * sum / System.Math.Pow(this.alpha, a / 2.0);
            return result;
//            if((n1 + n2 + a) % 2 == 0) return result; else return -result;
        }
    }
}

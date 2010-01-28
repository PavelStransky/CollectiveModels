using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.PT {
    public partial class PT3 {
        /// <summary>
        /// V�po�et minim�ln� vzd�lenosti mezi dv�ma hladinami
        /// </summary>
        private class Minimum {
            private int n1, n2;
            private double b1, b2;
            private double a, omega0, hbar;
            private int maxn;
            private int numev;

            public Minimum(double a, double b1, double b2, double omega0, double hbar, int n1, int n2, int maxn) {
                this.a = a;
                this.b1 = b1;
                this.b2 = b2;
                this.omega0 = omega0;
                this.hbar = hbar;
                this.n1 = n1;
                this.n2 = n2;
                this.maxn = maxn;

                this.numev = System.Math.Max(this.n1, this.n2) + 1;
            }

            /// <summary>
            /// Vlastn� hodnoty syst�mu pro dan� b
            /// </summary>
            /// <param name="b">Parametr b</param>
            private Vector EigenValues(double b) {
                PT3 pt3 = new PT3(this.a, b, this.omega0, this.hbar);
                pt3.Compute(this.maxn, false, this.numev, null);
                return pt3.GetEigenValues();
            }

            /// <summary>
            /// Vzd�lenost mezi zadan�mi hladinami
            /// </summary>
            /// <param name="b">Parametr b</param>
            public double Distance(double b) {
                Vector e = this.EigenValues(b);
                return System.Math.Abs(e[this.n1] - e[this.n2]);
            }

            /// <summary>
            /// V�po�et minim�ln� vzd�lenosti mezi dv�ma hladinami
            /// </summary>
            /// <param name="precision">P�esnost v�po�tu</param>
            public PointD Compute(double precision) {
                Bisection bisection = new Bisection(this.Distance);
                double bmin = bisection.Minimum(this.b1, this.b2, precision);

                Vector e = this.EigenValues(bmin);
                return new PointD(bmin, 0.5 * (e[this.n1] + e[this.n2]));
            }
        }
    }
}

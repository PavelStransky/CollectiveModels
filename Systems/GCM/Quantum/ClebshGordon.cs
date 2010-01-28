using System;
using System.Collections.Generic;
using System.Text;


namespace PavelStransky.Systems {
    using Math = System.Math;
    class ClebshGordon {
        private double[,] elements;
        private int i1, i2, i;

        private bool selection_rules(int j1, int j2, int m1, int m2,
            int j, int m) {
            if(m1 + m2 != m) return false;
            if(j > j1 + j2) return false;
            if(j < Math.Abs(j1 - j2)) return false;

            return true;
        }

        private int alpha_plus_sq(int i, int n) {
            return (i - n) * (i + n + 2) / 2;
        }

        private int alpha_minus_sq(int i, int n) {
            return (i + n) * (i - n + 2) / 2;
        }

        public double Value(double j1, double j2, double m1, double m2,
            double j, double m) {
            // vsechny parametry prevedeme na celociselne pocty polovin 
            i1 = (int)Math.Round(j1 * 2); // i1 = 1 <=> j1 = 1/2, ...
            i2 = (int)Math.Round(j2 * 2);
            i = (int)Math.Round(j * 2);
            int ret_n1 = (int)Math.Round(m1 * 2);
            int ret_n2 = (int)Math.Round(m2 * 2);
            int ret_n = (int)Math.Round(m * 2);

            if(!selection_rules(i1, i2, ret_n1, ret_n2, i, ret_n)) return 0;

            // druha +1 je kvuli nulovym okrajum
            elements = new double[i1 + 1 + 1, i2 + 1 + 1];

            int start_n1 = i1;
            int start_n2 = i - i1;

            // vyplnime vsechny elementy pro ktere je n2 maximalni, tj. 

            // _ne_normalizovana hodnota

            elements[(start_n1 + i1) / 2, (start_n2 + i2) / 2] = 1;

            for(int n2 = i - i1 + 2; n2 <= i2; n2 += 2) {
                int n1 = i - n2; // horni sikma cast

                // (K.50)
                elements[(n1 + i1) / 2, (n2 + i2) / 2] =
                     -Math.Sqrt(((double)alpha_plus_sq(i2, n2 - 2)) / alpha_minus_sq(i1, i - n2 + 2)) *
                     elements[(n1 + 2 + i1) / 2, (n2 - 2 + i2) / 2];
            }

            double N = 0; // soucet kvadratu podle (K.54) m=j
            for(int n2 = i - i1; n2 <= i2; n2 += 2) {
                int n1 = i - n2; // horni sikma cast
                N += elements[(n1 + i1) / 2, (n2 + i2) / 2] * elements[(n1 + i1) / 2, (n2 + i2) / 2];
            }

            N = 1 / Math.Sqrt(N);
            for(int n2 = i - i1; n2 <= i2; n2 += 2) {
                int n1 = i - n2; // horni sikma cast
                elements[(n1 + i1) / 2, (n2 + i2) / 2] *= N;
            }

            // (K.48)
            for(int n2 = i2; n2 >= -i2; n2 -= 2)
                for(int n1 = Math.Min(i1, i - n2 - 2); n1 >= Math.Max(-i1, -i - n2); n1 -= 2) {
                    elements[(n1 + i1) / 2, (n2 + i2) / 2] =
                        (Math.Sqrt(alpha_minus_sq(i1, n1 + 2)) * elements[(n1 + 2 + i1) / 2, (n2 + i2) / 2]
                        + Math.Sqrt(alpha_minus_sq(i2, n2 + 2)) * elements[(n1 + i1) / 2, (n2 + 2 + i2) / 2]) /
                        Math.Sqrt(alpha_minus_sq(i, n1 + n2 + 2));
                }

            return elements[(ret_n1 + i1) / 2, (ret_n2 + i2) / 2];
        }


        public double Six_j(double j1, double j2, double j3,
                            double m1, double m2, double m3) {
            return Math.Pow(-1, j1 - j2) * Value(j1, j2, 0, 0, j3, 0) / Math.Sqrt(2 * j3 + 1);
        }

    }
}

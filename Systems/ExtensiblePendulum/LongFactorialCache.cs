using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// (n-1/2)! stored in index n
    /// </summary>
    public class LongFactorialCache {
        private LongFraction [] hFactorialP, hFactorialN;
        private LongNumber[] factorial;

        public LongFactorialCache(int max, int hmin, int hmax) {
            LongNumber a = new LongNumber(1);
            LongNumber b = new LongNumber(1);

            this.factorial = new LongNumber[max + 1];
            this.factorial[0] = a;
            for(int i = 1; i <= max; i++) {
                a *= i;
                this.factorial[i] = a;
            }

            a = new LongNumber(1);
            this.hFactorialP = new LongFraction[hmax + 1];
            for(int i = 0; i <= hmax; i++) {
                this.hFactorialP[i] = new LongFraction(a, b);
                a *= (2 * i + 1);
                b *= 2;
            }

            a = new LongNumber(1);
            b = new LongNumber(1);
            this.hFactorialN = new LongFraction[hmin + 1];
            for(int i = 0; i <= hmin; i++) {
                this.hFactorialN[i] = new LongFraction(a, b);
                a *= -2;
                b *= (2 * i + 1);
            }
        }

        public LongNumber Get(int i) {
            return this.factorial[i];
        }

        public LongFraction GetHalf(int i) {
            if(i >= 0)
                return this.hFactorialP[i];
            else
                return this.hFactorialN[-i];
        }
    }
}

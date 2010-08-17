using System;
using System.Collections;

namespace PavelStransky.Math {
    /// <summary>
    /// T��da generuj�c� �asovou �adu ShotNoise
    /// </summary>
    /// <remarks>Phys. Rev. E 82, 021109 (2010), formula (3)</remarks>
    public class ShotNoise {
        /// <summary>
        /// Gener�tor rovnom�rn� rozd�len�ch n�hodn�ch ��sel
        /// </summary>
        private Random r;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ShotNoise() {
            this.r = new Random();
        }

        /// <summary>
        /// Vr�t� jednu hodnotu
        /// </summary>
        /// <param name="length">D�lka �asov� �ady</param>
        /// <param name="amplitudes">Amplitudy jednotliv�ch proces�</param>
        /// <param name="lambda">Poissonova intenzita</param>
        /// <param name="relax">Relaxace</param>
        public Vector GetVector(int length, Vector amplitudes, double relax, double lambda) {
            Vector result = new Vector(length);

            ArrayList times = new ArrayList();

            for(int i = 0; i < length; i++) {
                times.AddRange(this.Poisson(i, lambda));

                int j = 0;
                double v = 0;
                foreach(double t in times) {
                    v += amplitudes[j] * System.Math.Exp(-relax * (i - t));
                    j++;
                }

                result[i] = v;
            }

            return result;
        }

        /// <summary>
        /// Vr�t� �asy Poissonova procesu
        /// </summary>
        /// <param name="lambda">Poissonova intenzita</param>
        private ArrayList Poisson(int time, double lambda) {
            ArrayList result = new ArrayList();

            double L = System.Math.Exp(-lambda);
            double p = 1;

            int k = 0;
            do {
                p *= this.r.NextDouble();
                k++;
            } while(p > L);

            k -= 1;

            for(int i = 0; i < k; i++)
                result.Add(this.r.NextDouble() + time);

            result.Sort();
            return result;
        }
    }
}

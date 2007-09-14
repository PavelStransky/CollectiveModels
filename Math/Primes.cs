using System;
using System.Collections;
using System.Text;

namespace PavelStransky.Math {
    public class Primes {
        /// <summary>
        /// Vypoèítá prvoèísla menší než zadané èíslo
        /// </summary>
        /// <param name="max">Maximální prvoèíslo</param>
        public static int[] GetPrimes(int number) {
            ArrayList primes = new ArrayList();

            primes.Add(2);
            int count = 1;

            for(int i = 3; i < int.MaxValue; i += 2) {
                int sqrt = (int)System.Math.Sqrt(i);

                bool isPrime = true;
                foreach(int prime in primes) {
                    if(prime > sqrt)
                        break;
                    if(i % prime == 0) {
                        isPrime = false;
                        break;
                    }
                }

                if(isPrime) {
                    primes.Add(i);
                    count++;

                    if(count > number)
                        break;
                }
            }

            int[] result = new int[count];

            int j = 0;
            foreach(int prime in primes)
                result[j++] = prime;

            return result;
        }
    }
}

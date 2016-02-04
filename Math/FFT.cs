using System;
using System.Collections;
using System.Numerics;

namespace PavelStransky.Math {
	/// <summary>
	/// Tøída implementující algoritmus pro výpoèet FFT
	/// </summary>
	public class FFTCore {
		/// <summary>
		/// Urèí nejmenší možnou délku FFT øady
		/// (nejbližší mocninu 2 k dané délce øady)
		/// </summary>
		/// <param name="arrayLength">Délka vstupní øady</param>
		public static int FFTLength(int arrayLength) {
			return 1 << FFTLengthBase(arrayLength);
		}

		/// <summary>
		/// Urèí Log2 nejmenší možné délky FFT øady
		/// </summary>
		/// <param name="arrayLength">Délka vstupní øady</param>
		public static int FFTLengthBase(int arrayLength) {
			// První nejvyšší mocnina dvou
			int lbase = (int)System.Math.Log(arrayLength, 2);
			if(1 << lbase != arrayLength)
				lbase++;

			return lbase;
		}
		
		/// <summary>
		/// Vlastní funkce pro výpoèet FFT
		/// </summary>
		/// <param name="source">Zdrojová data</param>
		/// <param name="invFFT">true pro výpoèet inverzní FFT</param>
		protected static ComplexVector Compute(ComplexVector source, bool invFFT) {
			// Délka øady FFT
			int lbase = FFTLengthBase(source.Length);
			int length = FFTLength(source.Length);

			ComplexVector fft = new ComplexVector(length);

			// Pomocná promìnná
			Complex s;

			// Konstanta v exponenciele (zohledòuje koeficient inverzní FFT)
			double expKonst = 2 * System.Math.PI;
			if(invFFT) expKonst *= -1;

			// Bitová reverze
			for(int i = 0; i < length; i++) {
				// Nový index
				int k = 0;
				for(int j = 0; j < lbase; j++) 
					k |= ((i >> j) & 1) << (lbase - j - 1);
		
				// Do nového uspoøádání (je potøeba kontrolovat, zda ve starém poli nejsme mimo rozsah)
				fft[i] = k >= source.Length ? 0 : source[k];
			}

			// Vlastní FFT
			for(int i = lbase - 1; i >= 0; i--) {
				for(int j = 0; j < (1 << i); j++) {
					// Index liché èásti Fourierovy øady
					int k = 1 << (lbase - i);
					int index = j * k;
					k >>= 1;

					for(int l = 0; l < k; l++) {
						int i1 = index + l;
						int i2 = index + l + k;

						Complex W = Complex.Exp(new Complex(0, expKonst * l / (k << 1)));

						s = fft[i1];
						fft[i1] = s + W * fft[i2];
						fft[i2] = s - W * fft[i2];
					}
				}
			}

			// Normování inverzní FFT
			if(invFFT)
				for(int i = 0; i < length; i++)
					fft[i] /= (double)length;

			return fft;
		}
	}

	/// <summary>
	/// Fast Fourier Transform
	/// </summary>
	public class FFT : FFTCore {
		/// <summary>
		/// Výpoèetní metoda
		/// </summary>
		/// <param name="data">Vstupní hodnoty (komplexní vektor)</param>
		public static ComplexVector Compute(ComplexVector data) {
			return Compute(data, false);
		}

		/// <summary>
		/// Prázdný konstruktor (abychom nemohli vytvoøit instanci)
		/// </summary>
		private FFT() {}

		/// <summary>
		/// Poèet reálných frekvencí (do výkonového spektra)
		/// </summary>
		public static int NumFrequency(ComplexVector data) {
			return data.Length / 2 + 1;
		}
			
        /// <summary>
		/// Vlastní funkce pro výpoèet spektra
		/// </summary>
        /// <param name="data">Data</param>
        /// <param name="samplingRate">Sampling rate</param>
		public static PointVector PowerSpectrum(ComplexVector data, double samplingRate) {
			// Rozdíl dvou sousedních frekvencí
			double fdiff = 1.0 / (samplingRate * data.Length);
			int numFrequency = NumFrequency(data);

			PointVector result = new PointVector(numFrequency);
		
			// Stejnosmìrná složka
			result[0].X = 0;
			result[0].Y = System.Math.Pow(data[0].Magnitude, 2);

			for(int i = 1; i < numFrequency; i++) {
				result[i].X = i * fdiff;
                result[i].Y = System.Math.Pow(data[i].Magnitude, 2) + System.Math.Pow(data[data.Length - i].Magnitude, 2);
			}

			return result;
		}

        /// <summary>
        /// Funkce, která vrací fáze frekvencí
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="samplingRate">Sampling rate</param>
        public static PointVector Phases(ComplexVector data, double samplingRate) {
            // Rozdíl dvou sousedních frekvencí
            double fdiff = 1.0 / (samplingRate * data.Length);
            int numFrequency = NumFrequency(data);

            PointVector result = new PointVector(numFrequency);

            // Stejnosmìrná složka
            result[0].X = 0;
            result[0].Y = System.Math.Pow(data[0].Magnitude, 2);

            for(int i = 0; i < numFrequency; i++) {
                result[i].X = i * fdiff;
                result[i].Y = data[i].Phase;
            }

            return result;
        }
	}

	/// <summary>
	/// Inverse Fast Fourier Transform
	/// </summary>
	public class InvFFT : FFTCore {
		/// <summary>
		/// Výpoèetní metoda
		/// </summary>
		/// <param name="cv">Vstupní hodnoty (komplexní vektor)</param>
		public static ComplexVector Compute(ComplexVector cv) {
			return Compute(cv, true);
		}

		/// <summary>
		/// Prázdný konstruktor (abychom nemohli vytvoøit instanci)
		/// </summary>
		private InvFFT() {}
	}
}


using System;
using System.Collections;
using System.Numerics;

namespace PavelStransky.Math {
	/// <summary>
	/// T��da implementuj�c� algoritmus pro v�po�et FFT
	/// </summary>
	public class FFTCore {
		/// <summary>
		/// Ur�� nejmen�� mo�nou d�lku FFT �ady
		/// (nejbli��� mocninu 2 k dan� d�lce �ady)
		/// </summary>
		/// <param name="arrayLength">D�lka vstupn� �ady</param>
		public static int FFTLength(int arrayLength) {
			return 1 << FFTLengthBase(arrayLength);
		}

		/// <summary>
		/// Ur�� Log2 nejmen�� mo�n� d�lky FFT �ady
		/// </summary>
		/// <param name="arrayLength">D�lka vstupn� �ady</param>
		public static int FFTLengthBase(int arrayLength) {
			// Prvn� nejvy��� mocnina dvou
			int lbase = (int)System.Math.Log(arrayLength, 2);
			if(1 << lbase != arrayLength)
				lbase++;

			return lbase;
		}
		
		/// <summary>
		/// Vlastn� funkce pro v�po�et FFT
		/// </summary>
		/// <param name="source">Zdrojov� data</param>
		/// <param name="invFFT">true pro v�po�et inverzn� FFT</param>
		protected static ComplexVector Compute(ComplexVector source, bool invFFT) {
			// D�lka �ady FFT
			int lbase = FFTLengthBase(source.Length);
			int length = FFTLength(source.Length);

			ComplexVector fft = new ComplexVector(length);

			// Pomocn� prom�nn�
			Complex s;

			// Konstanta v exponenciele (zohled�uje koeficient inverzn� FFT)
			double expKonst = 2 * System.Math.PI;
			if(invFFT) expKonst *= -1;

			// Bitov� reverze
			for(int i = 0; i < length; i++) {
				// Nov� index
				int k = 0;
				for(int j = 0; j < lbase; j++) 
					k |= ((i >> j) & 1) << (lbase - j - 1);
		
				// Do nov�ho uspo��d�n� (je pot�eba kontrolovat, zda ve star�m poli nejsme mimo rozsah)
				fft[i] = k >= source.Length ? 0 : source[k];
			}

			// Vlastn� FFT
			for(int i = lbase - 1; i >= 0; i--) {
				for(int j = 0; j < (1 << i); j++) {
					// Index lich� ��sti Fourierovy �ady
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

			// Normov�n� inverzn� FFT
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
		/// V�po�etn� metoda
		/// </summary>
		/// <param name="data">Vstupn� hodnoty (komplexn� vektor)</param>
		public static ComplexVector Compute(ComplexVector data) {
			return Compute(data, false);
		}

		/// <summary>
		/// Pr�zdn� konstruktor (abychom nemohli vytvo�it instanci)
		/// </summary>
		private FFT() {}

		/// <summary>
		/// Po�et re�ln�ch frekvenc� (do v�konov�ho spektra)
		/// </summary>
		public static int NumFrequency(ComplexVector data) {
			return data.Length / 2 + 1;
		}
			
        /// <summary>
		/// Vlastn� funkce pro v�po�et spektra
		/// </summary>
        /// <param name="data">Data</param>
        /// <param name="samplingRate">Sampling rate</param>
		public static PointVector PowerSpectrum(ComplexVector data, double samplingRate) {
			// Rozd�l dvou sousedn�ch frekvenc�
			double fdiff = 1.0 / (samplingRate * data.Length);
			int numFrequency = NumFrequency(data);

			PointVector result = new PointVector(numFrequency);
		
			// Stejnosm�rn� slo�ka
			result[0].X = 0;
			result[0].Y = System.Math.Pow(data[0].Magnitude, 2);

			for(int i = 1; i < numFrequency; i++) {
				result[i].X = i * fdiff;
                result[i].Y = System.Math.Pow(data[i].Magnitude, 2) + System.Math.Pow(data[data.Length - i].Magnitude, 2);
			}

			return result;
		}

        /// <summary>
        /// Funkce, kter� vrac� f�ze frekvenc�
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="samplingRate">Sampling rate</param>
        public static PointVector Phases(ComplexVector data, double samplingRate) {
            // Rozd�l dvou sousedn�ch frekvenc�
            double fdiff = 1.0 / (samplingRate * data.Length);
            int numFrequency = NumFrequency(data);

            PointVector result = new PointVector(numFrequency);

            // Stejnosm�rn� slo�ka
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
		/// V�po�etn� metoda
		/// </summary>
		/// <param name="cv">Vstupn� hodnoty (komplexn� vektor)</param>
		public static ComplexVector Compute(ComplexVector cv) {
			return Compute(cv, true);
		}

		/// <summary>
		/// Pr�zdn� konstruktor (abychom nemohli vytvo�it instanci)
		/// </summary>
		private InvFFT() {}
	}
}


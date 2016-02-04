using System;

namespace PavelStransky.Math {
	/// <summary>
	/// Tøída implementující komplexní èísla
	/// </summary>
	public class Complex {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="re">Reálná èást</param>
		/// <param name="im">Imaginární èást</param>
		public Complex(double re, double im) {
			this.re = re;
			this.im = im;
		}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="re">Reálná èást</param>
		public Complex(double re) : this(re, 0) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		public Complex() : this(0, 0) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="s">Komplexní èíslo jako øetìzec ve tvaru a + bi</param>
		public Complex(string s) {
			Complex c = Complex.Parse(s);
			this.re = c.re;
			this.im = c.im;
		}

		/// <summary>
		/// Sèítání komplexních èísel
		/// </summary>
		public static Complex operator +(Complex c1, Complex c2) {
			return new Complex(c1.re + c2.re, c1.im + c2.im);
		}

		/// <summary>
		/// Odèítání komplexních èísel
		/// </summary>
		public static Complex operator -(Complex c1, Complex c2) {
			return new Complex(c1.re - c2.re, c1.im - c2.im);
		}

        /// <summary>
        /// Zmìna znaménka komplexního èísla
        /// </summary>
        public static Complex operator -(Complex c) {
            return new Complex(-c.re, -c.im);
        }

		/// <summary>
		/// Násobení komplexních èísel
		/// </summary>
		public static Complex operator *(Complex c1, Complex c2) {
			return new Complex(c1.re*c2.re - c1.im*c2.im, c1.re*c2.im + c1.im*c2.re);
		}

		/// <summary>
		/// Násobení komplexních èísel
		/// </summary>
		public static Complex operator *(Complex c, double d) {
			return new Complex(c.re * d, c.im * d);
		}

		/// <summary>
		/// Dìlení komplexních èísel
		/// </summary>
		public static Complex operator /(Complex c1, Complex c2) {
			double d = c2.re*c2.re + c2.im*c2.im;
			return new Complex((c1.re*c2.re + c1.im*c2.im) / d, (c1.im*c2.re - c1.re*c2.im) / d);
		}

		/// <summary>
		/// Dìlení komplexních èísel
		/// </summary>
		public static Complex operator /(Complex c, double d) {
			return new Complex(c.re / d, c.im / d);
		}

		/// <summary>
		/// Odmocnina komplexního èísla
		/// </summary>
		public static Complex Sqrt(Complex c) {
			double a, b;

			a = c.im / c.re;
			b = System.Math.Abs(c.re) * System.Math.Sqrt(a*a + 1);

			return new Complex(System.Math.Sqrt((b+c.re)/2), System.Math.Sign(c.im) * System.Math.Sqrt((b-c.re)/2));
		}
		
		/// <summary>
		/// Exponenciela
		/// </summary>
		public static Complex Exp(Complex c) {
			double d = System.Math.Exp(c.re);
            return new Complex(d * System.Math.Cos(c.im), d * System.Math.Sin(c.im));
		}

        /// <summary>
        /// Komplexní jednièka
        /// </summary>
        public static Complex I { get { return new Complex(0.0, 1.0); } }

		/// <summary>
		/// Vrátí velikost komplexního èísla
		/// </summary>
		public double Norm {get {return System.Math.Sqrt(this.SquaredNorm);}}

		/// <summary>
		/// Vrátí ètverec normy
		/// </summary>
		public double SquaredNorm {get {return this.re * this.re + this.im * this.im;}}

        /// <summary>
        /// Vrátí fázi
        /// </summary>
        public double Phase {
            get {
                if(this.re == 0) {
                    if(this.im < 0)
                        return -System.Math.PI;
                    else if(this.im > 0)
                        return System.Math.PI;
                    else
                        return 0;
                }
                return System.Math.Atan(this.im / this.re);
            }
        }

		/// <summary>
		/// Øetìzec
		/// </summary>
		public override string ToString() {
			return string.Format("{0,10:#####0.000} {1} {2,10:#####0.000}i", this.re, this.im < 0 ? '-' : '+', System.Math.Abs(this.im));
		}

		/// <summary>
		/// Pøevede èíslo z øetìzce na èíslo
		/// </summary>
		/// <param name="s">Komplexní èíslo ve tvaru øetìzce a + bi</param>
		public static Complex Parse(string s) {
			Complex retValue = new Complex(0, 0);

			s = s.Replace(" ", string.Empty).ToUpper();
			int posI = s.IndexOf('I');
			// Není imaginární èást
			if(posI < 0) {
				retValue = new Complex(double.Parse(s), 0.0);
			}
			else {
				s.Replace("I", string.Empty);

				for(int i = 1; i < s.Length; i++)
					if((s[i] == '+' || s[i] == '-') && s[i - 1] != 'E') {
						double re = double.Parse(s.Substring(0, i));
						double im = double.Parse(s.Substring(i + 1, s.Length - i - 1));

						// Imaginární èást na prvním místì
						if(posI <= i) 
							retValue = new Complex(im, re);
						else
							retValue = new Complex(re, im);
					}
			}

			return retValue;
		}

		/// <summary>
		/// Komplexní sdružení
		/// </summary>
		public static Complex operator !(Complex c) {
			return new Complex(c.re, -c.im);
		}

		/// <summary>
		/// Implicitní pøetypování reálného èísla na komplexní
		/// </summary>
		public static implicit operator Complex(double d) {
			return new Complex(d);
		}

		/// <summary>
		/// Vynuluje komplexní èíslo
		/// </summary>
		public void Clear() {
			this.re = 0;
			this.im = 0;
		}

		public double Re {get {return this.re;} set {this.re = value;}}
		public double Im {get {return this.im;} set {this.im = value;}}

		private double re, im;
	}

}

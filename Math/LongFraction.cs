using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    public class LongFraction: IExportable {
        private LongNumber numerator, denominator;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="numerator">Èitatel</param>
        /// <param name="denominator">Jmenovatel</param>
        public LongFraction(LongNumber numerator, LongNumber denominator) {
            this.numerator = numerator;
            this.denominator = denominator;

            if(this.denominator.Minus) {
                this.denominator.Minus = false;
                this.numerator.Minus = !this.numerator.Minus;
            }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="numerator"></param>
        public LongFraction(LongNumber numerator) {
            this.numerator = numerator;
            this.denominator = new LongNumber(1);
        }

        /// <summary>
        /// Èitatel
        /// </summary>
        public LongNumber Numerator { get { return this.numerator; } }

        /// <summary>
        /// Jmenovatel
        /// </summary>
        public LongNumber Denominator { get { return this.denominator; } }

        /// <summary>
        /// Zkrátí zlomek
        /// </summary>
        public void Abbreviate() {
            LongNumber mcd = LongNumber.MCD(this.numerator, this.denominator);

            this.numerator /= mcd;
            this.denominator /= mcd;
        }

        /// <summary>
        /// Sèítání
        /// </summary>
        public static LongFraction operator +(LongFraction f1, LongFraction f2) {
            LongNumber n1 = f1.numerator * f2.denominator;
            LongNumber n2 = f2.numerator * f1.denominator;
            LongNumber d = f1.denominator * f2.denominator;

            return new LongFraction(n1 + n2, d);
        }

        /// <summary>
        /// Odèítání
        /// </summary>
        public static LongFraction operator -(LongFraction f1, LongFraction f2) {
            LongNumber n1 = f1.numerator * f2.denominator;
            LongNumber n2 = f2.numerator * f1.denominator;
            LongNumber d = f1.denominator * f2.denominator;

            return new LongFraction(n1 - n2, d);
        }

        /// <summary>
        /// Násobení
        /// </summary>
        public static LongFraction operator *(LongFraction f1, LongFraction f2) {
            return new LongFraction(f1.numerator * f2.numerator, f1.denominator * f2.denominator);
        }

        /// <summary>
        /// Dìlení
        /// </summary>
        public static LongFraction operator /(LongFraction f1, LongFraction f2) {
            return new LongFraction(f1.numerator * f2.denominator, f1.denominator * f2.numerator);
        }

        /// <summary>
        /// Mocnina
        /// </summary>
        /// <param name="exponent">Exponent</param>
        /// <returns></returns>
        public LongFraction Power(int exponent) {
            LongFraction result = new LongFraction(1);

            for(int i = 0; i < exponent; i++)
                result *= this;

            return result;
        }

        /// <summary>
        /// True pro zápornou hodnotu
        /// </summary>
        public bool Minus { get { return this.numerator.Minus; } set { this.numerator.Minus = value; } }

        /// <summary>
        /// Konverze na double
        /// </summary>
        public static explicit operator double(LongFraction l) {
            double d1, d2;
            int exp1, exp2;

            l.numerator.ToDouble(out d1, out exp1);
            l.denominator.ToDouble(out d2, out exp2);

            return d1 / d2 * System.Math.Pow(10.0, exp1 - exp2);
        }

        /// <summary>
        /// Pøevod z dlouhého èísla na zlomek
        /// </summary>
        /// <param name="l">Dlouhé èíslo (èitatel)</param>
        public static implicit operator LongFraction(LongNumber l) {
            return new LongFraction(l);
        }

        /// <summary>
        /// Pøevod celého èísla na zlomek
        /// </summary>
       public static implicit operator LongFraction(int i) {
           return new LongFraction(i);
        }

        /// <summary>
        /// Pøevod na øetìzec
        /// </summary>
        public override string ToString() {
            string s1 = this.numerator.ToString();
            string s2 = this.denominator.ToString();

            int length1 = s1.Length;
            int length2 = s2.Length;

            int length = System.Math.Max(length1, length2);

            StringBuilder result = new StringBuilder();
            result.Append(' ', length - length1);
            result.Append(s1);
            result.Append(Environment.NewLine);
            result.Append('-', length);
            result.Append(Environment.NewLine);
            result.Append(' ', length - length2);
            result.Append(s2);

            return result.ToString();
        }

        #region Implementace IExportable
        public void Export(Export export) {
            IEParam param = new IEParam();
            param.Add(this.numerator, "Numerator");
            param.Add(this.denominator, "Denominator");
            param.Export(export);
        }

        public LongFraction(Core.Import import) {
            IEParam param = new IEParam(import);
            this.numerator = param.Get() as LongNumber;
            this.denominator = param.Get() as LongNumber;
        }
        #endregion
    }
}

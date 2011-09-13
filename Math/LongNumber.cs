using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Reprezentace èísla s libovolnou pøesností
    /// </summary>
    public class LongNumber: IExportable, ICloneable {
        private uint[] cells;
        private bool minus;

        // Maximální hodnota jedné buòky
        private static uint maxValue;

        // Maximální poèet èíslic v jedné buòce
        private static int maxDigits;

        // Formát k výpisu èísla (metoda ToString)
        private static string format;

        /// <summary>
        /// Vypoèítá maximální hodnotu jedné èíselné buòky
        /// </summary>
        static LongNumber() {
            maxDigits = (int)System.Math.Log10(uint.MaxValue);
            maxValue = (uint)System.Math.Pow(10, maxDigits);

            StringBuilder s = new StringBuilder();
            s.Append('0', maxDigits);

            format = s.ToString();
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="i">Celé èíslo</param>
        public LongNumber(int i) {
            this.minus = i < 0;

            if(i == 0)
                this.cells = new uint[0];
            else {
                this.cells = new uint[1];
                this.cells[0] = (uint)i;
            }
        }

        /// <summary>
        /// Prázdný konstruktor
        /// </summary>
        private LongNumber() { }

        /// <summary>
        /// Sèítání
        /// </summary>
        public static LongNumber operator +(LongNumber l1, LongNumber l2) {
            if(l1.minus == l2.minus)
                return Add(l1, l2);

            else {
                l2.minus = !l2.minus;
                return Subtract(l1, l2);
            }
        }

        /// <summary>
        /// Odèítání
        /// </summary>
        public static LongNumber operator -(LongNumber l1, LongNumber l2) {
            if(l1.minus == l2.minus)
                return Subtract(l1, l2);

            else {
                l2.minus = !l2.minus;
                return Add(l1, l2);
            }
        }

        /// <summary>
        /// Seète dvì dlouhá èísla (pøedpokladem je, že mají stejné znaménko)
        /// </summary>
        private static LongNumber Add(LongNumber l1, LongNumber l2) {
            int length1 = l1.cells.Length;
            int length2 = l2.cells.Length;
            int length = System.Math.Max(length1, length2) + 1;

            LongNumber result = new LongNumber();
            result.cells = new uint[length];

            bool carry = false;

            for(int i = 0; i < length; i++) {
                uint d = 0;

                if(i < length1)
                    d += l1.cells[i];
                if(i < length2)
                    d += l2.cells[i];

                if(carry)
                    d++;

                if(d >= maxValue) {
                    d -= maxValue;
                    carry = true;
                }
                else
                    carry = false;

                result.cells[i] = d;
            }

            result.minus = l1.minus;
            result.RemoveZeroCells();

            return result;
        }

        /// <summary>
        /// Odeète druhé èíslo od prvního (pøedpokladem jsou rozdílná znaménka)
        /// </summary>
        private static LongNumber Subtract(LongNumber l1, LongNumber l2) {
            int length1 = l1.cells.Length;
            int length2 = l2.cells.Length;

            LongNumber result;

            // Snažíme se, abychom odèítali delší èíslo od kratšího
            if(length1 < length2) {
                result = Subtract(l2, l1);
                result.minus = !result.minus;
                return result;
            }

            else if(length1 == length2) {
                for(int i = length1 - 1; i >= 0; i--)
                    if(l1.cells[i] < l2.cells[i]) {
                        result = Subtract(l2, l1);
                        result.minus = !result.minus;
                        return result;
                    }
                    else if(l1.cells[i] > l2.cells[i])
                        break;
            }

            // Tady už je první èíslo vždy delší
            int length = length1;

            result = new LongNumber();
            result.cells = new uint[length];

            bool carry = false;

            for(int i = 0; i < length; i++) {
                int d = 0;

                d += (int)l1.cells[i];
            
                if(i < length2)
                    d -= (int)l2.cells[i];

                if(carry)
                    d--;

                if(d < 0) {
                    d += (int)maxValue;
                    carry = true;
                }
                else
                    carry = false;

                result.cells[i] = (uint)d;
            }

            result.minus = l1.minus;
            result.RemoveZeroCells();

            return result;
        }

        /// <summary>
        /// Porovnání
        /// </summary>
        public static bool operator >(LongNumber l1, LongNumber l2) {
            if(l1.minus != l2.minus) {
                if(l1.minus)
                    return false;
                else
                    return true;
            }

            int length1 = l1.cells.Length;
            int length2 = l2.cells.Length;

            if(length1 != length2) {
                if((l1.minus && length1 > length2)
                    || (!l1.minus && length2 > length1))
                    return false;
                else
                    return true;
            }

            for(int i = length1 - 1; i >= 0; i--) {
                if(l1.cells[i] == l2.cells[i])
                    continue;

                if((l1.minus && l1.cells[i] > l2.cells[i]) ||
                    (!l1.minus && l1.cells[i] < l2.cells[i]))
                    return false;
                else 
                    return true;
            }

            return false;
        }

        public static bool operator <(LongNumber l1, LongNumber l2) {
            return l2 > l1;
        }

        public static bool operator >=(LongNumber l1, LongNumber l2) {
            return !(l2 > l1);
        }

        public static bool operator <=(LongNumber l1, LongNumber l2) {
            return !(l1 > l2);
        }

        public static bool IsEqual(LongNumber l1, LongNumber l2) {
            if(l1.minus != l2.minus)
                return false;

            int length1 = l1.cells.Length;
            int length2 = l2.cells.Length;

            if(length1 != length2) {
                return false;
            }

            for(int i = length1 - 1; i >= 0; i--) 
                if(l1.cells[i] != l2.cells[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Násobení
        /// </summary>
        public static LongNumber operator *(LongNumber l1, LongNumber l2) {
            int length1 = l1.cells.Length;
            int length2 = l2.cells.Length;
            int length = length1 * length2;

            LongNumber result = new LongNumber(0);

            for(int i = 0; i < length1; i++) {
                LongNumber l = new LongNumber();
                l.cells = new uint[length2 + i + 1];

                uint carry = 0;

                for(int j = 0; j < length2; j++) {
                    ulong u = (ulong)l1.cells[i] * (ulong)l2.cells[j] + carry;
                    l.cells[i + j] = (uint)(u % maxValue);
                    carry = (uint)(u / maxValue);
                }

                l.cells[i + length2] = carry;

                result += l;
            }

            result.minus = (l1.minus == l2.minus) ? false : true;
            return result;
        }

        /// <summary>
        /// Dìlení dlouhého èísla celým èíslem
        /// </summary>
        /// <param name="l">Dlouhé èíslo (dìlenec)</param>
        /// <param name="i">Celé èíslo (dìlitel)</param>
        public static LongNumber operator /(LongNumber l, int i) {
            LongNumber result;
            int remainder;

            l.Divide(i, out result, out remainder);

            return result;
        }

        /// <summary>
        /// Dìlení dlouhého èísla dlouhým èíslem
        /// </summary>
        /// <param name="l1">Dìlenec</param>
        /// <param name="l2">Dìlitel</param>
        public static LongNumber operator /(LongNumber l1, LongNumber l2) {
            LongNumber result;
            LongNumber remainder;

            l1.Divide(l2, out result, out remainder);

            return result;
        }

        /// <summary>
        /// Dìlení se zbytkem
        /// </summary>
        /// <param name="d">Dìlitel</param>
        /// <param name="result">Výsledek</param>
        /// <param name="remainder">Zbytek</param>
        public void Divide(int d, out LongNumber result, out int remainder) {
            int length = this.cells.Length;
            bool minus = d < 0;

            ulong ud = (ulong)System.Math.Abs(d);

            result = new LongNumber();
            result.cells = new uint[length];

            remainder = 0;

            for(int i = length - 1; i >= 0; i--) {
                ulong n = (ulong)remainder * maxValue + (ulong)this.cells[i];

                result.cells[i] = (uint)(n / ud);
                remainder = (int)(n % ud);
            }

            result.RemoveZeroCells();
            result.minus = (this.minus == minus) ? false : true;
        }

        /// <summary>
        /// Dìlení se zbytkem
        /// </summary>
        /// <param name="d">Dìlitel</param>
        /// <param name="result">Výsledek</param>
        /// <param name="remainder">Zbytek</param>
        public void Divide(LongNumber d, out LongNumber result, out LongNumber remainder) {
            // Dìlitel vìtší než dìlenec
            if(!((this.minus && this < d) || (!this.minus && this > d))) {
                result = 0;
                remainder = this;
                return;
            }

            int lengthd = d.cells.Length;

            // Pouze jedno èíslo dìlitele - použijeme rychlejší algoritmuss
            if(lengthd == 1) {
                int id = d.minus ? -(int)d.cells[0] : (int)d.cells[0];
                int ir;
                this.Divide(id, out result, out ir);
                remainder = new LongNumber(ir);
                return;
            }

            int length = this.cells.Length;

            // Pøíprava dìlitele
            int d1 = (int)d.cells[lengthd - 1];
            int d2 = (int)d1 + 1;

            LongNumber n = new LongNumber();
            n.cells = new uint[length - lengthd + 1];
            for(int i = length - lengthd; i >= 0; i--)
                n.cells[i] = this.cells[i + lengthd - 1];

            LongNumber result1 = n / d1;
            LongNumber result2 = n / d2;

            LongNumber back1 = result1 * d;
            LongNumber back2 = result2 * d;

            do {
                LongNumber resultm = (result1 + result2) / 2;
                LongNumber backm = resultm * d;

                if(backm > this) {
                    result1 = resultm;
                    back1 = backm;
                }
                else {
                    result2 = resultm;
                    back2 = backm;
                }

                LongNumber diff = result1 - result2;
                diff.minus = false;

                if(diff <= 1)
                    break;
            } while(true);

            remainder = this - back2;
            result = result2;

            if(remainder >= d) {
                remainder = this - back1;
                result = result1;
            }
        }

        /// <summary>
        /// Mocnina èísla
        /// </summary>
        /// <param name="exponent">Exponent</param>
        public LongNumber Power(int exponent) {
            LongNumber result = new LongNumber(1);

            for(int i = 0; i < exponent; i++) 
                result *= this;

            return result;
        }

        /// <summary>
        /// True pro zápornou hodnotu
        /// </summary>
        public bool Minus { get { return this.minus; } set { this.minus = value; } }

        /// <summary>
        /// Odstraní buòky, které jsou nulové
        /// </summary>
        private void RemoveZeroCells() {
            int length = this.cells.Length;
            int nonzerolength = length;

            while(nonzerolength > 0 && this.cells[nonzerolength - 1] == 0)
                nonzerolength--;

            if(nonzerolength == length)
                return;

            uint []result = new uint[nonzerolength];
            for(int i = 0; i < nonzerolength; i++)
                result[i] = this.cells[i];

            this.cells = result;
        }

        /// <summary>
        /// Nejvìtší spoleèný dìlitel dvou èísel
        /// </summary>
        public static LongNumber MCD(LongNumber l1, LongNumber l2) {
            if(l1.minus) {
                l1 = (LongNumber)l1.Clone();
                l1.minus = false;
            }

            // l1 je vždy vìtší
            if(l2 > l1) {
                LongNumber l = l2;
                l2 = l1;
                l1 = l;
            }

            while(!IsEqual(l2, 0)) {
                LongNumber l, r;
                l1.Divide(l2, out l, out r);

                l1 = l2;
                l2 = r;
            }

            return l1;
        }

        /// <summary>
        /// Øetìzec pøevede na dlouhé èíslo
        /// </summary>
        /// <param name="s">Øetìzec</param>
        public static LongNumber Parse(string s) {
            s = s.Trim();

            LongNumber result = new LongNumber();

            if(s[0] == '-')
                result.minus = true;
            else
                result.minus = false;

            if(s[0] == '-' || s[0] == '+')
                s = s.Substring(1);

            int slength = s.Length;
            int length = (slength - 1) / maxDigits + 1;

            result.cells = new uint[length];
            int i2 = slength;
            int i1 = slength - maxDigits;

            for(int i = 0; i < length; i++) {
                if(i1 < 0)
                    i1 = 0;

                result.cells[i] = uint.Parse(s.Substring(i1, i2 - i1));

                i2 = i1;
                i1 -= maxDigits;
            }

            return result;
        }

        /// <summary>
        /// Konverze na double
        /// </summary>
        public static explicit operator double(LongNumber l) {
            double d;
            int exp;

            l.ToDouble(out d, out exp);
            return d * System.Math.Pow(10.0, exp);
        }

        /// <summary>
        /// Pøevede èíslo na double a vrátí základ a øád
        /// </summary>
        /// <param name="d">Základ</param>
        /// <param name="exp">Øád</param>
        public void ToDouble(out double d, out int exp) {
            int length = this.cells.Length;

            if(length == 0){
                d = 0.0;
                exp = 0;
            }

            else if(length == 1) {
                d = (double)this.cells[0];
                exp = 0;
            }

            else {
                int i = 1;
                double e = 1.0;
                d = 0.0;

                while(i <= length) {
                    d += this.cells[length - i] * e;

                    if(i >= 4)
                        break;

                    e /= maxValue;
                    i++;
                }

                exp = (length - 1) * maxDigits;
            }

            if(this.minus)
                d = -d;
        }

        /// <summary>
        /// Konverze na LongNumber
        /// </summary>
        /// <param name="i">Celé èíslo</param>
        public static implicit operator LongNumber(int i) {
            return new LongNumber(i);
        }

        /// <summary>
        /// Pøevod na string
        /// </summary>
        public override string ToString() {
            StringBuilder result = new StringBuilder();
            
            int length = this.cells.Length;

            if(this.minus)
                result.Append('-');

            if(length == 0)
                result.Append('0');
            
            else {
                result.Append(this.cells[length - 1]);

                for(int i = length - 2; i >= 0; i--)
                    result.Append(this.cells[i].ToString(format));
            }

            return result.ToString();
        }

        #region Implementace IExportable
        public void Export(Export export) {
            if(export.Binary) {
                BinaryWriter b = export.B;

                int length = this.cells.Length;
                b.Write(length);
                for(int i = 0; i < length; i++)
                    b.Write(this.cells[i]);

                b.Write(this.minus);
            }
            else 
                export.T.WriteLine(this.ToString());
        }

        public LongNumber(PavelStransky.Core.Import import) {
            if(import.Binary) {
                BinaryReader b = import.B;

                int length = b.ReadInt32();
                this.cells = new uint[length];
                for(int i = 0; i < length; i++)
                    this.cells[i] = b.ReadUInt32();

                this.minus = b.ReadBoolean();
            }
            else {
                LongNumber result = LongNumber.Parse(import.T.ReadLine());
                this.cells = result.cells;
                this.minus = result.minus;
            }
        }

        #endregion

        public object Clone() {
            LongNumber result = new LongNumber();
            result.cells = (uint[])this.cells.Clone();
            result.minus = this.minus;
            return result;
        }
    }
}

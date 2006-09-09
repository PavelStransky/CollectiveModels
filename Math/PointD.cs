using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Bod o složkách typu double
    /// </summary>
    public class PointD: IExportable {
        private double x, y;

        public double X { get { return this.x; } set { this.x = value; } }
        public double Y { get { return this.y; } set { this.y = value; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="x">Hodnota x</param>
        /// <param name="y">Hodnota y</param>
        public PointD(double x, double y) {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PointD() {
            this.x = 0;
            this.y = 0;
        }

        /// <summary>
        /// Sèítání dvou bodù
        /// </summary>
        public static PointD operator +(PointD p1, PointD p2) {
            return new PointD(p1.X + p2.X, p1.Y + p2.Y);
        }

        /// <summary>
        /// Odèítání dvou bodù
        /// </summary>
        public static PointD operator -(PointD p1, PointD p2) {
            return new PointD(p1.X - p2.X, p1.Y - p2.Y);
        }

        /// <summary>
        /// Násobení dvou bodù
        /// </summary>
        public static PointD operator *(PointD p1, PointD p2) {
            return new PointD(p1.X * p2.X, p1.Y * p2.Y);
        }

        /// <summary>
        /// Dìlení dvou bodù
        /// </summary>
        public static PointD operator /(PointD p1, PointD p2) {
            return new PointD(p1.X / p2.X, p1.Y / p2.Y);
        }

        /// <summary>
        /// Vzdálenost dvou bodù
        /// </summary>
        /// <param name="p1">První bod</param>
        /// <param name="p2">Druhý bod</param>
        public static double Distance(PointD p1, PointD p2) {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;

            return System.Math.Sqrt(dx * dx + dy * dy);
        }

        public override string ToString() {
            return string.Format("X = {0}, Y = {1}", this.x, this.y);
        }

        #region Implementace IExportable
        		/// <summary>
		/// Vytvoøí bod ze StreamReaderu
		/// </summary>
		/// <param name="t">StreamReader</param>
		public PointD(StreamReader t) {
			this.Import(t);
		}

		/// <summary>
		/// Vytvoøí bod z BinaryReaderu
		/// </summary>
		/// <param name="b">BinaryReader</param>
		public PointD(BinaryReader b) {
			this.Import(b);
		}
		
        /// <summary>
        /// Uloží bod do souboru
        /// </summary>
        /// <param name="fName">Jméno souboru</param>
        /// <param name="binary">Ukládat v binární podobì</param>
        public void Export(string fName, bool binary) {
            FileStream f = new FileStream(fName, FileMode.Create);

            if(binary) {
                BinaryWriter b = new BinaryWriter(f);
                this.Export(b);
                b.Close();
            }
            else {
                StreamWriter t = new StreamWriter(f);
                this.Export(t);
                t.Close();
            }

            f.Close();
        }

        /// <summary>
        /// Uloží bod do souboru textovì
        /// </summary>
        /// <param name="t">StreamWriter</param>
        public void Export(StreamWriter t) {
            t.WriteLine(this.GetType().FullName);
            t.WriteLine("{0}\t{1}", this.x, this.y);
        }

        /// <summary>
        /// Uloží obsah vektoru do souboru binárnì
        /// </summary>
        /// <param name="b">BinaryWriter</param>
        public void Export(BinaryWriter b) {
            b.Write(this.GetType().FullName);
            b.Write(this.x);
            b.Write(this.y);
        }

        /// <summary>
        /// Naète bod ze souboru
        /// </summary>
        /// <param name="fName">Jméno souboru</param>
        /// <param name="binary">Soubor v binární podobì</param>
        public void Import(string fName, bool binary) {
            FileStream f = new FileStream(fName, FileMode.Open);

            if(binary) {
                BinaryReader b = new BinaryReader(f);
                this.Import(b);
                b.Close();
            }
            else {
                StreamReader t = new StreamReader(f);
                this.Import(t);
                t.Close();
            }

            f.Close();
        }

        /// <summary>
        /// Naète bod ze souboru textovì
        /// </summary>
        /// <param name="t">StreamReader</param>
        public void Import(StreamReader t) {
            ImportExportException.CheckImportType(t.ReadLine(), this.GetType());

			string line = t.ReadLine();
			string []s = line.Split('\t');
			this.x = double.Parse(s[0]);
            this.y = double.Parse(s[1]);
        }

        /// <summary>
        /// Naète bod ze souboru binárnì
        /// </summary>
        /// <param name="b">BinaryReader</param>
        public void Import(BinaryReader b) {
            ImportExportException.CheckImportType(b.ReadString(), this.GetType());

            this.x = b.ReadDouble();
            this.y = b.ReadDouble();
        }
        #endregion
    }
}

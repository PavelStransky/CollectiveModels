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
        /// Prázdný konstruktor
        /// </summary>
        public PointD() {
            this.x = 0;
            this.y = 0;
        }

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
        /// Uloží bod do souboru
        /// </summary>
		/// <param name="export">Export</param>
		public void Export(Export export) {
            if(export.Binary) {
                // Binárnì
                BinaryWriter b = export.B;
                b.Write(this.x);
                b.Write(this.y);
            }
            else {
                // Textovì
                StreamWriter t = export.T;
                t.WriteLine("{0}\t{1}", this.x, this.y);
            }
        }

        /// <summary>
        /// Naète bod ze souboru textovì
        /// </summary>
        /// <param name="import">Import</param>
        public void Import(Import import) {
            if(import.Binary) {
                // Binárnì
                BinaryReader b = import.B;
                this.x = b.ReadDouble();
                this.y = b.ReadDouble();
            }
            else {
                // Textovì
                StreamReader t = import.T;
                string line = t.ReadLine();
                string[] s = line.Split('\t');
                this.x = double.Parse(s[0]);
                this.y = double.Parse(s[1]);
            }
        }
        #endregion
    }
}

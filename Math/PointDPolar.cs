using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Bod o slo�k�ch typu double, pol�rn� sou�adnice
    /// </summary>
    public class PointDPolar {
        private double r, fi;

        public double R { get { return this.r; } set { this.fi = value; } }
        public double Fi { get { return this.fi; } set { this.r = value; } }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="r">Hodnota r</param>
        /// <param name="fi">Hodnota fi</param>
        public PointDPolar(double r, double fi) {
            this.r = r;
            this.fi = fi;
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        public PointDPolar() {
            this.r = 0;
            this.fi = 0;
        }

        /// <summary>
        /// Oper�tor p�etypov�n� na bod o sou�adnic�ch x, y
        /// </summary>
        /// <param name="p">Bod v pol�rn�ch sou�adnic�ch</param>
        public static implicit operator PointD(PointDPolar p) {
            return new PointD(p.R * System.Math.Cos(p.Fi), p.R * System.Math.Sin(p.Fi));
        }

        public override string ToString() {
            return string.Format("R = {0}, Fi = {1}", this.r, this.fi);
        }
    }
}

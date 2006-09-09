using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Bod o složkách typu double, polární souøadnice
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
        /// Operátor pøetypování na bod o souøadnicích x, y
        /// </summary>
        /// <param name="p">Bod v polárních souøadnicích</param>
        public static implicit operator PointD(PointDPolar p) {
            return new PointD(p.R * System.Math.Cos(p.Fi), p.R * System.Math.Sin(p.Fi));
        }

        public override string ToString() {
            return string.Format("R = {0}, Fi = {1}", this.r, this.fi);
        }
    }
}

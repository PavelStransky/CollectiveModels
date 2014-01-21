using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// T��da, kter� k zadan�mu kumulovan�mu Brodyho rozd�len� fituje nejl�pe
    /// Brodyho parametr (pomoc� metody nejmen��ch �tverc�)
    /// </summary>
    public class BrodyFit {
        // Data
        private PointVector data;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="data">Data k fitov�n�</param>
        public BrodyFit(PointVector data) {
            this.data = data;
        }

        /// <summary>
        /// Funkce pro minimalizaci
        /// </summary>
        /// <param name="x">Brodyho parametr</param>
        private double MinimizeFunction(double x) {
            int length = data.Length;
            
            double result = 0.0;
            for(int i = 0; i < length; i++) {
                double r = (SpecialFunctions.CumulBrody(data[i].X, x) - data[i].Y) / System.Math.Sqrt(data[i].Y);
//                double r = (SpecialFunctions.CumulBrody(data[i].X, x) - data[i].Y);
                result += r * r;
            }

            return result;
        }

        /// <summary>
        /// Provede chi^2 fit a vr�t� hodnotu Brodyho parametru a standardn� odchylku
        /// </summary>
        /// <param name="precision">P�esnost v�po�tu</param>
        public Vector Fit(double precision) {
            Bisection b = new Bisection(this.MinimizeFunction);
            Vector result = new Vector(2);
            result[0] = b.Minimum(0.0, 1.0, precision);
            result[1] = System.Math.Sqrt(this.MinimizeFunction(result[0])/this.data.Length);
            return result;
        }
    }
}

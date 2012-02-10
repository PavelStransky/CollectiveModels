using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// An interface implementing the geometrical method of PRL 98, 234301 (2007)
    /// </summary>
    public interface IGeometricalMethod {
        Matrix VMatrix(double e, double x, double y);

        /// <summary>
        /// Vypoèítá oblast se zápornými vlastními èísly V matice 
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="n">Poèet bodù v jedné køivce</param>
        /// <param name="div">Dìlení intervalu 2pi (celkový poèet bodù výpoètu)</param>
        /// <param name="ei">Index vlastní hodnoty (øazený odspodu, tj. 0 je nejnižší)</param>
        PointVector[] VMatrixContours(double e, int n, int div, int ei);

        PointVector[] EquipotentialContours(double e, int n, int div);
    }
}

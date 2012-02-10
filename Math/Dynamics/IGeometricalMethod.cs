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
        /// Vypo��t� oblast se z�porn�mi vlastn�mi ��sly V matice 
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="n">Po�et bod� v jedn� k�ivce</param>
        /// <param name="div">D�len� intervalu 2pi (celkov� po�et bod� v�po�tu)</param>
        /// <param name="ei">Index vlastn� hodnoty (�azen� odspodu, tj. 0 je nejni���)</param>
        PointVector[] VMatrixContours(double e, int n, int div, int ei);

        PointVector[] EquipotentialContours(double e, int n, int div);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Interface pro dynamick� syst�m
    /// </summary>
    public interface IDynamicalSystem {
        /// <summary>
        /// Energie syst�mu
        /// </summary>
        /// <param name="x">Sou�adnice a rychlosti</param>
        double E(Vector x);

        /// <summary>
        /// Jakobi�n
        /// </summary>
        /// <param name="x">Sou�adnice a rychlosti</param>
        /// <returns></returns>
        Matrix Jacobian(Vector x);

        /// <summary>
        /// Prav� strana pohybov�ch rovnic
        /// </summary>
        /// <param name="x">Sou�adnice a rychlosti</param>
        /// <returns></returns>
        Vector Equation(Vector x);

        /// <summary>
        /// Generuje po��te�n� podm�nky s danou energi�
        /// </summary>
        /// <param name="e">Energie</param>
        Vector IC(double e);

        /// <summary>
        /// Generuje po��te�n� podm�nky s danou energi� a s dan�m �hlov�m momentem
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="l">�hlov� moment</param>
        Vector IC(double e, double l);

        /// <summary>
        /// Generuje hybnosti do po��te�n�ch podm�nek
        /// </summary>
        /// <param name="ic">Sou�adnice po��te�n�ch podm�nek</param>
        /// <param name="e">Energie</param>
        /// <returns>True, pokud se generov�n� poda�ilo</returns>
        bool IC(Vector ic, double e);

        /// <summary>
        /// Hranice kinematicky dostupn� oblasti na osach x, y, vx, vy
        /// </summary>
        /// <param name="e">Energie</param>
        Vector Bounds(double e);

        /// <summary>
        /// Po�et stup�� volnosti
        /// </summary>
        int DegreesOfFreedom { get;}
    }
}

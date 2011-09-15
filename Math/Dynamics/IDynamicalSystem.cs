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
        Matrix Jacobian(Vector x);

        /// <summary>
        /// Prav� strana pohybov�ch rovnic
        /// </summary>
        /// <param name="x">Sou�adnice a rychlosti</param>
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
        /// Kontrola hrani�n�ch podm�nek
        /// </summary>
        /// <param name="bounds">Hranice</param>
        Vector CheckBounds(Vector bounds);

        /// <summary>
        /// Po�et stup�� volnosti
        /// </summary>
        int DegreesOfFreedom { get;}

        /// <summary>
        /// Hodnota Peresova invariantu (bude vyst�edov�na)
        /// </summary>
        /// <param name="x">Sou�adnice a hybnosti</param>
        double PeresInvariant(Vector x);

        /// <summary>
        /// Postprocessing hodnot (Chceme-li nap��klad d�lat modulo)
        /// </summary>
        /// <param name="x">Sou�adnice a rychlosti</param>
        /// <returns>True, pokud jsme d�lali postprocessing</returns>
        bool PostProcess(Vector x);

        /// <summary>
        /// Rozhodnut�, zda je dan� trajektorie regul�rn� nebo chaotick�
        /// </summary>
        /// <param name="meanSALI">Hodnota SALI</param>
        /// <param name="t">�as</param>
        /// <returns>0 pro chaotickou, 1 pro regul�rn� trajektorii, -1 pro nerozhodnutou</returns>
        int SALIDecision(double meanSALI, double t);
    }
}

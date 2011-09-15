using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Math {
    /// <summary>
    /// Interface pro dynamický systém
    /// </summary>
    public interface IDynamicalSystem {
        /// <summary>
        /// Energie systému
        /// </summary>
        /// <param name="x">Souøadnice a rychlosti</param>
        double E(Vector x);

        /// <summary>
        /// Jakobián
        /// </summary>
        /// <param name="x">Souøadnice a rychlosti</param>
        Matrix Jacobian(Vector x);

        /// <summary>
        /// Pravá strana pohybových rovnic
        /// </summary>
        /// <param name="x">Souøadnice a rychlosti</param>
        Vector Equation(Vector x);

        /// <summary>
        /// Generuje poèáteèní podmínky s danou energií
        /// </summary>
        /// <param name="e">Energie</param>
        Vector IC(double e);

        /// <summary>
        /// Generuje poèáteèní podmínky s danou energií a s daným úhlovým momentem
        /// </summary>
        /// <param name="e">Energie</param>
        /// <param name="l">Úhlový moment</param>
        Vector IC(double e, double l);

        /// <summary>
        /// Generuje hybnosti do poèáteèních podmínek
        /// </summary>
        /// <param name="ic">Souøadnice poèáteèních podmínek</param>
        /// <param name="e">Energie</param>
        /// <returns>True, pokud se generování podaøilo</returns>
        bool IC(Vector ic, double e);

        /// <summary>
        /// Hranice kinematicky dostupné oblasti na osach x, y, vx, vy
        /// </summary>
        /// <param name="e">Energie</param>
        Vector Bounds(double e);

        /// <summary>
        /// Kontrola hranièních podmínek
        /// </summary>
        /// <param name="bounds">Hranice</param>
        Vector CheckBounds(Vector bounds);

        /// <summary>
        /// Poèet stupòù volnosti
        /// </summary>
        int DegreesOfFreedom { get;}

        /// <summary>
        /// Hodnota Peresova invariantu (bude vystøedována)
        /// </summary>
        /// <param name="x">Souøadnice a hybnosti</param>
        double PeresInvariant(Vector x);

        /// <summary>
        /// Postprocessing hodnot (Chceme-li napøíklad dìlat modulo)
        /// </summary>
        /// <param name="x">Souøadnice a rychlosti</param>
        /// <returns>True, pokud jsme dìlali postprocessing</returns>
        bool PostProcess(Vector x);

        /// <summary>
        /// Rozhodnutí, zda je daná trajektorie regulární nebo chaotická
        /// </summary>
        /// <param name="meanSALI">Hodnota SALI</param>
        /// <param name="t">Èas</param>
        /// <returns>0 pro chaotickou, 1 pro regulární trajektorii, -1 pro nerozhodnutou</returns>
        int SALIDecision(double meanSALI, double t);
    }
}

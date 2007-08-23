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
        /// <returns></returns>
        Matrix Jacobian(Vector x);

        /// <summary>
        /// Pravá strana pohybových rovnic
        /// </summary>
        /// <param name="x">Souøadnice a rychlosti</param>
        /// <returns></returns>
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
        /// Poèet stupòù volnosti
        /// </summary>
        int DegreesOfFreedom { get;}
    }
}

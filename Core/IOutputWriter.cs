using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// Interface pro všelijaký výstup
    /// </summary>
    public interface IOutputWriter {
        /// <summary>
        /// Vypíše daný objekt
        /// </summary>
        /// <param name="o">Objekt</param>
        string Write(object o);

        /// <summary>
        /// Vypíše daný objekt a zalomí øádku
        /// </summary>
        /// <param name="o">Objekt</param>
        string WriteLine(object o);

        /// <summary>
        /// Zalomí øádku
        /// </summary>
        string WriteLine();

        /// <summary>
        /// Vyèistí výstup
        /// </summary>
        void Clear();

        /// <summary>
        /// Provede odsazení
        /// </summary>
        /// <param name="i">Velikost odsazení (kladné pøidá, záporné ubere)</param>
        int Indent(int i);
    }
}

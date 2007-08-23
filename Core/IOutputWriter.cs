using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// Interface pro v�elijak� v�stup
    /// </summary>
    public interface IOutputWriter {
        /// <summary>
        /// Vyp�e dan� objekt
        /// </summary>
        /// <param name="o">Objekt</param>
        string Write(object o);

        /// <summary>
        /// Vyp�e dan� objekt a zalom� ��dku
        /// </summary>
        /// <param name="o">Objekt</param>
        string WriteLine(object o);

        /// <summary>
        /// Zalom� ��dku
        /// </summary>
        string WriteLine();

        /// <summary>
        /// Vy�ist� v�stup
        /// </summary>
        void Clear();

        /// <summary>
        /// Provede odsazen�
        /// </summary>
        /// <param name="i">Velikost odsazen� (kladn� p�id�, z�porn� ubere)</param>
        int Indent(int i);
    }
}

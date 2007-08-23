using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// Rozhran� pro t��d�n� objekt�
    /// </summary>
    public interface ISortable {
        /// <summary>
        /// Jednoduch� t��d�n�
        /// </summary>
        object Sort();

        /// <summary>
        /// Jednoduch� t��d�n� sestupn�
        /// </summary>
        object SortDesc();

        /// <summary>
        /// T��d�n� s kl��i
        /// </summary>
        /// <param name="keys">Kl��e</param>
        object Sort(ISortable keys);

        /// <summary>
        /// T��d�n� s kl��i sestupn�
        /// </summary>
        /// <param name="keys">Kl��e</param>
        object SortDesc(ISortable keys);

        /// <summary>
        /// D�lka - po�et objekt� k set��d�n�
        /// </summary>
        int Length { get;}

        /// <summary>
        /// Keys for sorting
        /// </summary>
        Array GetKeys();
    }
}

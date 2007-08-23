using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// Rozhraní pro tøídìní objektù
    /// </summary>
    public interface ISortable {
        /// <summary>
        /// Jednoduché tøídìní
        /// </summary>
        object Sort();

        /// <summary>
        /// Jednoduché tøídìní sestupnì
        /// </summary>
        object SortDesc();

        /// <summary>
        /// Tøídìní s klíèi
        /// </summary>
        /// <param name="keys">Klíèe</param>
        object Sort(ISortable keys);

        /// <summary>
        /// Tøídìní s klíèi sestupnì
        /// </summary>
        /// <param name="keys">Klíèe</param>
        object SortDesc(ISortable keys);

        /// <summary>
        /// Délka - poèet objektù k setøídìní
        /// </summary>
        int Length { get;}

        /// <summary>
        /// Keys for sorting
        /// </summary>
        Array GetKeys();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// Rozhraní pro export dat do souboru
    /// </summary>
    public interface IExportable {
        /// <summary>
        /// Uloží data do souboru
        /// </summary>
        void Export(Export export);

        /// <summary>
        /// Naète data
        /// </summary>
        void Import(Import import);
    }
}

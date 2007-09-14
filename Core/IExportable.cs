using System;
using System.Collections.Generic;
using System.Text;

namespace PavelStransky.Core {
    /// <summary>
    /// Rozhran� pro export dat do souboru
    /// </summary>
    public interface IExportable {
        /// <summary>
        /// Ulo�� data do souboru
        /// </summary>
        void Export(Export export);
    }
}

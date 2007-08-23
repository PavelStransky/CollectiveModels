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

        /// <summary>
        /// Na�te data
        /// </summary>
        void Import(Import import);
    }
}

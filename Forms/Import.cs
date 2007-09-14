using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Expression;

namespace PavelStransky.Forms {
    /// <summary>
    /// Nová implementace Import (nové typy)
    /// </summary>
    public class Import : PavelStransky.Expression.Import {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otevøení</param>
        /// <param name="binary">True, pokud bude soubor binární</param>
        public Import(string fileName, bool binary) : base(fileName, binary) { }

        /// <summary>
        /// Nové objekty
        /// </summary>
        /// <param name="typeName">Jméno typu objektu</param>
        /// <returns></returns>
        public override object CreateObject(string typeName) {
            if(typeName == typeof(Editor).FullName)
                return new Editor(this);
            else if(typeName == typeof(ResultForm).FullName)
                return new ResultForm(this);
            else if(typeName == typeof(GraphForm).FullName)
                return new GraphForm(this);
            else
                return base.CreateObject(typeName);
        }
    }
}

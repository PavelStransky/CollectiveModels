using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Expression;

namespace PavelStransky.Forms {
    /// <summary>
    /// Nov� implementace Import (nov� typy)
    /// </summary>
    public class Import : PavelStransky.Expression.Import {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otev�en�</param>
        /// <param name="binary">True, pokud bude soubor bin�rn�</param>
        public Import(string fileName, bool binary) : base(fileName, binary) { }

        /// <summary>
        /// Nov� objekty
        /// </summary>
        /// <param name="typeName">Jm�no typu objektu</param>
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

using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Expression;

namespace PavelStransky.Forms {
    /// <summary>
    /// Nov� implementace Import (nov� typy)
    /// </summary>
    public class Import: PavelStransky.Expression.Import {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otev�en�</param>
        public Import(string fileName) : base(fileName) { }

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
            else if(typeName == typeof(SingleGraphForm).FullName || typeName == "PavelStransky.Forms.GraphForm")
                return new SingleGraphForm(this);
            else
                return base.CreateObject(typeName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;

namespace PavelStransky.Math {
    /// <summary>
    /// Nov� implementace Import (nov� typy)
    /// </summary>
    public class Import: PavelStransky.Core.Import {
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
            if(typeName == typeof(Vector).FullName)
                return new Vector();
            else if(typeName == typeof(Matrix).FullName)
                return new Matrix();
            else if(typeName == typeof(PointD).FullName)
                return new PointD();
            else if(typeName == typeof(PointVector).FullName)
                return new PointVector();
            else if(typeName == typeof(ComplexVector).FullName)
                return new ComplexVector();
            else if(typeName == typeof(Jacobi).FullName)
                return new Jacobi();
            else
                return null;
        }
    }
}
    

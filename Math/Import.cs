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
        public Import(string fileName) : base(fileName) { }

        /// <summary>
        /// Nov� objekty
        /// </summary>
        /// <param name="typeName">Jm�no typu objektu</param>
        /// <returns></returns>
        public override object CreateObject(string typeName) {
            if(typeName == typeof(Vector).FullName)
                return new Vector(this);
            else if(typeName == typeof(Matrix).FullName)
                return new Matrix(this);
            else if(typeName == typeof(PointD).FullName)
                return new PointD(this);
            else if(typeName == typeof(PointVector).FullName)
                return new PointVector(this);
            else if(typeName == typeof(ComplexVector).FullName)
                return new ComplexVector(this);
            else if(typeName == typeof(Jacobi).FullName)
                return new Jacobi(this);
            else if(typeName == typeof(LongNumber).FullName)
                return new LongNumber(this);
            else if(typeName == typeof(LongFraction).FullName)
                return new LongFraction(this);
            else
                return null;
        }
    }
}
    

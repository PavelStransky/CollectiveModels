using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.IBM;

namespace PavelStransky.Expression {
    /// <summary>
    /// Nová implementace Import (nové typy)
    /// </summary>
    public class Import : PavelStransky.Math.Import {
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
            if(typeName == typeof(Array).FullName)
                return new Array();
            else if(typeName == typeof(Context).FullName)
                return new Context();
            else if(typeName == typeof(ClassicalGCM).FullName)
                return new ClassicalGCM();
            else if(typeName == typeof(ExtendedClassicalGCM1).FullName)
                return new ExtendedClassicalGCM1();
            else if(typeName == typeof(ExtendedClassicalGCM2).FullName)
                return new ExtendedClassicalGCM2();
            else if(typeName == typeof(ClassicalGCMJ).FullName)
                return new ClassicalGCMJ();
            else if(typeName == typeof(ClassicalIBM).FullName)
                return new ClassicalIBM();
            else if(typeName == typeof(Graph).FullName)
                return new Graph();
            else
                return base.CreateObject(typeName);
        }
    }
}

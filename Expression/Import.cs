using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.IBM;
using PavelStransky.PT;
using PavelStransky.Expression;

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
            if(typeName == typeof(TArray).FullName || typeName == "PavelStransky.Expression.Array")
                return new TArray(this);
            else if(typeName == typeof(List).FullName)
                return new List(this);
            else if(typeName == typeof(Context).FullName)
                return new Context(this);
            else if(typeName == typeof(ClassicalGCM).FullName)
                return new ClassicalGCM(this);
            else if(typeName == typeof(ExtendedClassicalGCM1).FullName)
                return new ExtendedClassicalGCM1(this);
            else if(typeName == typeof(ExtendedClassicalGCM2).FullName)
                return new ExtendedClassicalGCM2(this);
            else if(typeName == typeof(ClassicalGCMJ).FullName)
                return new ClassicalGCMJ(this);
            else if(typeName == typeof(ClassicalIBM).FullName)
                return new ClassicalIBM(this);
            else if(typeName == typeof(Graph).FullName)
                return new Graph(this);
            else if(typeName == typeof(LHOQuantumGCMC).FullName)
                return new LHOQuantumGCMC(this);
            else if(typeName == typeof(LHOQuantumGCMR).FullName)
                return new LHOQuantumGCMR(this);
            else if(typeName == typeof(LHOQuantumGCMRFull).FullName)
                return new LHOQuantumGCMRFull(this);
            else if(typeName == typeof(LHOQuantumGCMRL).FullName)
                return new LHOQuantumGCMRL(this);
            else if(typeName == typeof(LHOQuantumGCMRLO).FullName)
                return new LHOQuantumGCMRLO(this);
            else if(typeName == typeof(LHOQuantumGCMRLE).FullName)
                return new LHOQuantumGCMRLE(this);
            else if(typeName == typeof(LHOQuantumGCM5D).FullName)
                return new LHOQuantumGCM5D(this);
            else if(typeName == typeof(LHOQuantumGCMRALO).FullName)
                return new LHOQuantumGCMRALO(this);
            else if(typeName == typeof(LHOQuantumGCMRALE).FullName)
                return new LHOQuantumGCMRALE(this);
            else if(typeName == typeof(LHOQuantumGCMRAL5D).FullName)
                return new LHOQuantumGCMRAL5D(this);
            else if(typeName == typeof(PT1).FullName)
                return new PT1(this);
            else if(typeName == typeof(PT2).FullName)
                return new PT2(this);
            else
                return base.CreateObject(typeName);
        }
    }
}

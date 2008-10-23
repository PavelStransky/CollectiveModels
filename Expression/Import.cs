using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.IBM;
using PavelStransky.PT;
using PavelStransky.Expression;
using PavelStransky.ManyBody;

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
            else if(typeName == typeof(GraphParameterValues).FullName)
                return new GraphParameterValues(this);
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
            else if(typeName == typeof(LHOQuantumGCMIC).FullName || typeName == "PavelStransky.GCM.LHOQuantumGCMC")
                return new LHOQuantumGCMIC(this);
            else if(typeName == typeof(LHOQuantumGCMIR).FullName
                || typeName == "PavelStransky.GCM.LHOQuantumGCMR"
                || typeName == "PavelStransky.GCM.LHOQuantumGCMRL")
                return new LHOQuantumGCMIR(this);
            else if(typeName == typeof(LHOQuantumGCMIRFull).FullName || typeName == "PavelStransky.GCM.LHOQuantumGCMRFull")
                return new LHOQuantumGCMIRFull(this);
            else if(typeName == typeof(LHOQuantumGCMIRO).FullName || typeName == "PavelStransky.GCM.LHOQuantumGCMRLO")
                return new LHOQuantumGCMIRO(this);
            else if(typeName == typeof(LHOQuantumGCMI5D).FullName || typeName == "PavelStransky.GCM.LHOQuantumGCM5D")
                return new LHOQuantumGCMI5D(this);
            else if(typeName == typeof(LHOQuantumGCMARO).FullName || typeName == "PavelStransky.GCM.LHOQuantumGCMRALO")
                return new LHOQuantumGCMARO(this);
            else if(typeName == typeof(LHOQuantumGCMARE).FullName || typeName == "PavelStransky.GCM.LHOQuantumGCMRALE")
                return new LHOQuantumGCMARE(this);
            else if(typeName == typeof(LHOQuantumGCMA5D).FullName || typeName == "PavelStransky.GCM.LHOQuantumGCMRAL5D")
                return new LHOQuantumGCMA5D(this);
            else if(typeName == typeof(PT1).FullName)
                return new PT1(this);
            else if(typeName == typeof(PT2).FullName)
                return new PT2(this);
            else if(typeName == typeof(PT3).FullName)
                return new PT3(this);
            else if(typeName == typeof(ThreeBody).FullName || typeName == "PavelStransky.ThreeBody.ThreeBody")
                return new ThreeBody(this);
            else if(typeName == typeof(TwoBody).FullName)
                return new TwoBody(this);
            else
                return base.CreateObject(typeName);
        }
    }
}

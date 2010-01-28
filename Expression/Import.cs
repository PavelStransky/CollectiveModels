using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.Systems;

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
            else if(typeName == typeof(ClassicalGCM).FullName || typeName == "PavelStransky.GCM.ClassicalGCM")
                return new ClassicalGCM(this);
            else if(typeName == typeof(ExtendedClassicalGCM1).FullName || typeName == "PavelStransky.GCM.ExtendedClassicalGCM1")
                return new ExtendedClassicalGCM1(this);
            else if(typeName == typeof(ExtendedClassicalGCM2).FullName || typeName == "PavelStransky.GCM.ExtendedClassicalGCM2")
                return new ExtendedClassicalGCM2(this);
            else if(typeName == typeof(ClassicalGCMJ).FullName || typeName == "PavelStransky.GCM.ClassicalGCMJ")
                return new ClassicalGCMJ(this);
            else if(typeName == typeof(ClassicalIBM).FullName || typeName == "PavelStransky.IBM.ClassicalIBM")
                return new ClassicalIBM(this);
            else if(typeName == typeof(Graph).FullName)
                return new Graph(this);
            else if(typeName == typeof(LHOQuantumGCMIC).FullName 
                || typeName == "PavelStransky.GCM.LHOQuantumGCMC"
                || typeName == "PavelStransky.GCM.LHOQuantumGCMIC")
                return new LHOQuantumGCMIC(this);
            else if(typeName == typeof(LHOQuantumGCMIR).FullName
                || typeName == "PavelStransky.GCM.LHOQuantumGCMR"
                || typeName == "PavelStransky.GCM.LHOQuantumGCMRL"
                || typeName == "PavelStransky.GCM.LHOQuantumGCMIR")
                return new LHOQuantumGCMIR(this);
            else if(typeName == typeof(LHOQuantumGCMIRFull).FullName 
                || typeName == "PavelStransky.GCM.LHOQuantumGCMRFull"
                || typeName == "PavelStransky.GCM.LHOQuantumGCMIRFull")
                return new LHOQuantumGCMIRFull(this);
            else if(typeName == typeof(LHOQuantumGCMIRO).FullName 
                || typeName == "PavelStransky.GCM.LHOQuantumGCMRLO"
                || typeName == "PavelStransky.GCM.LHOQuantumGCMIRO")
                return new LHOQuantumGCMIRO(this);
            else if(typeName == typeof(LHOQuantumGCMI5D).FullName 
                || typeName == "PavelStransky.GCM.LHOQuantumGCM5D"
                || typeName == "PavelStransky.GCM.LHOQuantumGCMI5D")
                return new LHOQuantumGCMI5D(this);
            else if(typeName == typeof(LHOQuantumGCMARO).FullName 
                || typeName == "PavelStransky.GCM.LHOQuantumGCMRALO"
                || typeName == "PavelStransky.GCM.LHOQuantumGCMARO")
                return new LHOQuantumGCMARO(this);
            else if(typeName == typeof(LHOQuantumGCMARE).FullName 
                || typeName == "PavelStransky.GCM.LHOQuantumGCMRALE"
                || typeName == "PavelStransky.GCM.LHOQuantumGCMARE")
                return new LHOQuantumGCMARE(this);
            else if(typeName == typeof(LHOQuantumGCMA5D).FullName 
                || typeName == "PavelStransky.GCM.LHOQuantumGCMRAL5D"
                || typeName == "PavelStransky.GCM.LHOQuantumGCMA5D")
                return new LHOQuantumGCMA5D(this);
            else if(typeName == typeof(PT1).FullName || typeName == "PavelStransky.PT.PT1")
                return new PT1(this);
            else if(typeName == typeof(PT2).FullName || typeName == "PavelStransky.PT.PT2")
                return new PT2(this);
            else if(typeName == typeof(PT3).FullName || typeName == "PavelStransky.PT.PT3")
                return new PT3(this);
            else if(typeName == typeof(ThreeBody).FullName 
                || typeName == "PavelStransky.ThreeBody.ThreeBody"
                || typeName == "PavelStransky.ManyBody.ThreeBody")
                return new ThreeBody(this);
            else if(typeName == typeof(TwoBody).FullName || typeName == "PavelStransky.ManyBody.TwoBody")
                return new TwoBody(this);
            else if(typeName == typeof(CoupledHarmonicOscillator).FullName || typeName == "PavelStransky.CHO.CoupledHarmonicOscillator")
                return new CoupledHarmonicOscillator(this);
            else if(typeName == typeof(DoublePendulum).FullName || typeName == "PavelStransky.DoublePendulum.DoublePendulum")
                return new DoublePendulum(this);
            else
                return base.CreateObject(typeName);
        }
    }
}

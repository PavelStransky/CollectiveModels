using System;
using System.Collections.Generic;
using System.Text;

using PavelStransky.Core;
using PavelStransky.Math;

namespace PavelStransky.Systems {
    /// <summary>
    /// Nová implementace Import (nové typy)
    /// </summary>
    public class Import: PavelStransky.Math.Import {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="fileName">Soubor k otevøení</param>
        public Import(string fileName) : base(fileName) { }

        /// <summary>
        /// Nové objekty
        /// </summary>
        /// <param name="typeName">Jméno typu objektu</param>
        /// <returns></returns>
        public override object CreateObject(string typeName) {
            if(typeName == typeof(LHO5DIndex).FullName)
                return new LHO5DIndex(this);
            else if(typeName == typeof(LHO5DIndexI).FullName)
                return new LHO5DIndexI(this);
            else if(typeName == typeof(LHOCartesianIndex).FullName)
                return new LHOCartesianIndex(this);
            else if(typeName == typeof(LHOPolarIndexE).FullName)
                return new LHOPolarIndexE(this);
            else if(typeName == typeof(LHOPolarIndexI).FullName)
                return new LHOPolarIndexI(this);
            else if(typeName == typeof(LHOPolarIndexIFull).FullName)
                return new LHOPolarIndexIFull(this);
            else if(typeName == typeof(LHOPolarIndexIO).FullName)
                return new LHOPolarIndexIO(this);
            else if(typeName == typeof(LHOPolarIndexO).FullName)
                return new LHOPolarIndexO(this);
            else if(typeName == typeof(DPBasisIndex).FullName)
                return new DPBasisIndex(this);
            else if(typeName == typeof(PTBasisIndex).FullName)
                return new PTBasisIndex(this);
            else if(typeName == typeof(SpheroidBasisIndex).FullName)
                return new SpheroidBasisIndex(this);
            else if(typeName == typeof(SCBasisIndex).FullName)
                return new SCBasisIndex(this);
            else if(typeName == typeof(EPBasisIndex).FullName)
                return new EPBasisIndex(this);
            else if(typeName == typeof(LipkinFullBasisIndex).FullName)
                return new LipkinFullBasisIndex(this);
            else if(typeName == typeof(LipkinFactorizedBasisIndex).FullName)
                return new LipkinFactorizedBasisIndex(this);
            else if(typeName == typeof(LipkinOneBasisIndex).FullName)
                return new LipkinOneBasisIndex(this);
            else if(typeName == typeof(LipkinTwoBasisIndex).FullName)
                return new LipkinTwoBasisIndex(this);

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
            else if(typeName == typeof(QuantumDP).FullName)
                return new QuantumDP(this);
            else if(typeName == typeof(ClassicalDP).FullName)
                return new ClassicalDP(this);
            else if(typeName == typeof(EigenSystem).FullName)
                return new EigenSystem(this);
            else if(typeName == typeof(Traffic).FullName)
                return new Traffic(this);
            else if(typeName == typeof(Street).FullName)
                return new Street(this);
            else if(typeName == typeof(Crossing).FullName)
                return new Crossing(this);
            else if(typeName == typeof(Spheroid).FullName)
                return new Spheroid(this);
            else if(typeName == typeof(SturmCoulomb).FullName)
                return new SturmCoulomb(this);
            else if(typeName == typeof(QuantumEP).FullName)
                return new QuantumEP(this);
            else if(typeName == typeof(ClassicalEP).FullName)
                return new ClassicalEP(this);
            else if(typeName == typeof(LipkinFull).FullName)
                return new LipkinFull(this);
            else if(typeName == typeof(LipkinFactorized).FullName)
                return new LipkinFactorized(this);
            else if(typeName == typeof(LipkinOne).FullName)
                return new LipkinOne(this);
            else if(typeName == typeof(LipkinTwo).FullName)
                return new LipkinTwo(this);
            else
                return base.CreateObject(typeName);
        }
    }
}

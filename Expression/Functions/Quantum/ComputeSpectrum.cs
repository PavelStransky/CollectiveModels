using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.PT;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Computes spectrum of a LHOQuantumGCM object
    /// </summary>
    public class ComputeSpectrum : Fnc {
        public override string Help { get { return Messages.HelpComputeSpectrum; } }

        protected override void CreateParameters() {
            this.SetNumParams(6);

            this.SetParam(0, true, true, false, Messages.PLHOQuantumGCM, Messages.PLHOQuantumGCMDescription, null, typeof(LHOQuantumGCM), typeof(PT1));
            this.SetParam(1, true, true, false, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, null, typeof(int));
            this.SetParam(2, false, true, false, Messages.PEVectors, Messages.PEvectorsDescription, false, typeof(bool));
            this.SetParam(3, false, true, false, Messages.PNumEV, Messages.PNumEVDescription, 0, typeof(int));
            this.SetParam(4, false, true, false, Messages.PComputeMethod, Messages.PComputeMethodDescription, "lapackband", typeof(string));
            this.SetParam(5, false, true, false, Messages.PNumSteps, Messages.PNumStepsDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            object item = arguments[0];

            int maxE = (int)arguments[1];
            bool ev = (bool)arguments[2];
            int numev = (int)arguments[3];                  // 0 - berou se všechny vlastní hodnoty
            LHOQuantumGCM.ComputeMethod method = (LHOQuantumGCM.ComputeMethod)Enum.Parse(typeof(LHOQuantumGCM.ComputeMethod), (string)arguments[4], true);
            int numSteps = (int)arguments[5];               // 0 - numsteps je dopoèítáno automaticky

            if(item is LHOQuantumGCM)
                (item as LHOQuantumGCM).Compute(maxE, numSteps, ev, numev, guider, method);
            else if(item is PT1)
                (item as PT1).Compute(maxE, ev, numev, guider);

            return item;
        }
    }
}
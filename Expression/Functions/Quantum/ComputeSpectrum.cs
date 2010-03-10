using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Computes spectrum of a LHOQuantumGCM object
    /// </summary>
    public class ComputeSpectrum : Fnc {
        public override string Help { get { return Messages.HelpComputeSpectrum; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PQuantumSystem, Messages.PQuantumSystemDescription, null, typeof(IQuantumSystem));
            this.SetParam(1, true, true, false, Messages.PMaxEnergy, Messages.PMaxEnergyDescription, null, typeof(Vector), typeof(int));
            this.SetParam(2, false, true, false, Messages.PEVectors, Messages.PEvectorsDescription, false, typeof(bool));
            this.SetParam(3, false, true, false, Messages.PNumEV, Messages.PNumEVDescription, 0, typeof(int));
            this.SetParam(4, false, true, false, Messages.PComputeMethod, Messages.PComputeMethodDescription, "lapackband", typeof(string));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IQuantumSystem system = (IQuantumSystem)arguments[0];
            
            Vector max;
            if(arguments[1] is int) {
                max = new Vector(1);
                max[0] = (int)arguments[1];
            }
            else
                max = arguments[1] as Vector;

            bool ev = (bool)arguments[2];
            int numev = (int)arguments[3];                  // 0 - berou se všechny vlastní hodnoty
            ComputeMethod method = (ComputeMethod)Enum.Parse(typeof(ComputeMethod), (string)arguments[4], true);

            system.EigenSystem.Diagonalize(max, ev, numev, guider, method);
            return system;
        }
    }
}
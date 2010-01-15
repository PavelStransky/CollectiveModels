using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;
using PavelStransky.Core;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the energy for which the regular behavior starts breaking into the chaotic
    /// </summary>
    public class RegularityBreakSALI: Fnc {
        public override string Help { get { return Messages.HelpRegularityBreakSALI; } }

        protected override void CreateParameters() {
            this.SetNumParams(11);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, true, Messages.PEnergyMin, Messages.PEnergyMinDescription, null, typeof(double));
            this.SetParam(2, true, true, true, Messages.PEnergyMax, Messages.PEnergyMaxDescription, null, typeof(double));
            this.SetParam(3, true, true, false, Messages.PSizeX, Messages.PSizeXDescription, null, typeof(int));
            this.SetParam(4, true, true, false, Messages.PSizeY, Messages.PSizeYDescription, null, typeof(int));
            this.SetParam(5, false, true, true, Messages.PPrecisionEnergy, Messages.PPrecisionEnergyDescription, 1E-3, typeof(double));
            this.SetParam(6, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, "normal", typeof(string));
            this.SetParam(7, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 1E-3, typeof(double));
            this.SetParam(8, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, string.Empty, typeof(string));
            this.SetParam(9, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
            this.SetParam(10, false, true, false, Messages.PXSection, Messages.PXSectionDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;

            double emin = (double)arguments[1];
            double emax = (double)arguments[2];
            int sizex = (int)arguments[3];
            int sizey = (int)arguments[4];
            double precisione = (double)arguments[5];
            
            RungeKuttaMethods rkMethodT = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[6], true);
            double precisionT = (double)arguments[7];

            RungeKuttaMethods rkMethodW =
                (string)arguments[8] == string.Empty
                ? rkMethodT
                : (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[8], true);
            double precisionW =
                (double)arguments[9] <= 0.0
                ? precisionT
                : (double)arguments[9];
            
            bool isX = (bool)arguments[10];

            SALIContourGraph sali = new SALIContourGraph(dynamicalSystem, precisionT, rkMethodT, precisionW, rkMethodW);

            DateTime startTime = DateTime.Now;
            guider.WriteLine(string.Format(Messages.MsgRegularityBreak, emin, emax));
            guider.WriteLine(string.Format(Messages.MsgStartTime, startTime));

            while(System.Math.Abs(emax - emin) > precisione) {
                double e = (emax + emin) * 0.5;
                guider.Write(string.Format("E = {0} ", e));
                bool r = sali.IsRegularGraph(e, sizex, sizey, isX, guider);

                if(r)
                    emin = e;
                else
                    emax = e;
            }

            guider.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));

            return (emin + emax) * 0.5; 
        }
    }
}

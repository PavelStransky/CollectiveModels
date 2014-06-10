using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Creates matrix with Poincaré section by the plane y = 0 for 2D system; contours are determined by SALI
    /// </summary>
    public class SALIG: Fnc {
        public override string Help { get { return Messages.HelpSALIG; } }

        protected override void CreateParameters() {
            this.SetNumParams(10);

            this.SetParam(0, true, true, false, Messages.PDynamicalSystem, Messages.PDynamicalSystemDescription, null, typeof(IDynamicalSystem));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, true, true, false, Messages.PSizeX, Messages.PSizeXDescription, null, typeof(int));
            this.SetParam(3, true, true, false, Messages.PSizeY, Messages.PSizeYDescription, null, typeof(int));
            this.SetParam(4, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, "normal", typeof(string));
            this.SetParam(5, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
            this.SetParam(6, false, true, false, Messages.PRungeKuttaMethod, Messages.PRungeKuttaDescription, string.Empty, typeof(string));
            this.SetParam(7, false, true, true, Messages.PPrecision, Messages.PPrecisionDescription, 0.0, typeof(double));
            this.SetParam(8, false, true, false, Messages.PXSection, Messages.PXSectionDescription, false, typeof(bool), typeof(int));
            this.SetParam(9, false, true, false, Messages.POneOrientation, Messages.POneOrientationDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            IDynamicalSystem dynamicalSystem = arguments[0] as IDynamicalSystem;

            double e = (double)arguments[1];
            int sizex = (int)arguments[2];
            int sizey = (int)arguments[3];
            RungeKuttaMethods rkMethodT = (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[4], true);
            double precisionT = (double)arguments[5];
            
            RungeKuttaMethods rkMethodW =
                (string)arguments[6] == string.Empty
                ? rkMethodT
                : (RungeKuttaMethods)Enum.Parse(typeof(RungeKuttaMethods), (string)arguments[6], true);
            double precisionW =
                (double)arguments[7] <= 0.0
                ? precisionT
                : (double)arguments[7];

            int section = 0;

            if(arguments[8] is int)
                section = (int)arguments[8];
            else if(arguments[8] is bool)
                if((bool)arguments[8])
                    section = 0;
                else
                    section = 1;
            
            bool oneOrientation = (bool)arguments[9];

            SALIContourGraph sali = new SALIContourGraph(dynamicalSystem, precisionT, rkMethodT, precisionW, rkMethodW);
            ArrayList a = sali.Compute(e, sizex, sizey, section, oneOrientation, guider);

            List result = new List();
            result.AddRange(a);

            int count = result.Count;

            List sections = new List();
            List ics = new List();
            List salis = new List();

            ics.AddRange(result[count - 3] as ArrayList);
            sections.AddRange(result[count - 2] as ArrayList);
            salis.AddRange(result[count - 1] as ArrayList);

            result[count - 3] = ics;
            result[count - 2] = sections;
            result[count - 1] = salis;

            return result;
        }
    }
}

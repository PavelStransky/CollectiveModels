using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Core;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Returns the energy for which there should be, according to the geometrical theory of chaos,
    /// the transition between regular and chaotic behaviour
    /// </summary>
    public class RegularityBreakCurvature: Fnc {
        public override string Help { get { return Messages.HelpRegularityBreakCurvature; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(GCM));
            this.SetParam(1, true, true, true, Messages.PEnergyMin, Messages.PEnergyMinDescription, null, typeof(double));
            this.SetParam(2, true, true, true, Messages.PEnergyMax, Messages.PEnergyMaxDescription, null, typeof(double));
            this.SetParam(3, false, true, false, Messages.PDivision, Messages.PDivisionDescription, 0, typeof(int));
            this.SetParam(4, false, true, true, Messages.PPrecisionEnergy, Messages.PPrecisionEnergyDescription, defaultPrecision, typeof(double));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            GCM gcm = arguments[0] as GCM;

            double emin = (double)arguments[1];
            double emax = (double)arguments[2];

            int div = (int)arguments[3];

            double precisione = (double)arguments[4];

            DateTime startTime = DateTime.Now;
            guider.WriteLine(string.Format(Messages.MsgRegularityBreak, emin, emax));
            guider.WriteLine(string.Format(Messages.MsgStartTime, startTime));

            while(System.Math.Abs(emax - emin) > precisione) {
                double e = (emax + emin) * 0.5;
                guider.Write(string.Format("E = {0}...", e));

                guider.Write("E");
                PointVector[] equipotential = gcm.EquipotentialContours(e, 0, div);

                guider.Write("V");
                PointVector[] vzero = gcm.VMatrixContours(e, 0, div, 0);

                int el = equipotential.Length;
                int vl = vzero.Length;

                bool intersection = false;
                for(int i = 0; i < el; i++) {
                    for(int j = 0; j < vl; j++) {
                        // Existuje prùseèík køivek?
                        guider.Write("I");
                        if(equipotential[i].Intersection(vzero[j]).Length > 0) {
                            intersection = true;
                            break;
                        }
                        // Je køivka vzero celá uvnitø køivky equipotential?
                        guider.Write("P");
                        if(equipotential[i].IsInside(vzero[j][0])) {
                            intersection = true;
                            break;
                        }
                    }
                    if(intersection)
                        break;
                }

                if(intersection) {
                    guider.WriteLine("...Ch");
                    emax = e;
                }
                else {
                    guider.WriteLine("...R");
                    emin = e;
                }
            }

            guider.WriteLine(SpecialFormat.Format(DateTime.Now - startTime, true));

            return (emin + emax) * 0.5;
        }

        private const double defaultPrecision = 1E-3;
    }
}

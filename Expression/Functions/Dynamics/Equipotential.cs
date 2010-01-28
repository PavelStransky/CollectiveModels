using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For GCM system and given energy calculates equipotential line
    /// </summary>
    public class Equipotential: Fnc {
        public override string Help { get { return Messages.HelpEquipotential; } }

        protected override void CreateParameters() {
            this.SetNumParams(4);

            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(GCM));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, false, true, false, Messages.P3Equipotential, Messages.P3EquipotentialDescription, 0, typeof(int));
            this.SetParam(3, false, true, false, Messages.PDivision, Messages.PDivisionDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            GCM gcm = arguments[0] as GCM;
            double e = (double)arguments[1];

            PointVector[] equipotentials;
            equipotentials = gcm.EquipotentialContours(e, (int)arguments[2], (int)arguments[3]);

            int length = equipotentials.Length;
            TArray result = new TArray(typeof(PointVector), length);
            for(int i = 0; i < length; i++)
                result[i] = equipotentials[i];
            return result;
        }
    }
}

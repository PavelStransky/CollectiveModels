using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.GCM;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// For GCM system and given energy calculates equipotential line
    /// </summary>
    public class Equipotential: FunctionDefinition {
        public override string Help { get { return Messages.EquipotentialHelp; } }

        protected override void CreateParameters() {
            this.NumParams(3);

            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(PavelStransky.GCM.GCM));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, false, true, false, Messages.EquipotentialP3, Messages.EquipotentialP3Description, null, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            PavelStransky.GCM.GCM gcm = arguments[0] as PavelStransky.GCM.GCM;
            double e = (double)arguments[1];

            PointVector[] equipotentials;
            if(arguments[2] != null)
                equipotentials = gcm.EquipotentialContours(e, (int)arguments[2]);
            else
                equipotentials = gcm.EquipotentialContours(e);

            int length = equipotentials.Length;
            TArray result = new TArray(typeof(PointVector), length);
            for(int i = 0; i < length; i++)
                result[i] = equipotentials[i];
            return result;
        }
    }
}

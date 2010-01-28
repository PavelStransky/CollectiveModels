using System;
using System.Collections;

using PavelStransky.Expression;
using PavelStransky.Math;
using PavelStransky.Systems;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// For GCM system and given energy calculates the line where the given eigenvalue of V matrix is zero
    /// </summary>
    public class VMatrixZero: Fnc {
        public override string Help { get { return Messages.HelpVMatrixZero; } }

        protected override void CreateParameters() {
            this.SetNumParams(5);

            this.SetParam(0, true, true, false, Messages.PGCM, Messages.PGCMDescription, null, typeof(GCM));
            this.SetParam(1, true, true, true, Messages.PEnergy, Messages.PEnergyDescription, null, typeof(double));
            this.SetParam(2, false, true, false, Messages.P3Equipotential, Messages.P3EquipotentialDescription, 0, typeof(int));
            this.SetParam(3, false, true, false, Messages.PDivision, Messages.PDivisionDescription, 0, typeof(int));
            this.SetParam(4, false, true, false, Messages.PEigenValueIndex, Messages.PEigenValueIndexDescription, 0, typeof(int));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            GCM gcm = arguments[0] as GCM;
            double e = (double)arguments[1];

            PointVector[] equipotentials = gcm.VMatrixContours(e, (int)arguments[2], (int)arguments[3], (int)arguments[4]);

            int length = equipotentials.Length;
            TArray result = new TArray(typeof(PointVector), length);
            for(int i = 0; i < length; i++)
                result[i] = equipotentials[i];
            return result;
        }
    }
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Create a matrix of shifted vector (with a circle boundary conditions)
    /// </summary>
    public class ShiftMatrix : Fnc {
        public override string Help { get { return Messages.HelpShiftMatrix; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);
            this.SetParam(0, true, true, false, Messages.PVector, Messages.PVectorDescription, null, typeof(Vector));
            this.SetParam(1, true, true, false, Messages.PShift, Messages.PShiftDescription, null, typeof(TArray));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector vector = arguments[0] as Vector;
            TArray shifts = arguments[1] as TArray;

            int length = vector.Length;
            int count = shifts.Length;

            Matrix result = new Matrix(count, length);

            for(int i = 0; i < count; i++)
                for(int j = 0; j < length; j++)
                    result[i, j] = vector[(j + (int)shifts[i]) % length];

            return result;
        }
    }
}

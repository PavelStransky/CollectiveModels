using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Eigensystem of a general square matrix calculated using LAPACK library; 
    /// </summary>
    public class EigenSystem: Fnc {
        public override string Help { get { return Messages.HelpEigenSystem; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PSymmetricMatrix, Messages.PSymmetricMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, false, true, false, Messages.PEVectors, Messages.PEvectorsDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            bool ev = (bool)arguments[1];

            return (TArray)LAPackDLL.dgeev(m, ev);
        }
    }
}

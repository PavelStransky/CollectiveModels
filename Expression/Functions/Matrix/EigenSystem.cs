using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Expression.Functions.Def {
    /// <summary>
    /// Eigensystem of a general square complex matrix calculated using LAPACK library; 
    /// </summary>
    public class EigenSystem: Fnc {
        public override string Help { get { return Messages.HelpEigenSystem; } }

        protected override void CreateParameters() {
            this.SetNumParams(3);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, false, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(2, false, true, false, Messages.PEVectors, Messages.PEvectorsDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix mr = (Matrix)arguments[0];
            Matrix mi = (Matrix)arguments[1];
            bool ev = (bool)arguments[2];

            if(mi == null)
                return (TArray)LAPackDLL.dgeev(mr, ev);
            else {
                CMatrix cm = new CMatrix(mr, mi);
                return (TArray)LAPackDLL.zgeev(cm, ev, ev);
            }
        }
    }
}

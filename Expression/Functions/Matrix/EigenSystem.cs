using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;
using PavelStransky.DLLWrapper;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Eigensystem of a matrix calculated using LAPACK library; 
    /// before calculation it makes symmetrization of a matrix
    /// </summary>
    public class EigenSystem: FunctionDefinition {
        public override string Help { get { return Messages.HelpEigenSystem; } }

        protected override void CreateParameters() {
            this.SetNumParams(2);

            this.SetParam(0, true, true, false, Messages.PSymmetricMatrix, Messages.PSymmetricMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, false, true, false, Messages.PEVectors, Messages.PEvectorsDescription, false, typeof(bool));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            m = m.Symmetrize();

            bool ev = (bool)arguments[1];

            if(ev)
                return (TArray)LAPackDLL.dsyev(m, ev);
            else
                return LAPackDLL.dsyev(m, ev)[0];
        }
    }
}

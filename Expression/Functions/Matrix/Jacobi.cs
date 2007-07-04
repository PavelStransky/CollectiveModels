using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
    /// Eigensystem of a matrix calculated using Jacobi method; 
    /// before calculation it makes symmetrization of a matrix
	/// </summary>
	public class FnJacobi: FunctionDefinition {
        public override string Name { get { return name; } }
		public override string Help {get {return Messages.HelpJacobi;}}

        protected override void CreateParameters() {
            this.NumParams(1);

            this.SetParam(0, true, true, false, Messages.PSymmetricMatrix, Messages.PSymmetricMatrixDescription, null, typeof(Matrix));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
            m = m.Symmetrize();
			Jacobi jacobi = new Jacobi(m);
			
            jacobi.SortAsc();
			TArray result = new TArray(typeof(Vector), m.Length + 1);

			result[0] = new Vector(jacobi.EigenValue);
			for(int i = 0; i < jacobi.EigenVector.Length; i++)
				result[i + 1] = jacobi.EigenVector[i];

			return result;
		}

        private const string name = "jacobi";
	}
}

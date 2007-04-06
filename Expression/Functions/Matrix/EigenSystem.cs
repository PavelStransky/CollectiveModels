using System;
using System.Collections;

using PavelStransky.GCM;
using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vypo��t� syst�m vlastn�ch hodnot a vektor� symetrick� matice
	/// </summary>
	public class EigenSystem: FunctionDefinition {
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}

		protected override void CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 1);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(Matrix));
		}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];
			Jacobi jacobi = new Jacobi(m);
			
            jacobi.SortAsc();
			TArray result = new TArray(typeof(Vector), m.Length + 1);

			result[0] = new Vector(jacobi.EigenValue);
			for(int i = 0; i < jacobi.EigenVector.Length; i++)
				result[i + 1] = jacobi.EigenVector[i];

			return result;
		}

		private const string help = "Vypo��t� syst�m vlastn�ch hodnot a vektor� pro symetrickou matici. Vrac� �adu vektor�, jej� prvn� prvek obsahuje vlastn� hodnoty, zbytek prvk� vlastn� vektory";
		private const string parameters = "Matrix";
	}
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvo�� PointVector
	/// </summary>
	public class FnPointVector: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}
		
		protected override void CheckArguments(ArrayList evaluatedArguments) {
			this.CheckArgumentsNumber(evaluatedArguments, 2);
            this.CheckArgumentsType(evaluatedArguments, 0, typeof(Vector));
            this.CheckArgumentsType(evaluatedArguments, 1, typeof(Vector));
		}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Vector v1 = arguments[0] as Vector;
            Vector v2 = arguments[1] as Vector;

            if(v1.Length != v2.Length)
                throw new FunctionDefinitionException(string.Format(errorMessageNotEqualLengthVector, this.Name),
                    string.Format(errorMessageNotEqualLengthVectorDetail, v1.Length, v2.Length));

            PointVector result = new PointVector(v1.Length);
            for(int i = 0; i < result.Length; i++)
                result[i] = new PointD(v1[i], v2[i]);

            return result;
        }

		private const string errorMessageNotEqualLengthVector = "Pro vytvo�en� �ady vektor� pomoc� funkce '{0}' je nutn�, aby vstupn� vektory m�ly stejnou d�lku.";
		private const string errorMessageNotEqualLengthVectorDetail = "D�lka prvn�ho vektoru: {0}\nD�lka druh�ho vektoru: {1}";

		private const string name = "pointvector";
		private const string help = "Vytvo�� vektor bod� (PointVector)";
		private const string parameters = "x (Vector); y (Vector)";
	}
}

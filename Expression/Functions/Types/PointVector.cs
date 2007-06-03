using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Vytvoøí PointVector
	/// </summary>
	public class FnPointVector: FunctionDefinition {
		public override string Name {get {return name;}}
		public override string Help {get {return help;}}
		public override string Parameters {get {return parameters;}}
		
		protected override void CheckArguments(ArrayList evaluatedArguments, bool evaluateArray) {
			this.CheckArgumentsMinNumber(evaluatedArguments, 1);
            this.CheckArgumentsMaxNumber(evaluatedArguments, 2);

            if(evaluatedArguments.Count == 1) {
                this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(Vector), typeof(List), typeof(TArray));
            }
            else {
                this.CheckArgumentsType(evaluatedArguments, 0, evaluateArray, typeof(Vector));
                this.CheckArgumentsType(evaluatedArguments, 1, evaluateArray, typeof(Vector));
            }
		}

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            if(arguments.Count == 1) {
                if(arguments[0] is Vector)
                    return new PointVector(arguments[0] as Vector);

                else if(arguments[0] is List) {
                    List l = arguments[0] as List;
                    int c = l.Count;

                    int i = 0;
                    PointVector result = new PointVector(c);
                    foreach(object o in l) {
                        if(o is PointD)
                            result[i++] = (PointD)o;
                        else
                            this.BadTypeError(arguments, 0);
                    }

                    return result;
                }

                else if(arguments[0] is TArray) {
                    TArray a = arguments[0] as TArray;
                    int c = a.GetNumElements();

                    int i = 0;
                    PointVector result = new PointVector(c);
                    a.ResetEnumerator();

                    foreach(object o in a) {
                        if(o is PointD)
                            result[i++] = (PointD)o;
                        else
                            this.BadTypeError(o, 0);
                    }
                }
            }
            else {
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

            return null;
        }

		private const string errorMessageNotEqualLengthVector = "Pro vytvoøení øady vektorù pomocí funkce '{0}' je nutné, aby vstupní vektory mìly stejnou délku.";
		private const string errorMessageNotEqualLengthVectorDetail = "Délka prvního vektoru: {0}\nDélka druhého vektoru: {1}";

		private const string name = "pointvector";
		private const string help = "Vytvoøí vektor bodù (PointVector)";
		private const string parameters = "(x (Vector); y (Vector)) | (Seznam bodù (List))";
	}
}

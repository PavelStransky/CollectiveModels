using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
	/// Converts given data to a pointvector
	/// </summary>
	public class FnPointVector: FunctionDefinition {
		public override string Name {get {return name;}}
        public override string Help { get { return Messages.HelpPointVector; } }

        protected override void CreateParameters() {
            this.NumParams(2);

            this.SetParam(0, true, true, false, Messages.P1PointVector, Messages.P1PointVectorDescription, null,
                typeof(Vector), typeof(List), typeof(TArray));
            this.SetParam(1, false, true, false, Messages.P2PointVector, Messages.P2PointVectorDescription, null, typeof(Vector));
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
                    throw new FunctionDefinitionException(string.Format(Messages.EMNotEqualVectorLength, this.Name),
                        string.Format(Messages.EMNotEqualVectorLengthDetail, v1.Length, v2.Length));

                PointVector result = new PointVector(v1.Length);
                for(int i = 0; i < result.Length; i++)
                    result[i] = new PointD(v1[i], v2[i]);

                return result;
            }

            return null;
        }

		private const string name = "pointvector";
	}
}

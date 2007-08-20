using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
    /// Returns selected columns from a matrix
	/// </summary>
	public class GetColumns: FunctionDefinition {
		public override string Help {get {return Messages.HelpGetColumns;}}

        protected override void CreateParameters() {
            this.SetNumParams(2, true);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, false, Messages.PIndexColumn, Messages.PIndexColumnDescription, null,
                typeof(int), typeof(TArray));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];

            List columnsToGet = new List();

            foreach(object o in arguments) {
                if(o is int)
                    columnsToGet.Add(o);
                else if(o is TArray) {
                    TArray ta = o as TArray;
                    if(ta.GetItemType() != typeof(int))
                        this.BadTypeError(ta[0], 1);

                    ta.ResetEnumerator();
                    foreach(int i in ta)
                        columnsToGet.Add(i);
                }
            }
            
            int lengthX = m.LengthX;
            int n = columnsToGet.Count;

			Matrix result = new Matrix(lengthX, n);
            for(int i = 0; i < lengthX; i++) {
                int j = 0;
                foreach(int k in columnsToGet) {
                    result[i, j++] = m[i, k];
                }
            }

			return result;
		}
	}
}

using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
	/// <summary>
    /// Returns selected rows from a matrix
	/// </summary>
	public class GetRows: FunctionDefinition {
		public override string Help {get {return Messages.HelpGetRows;}}

        protected override void CreateParameters() {
            this.NumParams(2, true);

            this.SetParam(0, true, true, false, Messages.PMatrix, Messages.PMatrixDescription, null, typeof(Matrix));
            this.SetParam(1, true, true, false, Messages.PIndexRow, Messages.PIndexRowDescription, null,
                typeof(int), typeof(TArray));
        }

        protected override object EvaluateFn(Guider guider, ArrayList arguments) {
            Matrix m = (Matrix)arguments[0];

            List rowsToGet = new List();

            foreach(object o in arguments) {
                if(o is int)
                    rowsToGet.Add(o);
                else if(o is TArray) {
                    TArray ta = o as TArray;
                    if(ta.GetItemType() != typeof(int))
                        this.BadTypeError(ta[0], 1);

                    ta.ResetEnumerator();
                    foreach(int i in ta)
                        rowsToGet.Add(i);
                }
            }
            
            int lengthY = m.LengthY;
            int n = rowsToGet.Count;

			Matrix result = new Matrix(n, lengthY);
            for(int j = 0; j < lengthY; j++) {
                int i = 0;
                foreach(int k in rowsToGet) {
                    result[i++, j] = m[k, j];
                }
            }

			return result;
		}
	}
}

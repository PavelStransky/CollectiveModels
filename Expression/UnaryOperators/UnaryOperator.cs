using System;

using PavelStransky.Math;
using PavelStransky.Expression.Operators;

namespace PavelStransky.Expression.UnaryOperators {
	/// <summary>
	/// T��da implementuj�c� un�rn� oper�tory
	/// </summary>
	public abstract class UnaryOperator: Operator {
		/// <summary>
		/// V�po�et v�sledku oper�toru
		/// </summary>
		/// <param name="left">Lev� ��st oper�toru</param>
		/// <param name="right">Prav� ��st oper�toru</param>
		public virtual object Evaluate(object item) {
			if(item is int)
				return this.EvaluateI((int)item);
			else if(item is double)
				return this.EvaluateD((double)item);
			else if(item is PointD)
				return this.EvaluateP((PointD)item);
			else if(item is Vector)
				return this.EvaluateV((Vector)item);
			else if(item is PointVector)
				return this.EvaluatePv((PointVector)item);
			else if(item is Matrix)
				return this.EvaluateM((Matrix)item);
			else if(item is string)
				return this.EvaluateS((string)item);
			else if(item is TArray) 
				return this.EvaluateA((TArray)item);
			else
				return this.UnknownType(item);
		}

		#region Evaluate functions
		protected virtual object EvaluateI(int item) {
			return this.UnknownType(item);
		}

		protected virtual object EvaluateD(double item) {
			return this.UnknownType(item);
		}

		protected virtual object EvaluateP(PointD item) {
			return this.UnknownType(item);
		}

		protected virtual object EvaluateV(Vector item) {
			return this.UnknownType(item);
		}

		protected virtual object EvaluatePv(PointVector item) {
			return this.UnknownType(item);
		}

		protected virtual object EvaluateM(Matrix item) {
			return this.UnknownType(item);
		}

		protected virtual object EvaluateS(string item) {
			return this.UnknownType(item);
		}

		protected virtual object EvaluateA(TArray item) {
            TArray result = null;

            item.ResetEnumerator();
            int[] index = (int[])item.StartEnumIndex.Clone();
            int[] lengths = item.Lengths;
            int rank = item.Rank;

            do {
                object o = this.Evaluate(item[index]);
                if(result == null)
                    result = new TArray(o.GetType(), lengths);

                result[index] = o;
            }
            while(TArray.MoveNext(rank, index, item.StartEnumIndex, item.EndEnumIndex));

            return result;
        }
		#endregion

		/// <summary>
		/// V�jimka - nezn�m� typ pro v�po�et v�razu
		/// </summary>
		/// <param name="item">V�raz</param>
		protected object UnknownType(object item) {
			throw new OperatorException(string.Format(errorMessageUnknownType, this.OperatorName, item.GetType().FullName));
		}

		private const string errorMessageUnknownType = "Oper�tor '{0}' nelze pou��t pro typ {1}.";
	}
}

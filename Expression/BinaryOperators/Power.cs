using System;
using System.Text;

using PavelStransky.Math;
using PavelStransky.Expression.Operators;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// Operátor mocnina po prvcích
	/// </summary>
	public class Power: BinaryOperator {
		public override string OperatorName {get {return operatorName;}}
		public override int Priority {get {return powerPriority;}}

		protected override object EvaluateDDx(double left, double right) {
			return System.Math.Pow(left, right);
		}

		protected override object EvaluateDVx(double left, Vector right) {
			Vector result = new Vector(right.Length);

			for(int i = 0; i < result.Length; i++)
				result[i] = System.Math.Pow(left, right[i]);
			
			return result;
		}

		protected override object EvaluateDMx(double left, Matrix right) {
			Matrix result = new Matrix(right.LengthX, right.LengthY);

			for(int i = 0; i < result.LengthX; i++)
				for(int j = 0; j < result.LengthY; j++)
					result[i, j] = System.Math.Pow(left, right[i, j]);
			
			return result;
		}

		protected override object EvaluateVDx(Vector left, double right) {
			Vector result = new Vector(left.Length);

			for(int i = 0; i < result.Length; i++)
				result[i] = System.Math.Pow(left[i], right);
			
			return result;
		}

		protected override object EvaluateVVx(Vector left, Vector right) {
			if(left.Length != right.Length)
				throw new OperatorException(string.Format(errorMessageNotEqualLengthVector, this.OperatorName, left.Length, right.Length));

			Vector result = new Vector(left.Length);

			for(int i = 0; i < result.Length; i++)
				result[i] = System.Math.Pow(left[i], right[i]);
			
			return result;
		}

		protected override object EvaluateMDx(Matrix left, double right) {
			Matrix result = new Matrix(left.LengthX, left.LengthY);

			for(int i = 0; i < result.LengthX; i++)
				for(int j = 0; j < result.LengthY; j++)
					result[i, j] = System.Math.Pow(left[i, j], right);
			
			return result;
		}

		protected override object EvaluateMMx(Matrix left, Matrix right) {
			if(left.LengthX != right.LengthX || left.LengthY != right.LengthY)
				throw new OperatorException(string.Format(errorMessageNotEqualLengthMatrix, this.OperatorName, left.LengthX, left.LengthY, right.LengthX, right.LengthY));

			Matrix result = new Matrix(left.LengthX, left.LengthY);

			for(int i = 0; i < result.LengthX; i++)
				for(int j = 0; j < result.LengthY; j++)
					result[i, j] = System.Math.Pow(left[i, j], right[i, j]);
			
			return result;
		}

		protected override object EvaluateSI(string left, int right) {
			StringBuilder result = new StringBuilder(left.Length * right);
			for(int i = 0; i < right; i++)
				result.Append(left);
			return result.ToString();
		}

		private const string errorMessageNotEqualLengthVector = "Pro použití operátoru '{0}' mezi vektory je nutné, aby jejich rozmìry ({1}, {2}) byly shodné.";
		private const string errorMessageNotEqualLengthMatrix = "Pro použití operátoru '{0}' mezi maticemi je nutné, aby jejich rozmìry (({1},{2}), {{3},{4})) byly shodné.";

		private const string operatorName = "^^";
	}
}

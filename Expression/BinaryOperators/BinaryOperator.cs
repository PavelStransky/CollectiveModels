using System;

using PavelStransky.Math;
using PavelStransky.Expression.Operators;

namespace PavelStransky.Expression.BinaryOperators {
	/// <summary>
	/// T��da implementuj�c� bin�rn� oper�tory
	/// </summary>
	public abstract class BinaryOperator: Operator {
		/// <summary>
		/// Priorita oper�toru (��m ni���, t�m se d�l� d��ve)
		/// </summary>
		public abstract int Priority {get;}

		/// <summary>
		/// Maxim�ln� priorita
		/// </summary>
		public static int MaxPriority {get {return maxPriority;}}

		/// <summary>
		/// V�po�et v�sledku oper�toru
		/// </summary>
		/// <param name="left">Lev� ��st oper�toru</param>
		/// <param name="right">Prav� ��st oper�toru</param>
		public virtual object Evaluate(object left, object right) {
            if(left is int)
                return this.EvaluateI((int)left, right);
            else if(left is double)
                return this.EvaluateD((double)left, right);
            else if(left is PointD)
                return this.EvaluateP((PointD)left, right);
            else if(left is Vector)
                return this.EvaluateV((Vector)left, right);
            else if(left is PointVector)
                return this.EvaluatePv((PointVector)left, right);
            else if(left is Matrix)
                return this.EvaluateM((Matrix)left, right);
            else if(left is string)
                return this.EvaluateS((string)left, right);
            else if(left is Array)
                return this.EvaluateA((Array)left, right);
            else if(left is DateTime)
                return this.EvaluateTime((DateTime)left, right);
            else
                return this.UnknownType(left, right);
		}

		#region Evaluate functions
		protected virtual object EvaluateI(int left, object right) {
			if(right is int)
				return this.EvaluateII(left, (int)right);
			else if(right is double)
				return this.EvaluateID(left, (double)right);
			else if(right is PointD)
				return this.EvaluateIP(left, (PointD)right);
			else if(right is Vector)
				return this.EvaluateIV(left, (Vector)right);
			else if(right is PointVector)
				return this.EvaluateIPv(left, (PointVector)right);
			else if(right is Matrix)
				return this.EvaluateIM(left, (Matrix)right);
			else if(right is string)
				return this.EvaluateIS(left, (string)right);
			else if(right is Array) 
				return this.EvaluateIA(left, (Array)right);
			else
				return this.UnknownType(left, right);
		}

		protected virtual object EvaluateD(double left, object right) {
			if(right is int)
				return this.EvaluateDI(left, (int)right);
			else if(right is double)
				return this.EvaluateDDx(left, (double)right);
			else if(right is PointD)
				return this.EvaluateDPx(left, (PointD)right);
			else if(right is Vector)
				return this.EvaluateDVx(left, (Vector)right);
			else if(right is PointVector)
				return this.EvaluateDPvx(left, (PointVector)right);
			else if(right is Matrix)
				return this.EvaluateDMx(left, (Matrix)right);
			else if(right is string)
				return this.EvaluateDS(left, (string)right);
			else if(right is Array) 
				return this.EvaluateDA(left, (Array)right);
			else
				return this.UnknownType(left, right);
		}

		protected virtual object EvaluateP(PointD left, object right) {
			if(right is int)
				return this.EvaluatePI(left, (int)right);
			else if(right is double)
				return this.EvaluatePDx(left, (double)right);
			else if(right is PointD)
				return this.EvaluatePPx(left, (PointD)right);
			else if(right is Vector)
				return this.EvaluatePVx(left, (Vector)right);
			else if(right is PointVector)
				return this.EvaluatePPvx(left, (PointVector)right);
			else if(right is Matrix)
				return this.EvaluatePMx(left, (Matrix)right);
			else if(right is string)
				return this.EvaluatePS(left, (string)right);
			else if(right is Array) 
				return this.EvaluatePA(left, (Array)right);
			else
				return this.UnknownType(left, right);
		}

		protected virtual object EvaluateV(Vector left, object right) {
			if(right is int)
				return this.EvaluateVI(left, (int)right);
			else if(right is double)
				return this.EvaluateVDx(left, (double)right);
			else if(right is PointD)
				return this.EvaluateVPx(left, (PointD)right);
			else if(right is Vector)
				return this.EvaluateVVx(left, (Vector)right);
			else if(right is PointVector)
				return this.EvaluateVPvx(left, (PointVector)right);
			else if(right is Matrix)
				return this.EvaluateVMx(left, (Matrix)right);
			else if(right is string)
				return this.EvaluateVS(left, (string)right);
			else if(right is Array) 
				return this.EvaluateVA(left, (Array)right);
			else
				return this.UnknownType(left, right);
		}

		protected virtual object EvaluatePv(PointVector left, object right) {
			if(right is int)
				return this.EvaluatePvI(left, (int)right);
			else if(right is double)
				return this.EvaluatePvDx(left, (double)right);
			else if(right is PointD)
				return this.EvaluatePvPx(left, (PointD)right);
			else if(right is Vector)
				return this.EvaluatePvVx(left, (Vector)right);
			else if(right is PointVector)
				return this.EvaluatePvPvx(left, (PointVector)right);
			else if(right is Matrix)
				return this.EvaluatePvMx(left, (Matrix)right);
			else if(right is string)
				return this.EvaluatePvS(left, (string)right);
			else if(right is Array) 
				return this.EvaluatePvA(left, (Array)right);
			else
				return this.UnknownType(left, right);
		}

		protected virtual object EvaluateM(Matrix left, object right) {
			if(right is int)
				return this.EvaluateMI(left, (int)right);
			else if(right is double)
				return this.EvaluateMDx(left, (double)right);
			else if(right is PointD)
				return this.EvaluateMPx(left, (PointD)right);
			else if(right is Vector)
				return this.EvaluateMVx(left, (Vector)right);
			else if(right is PointVector)
				return this.EvaluateMPvx(left, (PointVector)right);
			else if(right is Matrix)
				return this.EvaluateMMx(left, (Matrix)right);
			else if(right is string)
				return this.EvaluateMS(left, (string)right);
			else if(right is Array) 
				return this.EvaluateMA(left, (Array)right);
			else
				return this.UnknownType(left, right);
		}
		
		protected virtual object EvaluateS(string left, object right) {
			if(right is int)
				return this.EvaluateSI(left, (int)right);
			else if(right is double)
				return this.EvaluateSD(left, (double)right);
			else if(right is PointD)
				return this.EvaluateSP(left, (PointD)right);
			else if(right is Vector)
				return this.EvaluateSV(left, (Vector)right);
			else if(right is PointVector)
				return this.EvaluateSPv(left, (PointVector)right);
			else if(right is Matrix)
				return this.EvaluateSM(left, (Matrix)right);
			else if(right is string)
				return this.EvaluateSSx(left, (string)right);
			else if(right is Array) 
				return this.EvaluateSA(left, (Array)right);
			else
				return this.EvaluateSSx(left, right.ToString());
		}

        protected virtual object EvaluateTime(DateTime left, object right) {
            if(right is DateTime)
                return this.EvaluateTimeTimex(left, (DateTime)right);
            else
                return this.UnknownType(left, right);
        }

		protected virtual object EvaluateA(Array left, object right) {
			Array result = new Array();

			if(right is Array) {
				Array r = right as Array;

				if(left.Count != r.Count)
					throw new OperatorException(string.Format(errorMessageNotEqualLength, this.OperatorName, left.Count, r.Count));

				for(int i = 0; i < left.Count; i++)
					result.Add(this.Evaluate(left[i], r[i]));
			}
			else {
				for(int i = 0; i < left.Count; i++)
					result.Add(this.Evaluate(left[i], right));
			}
				
			return result;
		}

		protected virtual object EvaluateII(int left, int right) {
			return this.EvaluateDDx((double)left, (double)right);
		}

		protected virtual object EvaluateID(int left, double right) {
			return this.EvaluateDDx((double)left, right);
		}

		protected virtual object EvaluateIP(int left, PointD right) {
			return this.EvaluateDPx((double)left, right);
		}

		protected virtual object EvaluateIV(int left, Vector right) {
			return this.EvaluateDVx((double)left, right);
		}

		protected virtual object EvaluateIPv(int left, PointVector right) {
			return this.EvaluateDPvx((double)left, right);
		}

		protected virtual object EvaluateIM(int left, Matrix right) {
			return this.EvaluateDMx((double)left, right);
		}

		protected virtual object EvaluateIS(int left, string right) {
			return this.EvaluateSSx(left.ToString(), right);
		}

		protected virtual object EvaluateIA(int left, Array right) {
			Array result = new Array();
			for(int i = 0; i < right.Count; i++)
				result.Add(this.EvaluateI(left, right[i]));
			return result;
		}

		protected virtual object EvaluateDI(double left, int right) {
			return this.EvaluateDDx(left, (double)right);
		}

		protected virtual object EvaluateDDx(double left, double right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateDPx(double left, PointD right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateDVx(double left, Vector right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateDPvx(double left, PointVector right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateDMx(double left, Matrix right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateDS(double left, string right) {
			return this.EvaluateSSx(left.ToString(), right);
		}

		protected virtual object EvaluateDA(double left, Array right) {
			Array result = new Array();
			for(int i = 0; i < right.Count; i++)
				result.Add(this.EvaluateD(left, right[i]));
			return result;
		}

		protected virtual object EvaluatePI(PointD left, int right) {
			return this.EvaluatePDx(left, (double)right);
		}

		protected virtual object EvaluatePDx(PointD left, double right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluatePPx(PointD left, PointD right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluatePVx(PointD left, Vector right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluatePPvx(PointD left, PointVector right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluatePMx(PointD left, Matrix right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluatePS(PointD left, string right) {
			return this.EvaluateSSx(left.ToString(), right);
		}

		protected virtual object EvaluatePA(PointD left, Array right) {
			Array result = new Array();
			for(int i = 0; i < right.Count; i++)
				result.Add(this.EvaluateP(left, right[i]));
			return result;
		}

		protected virtual object EvaluateVI(Vector left, int right) {
			return this.EvaluateVDx(left, (double)right);
		}	

		protected virtual object EvaluateVDx(Vector left, double right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateVPx(Vector left, PointD right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateVVx(Vector left, Vector right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateVPvx(Vector left, PointVector right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateVMx(Vector left, Matrix right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateVS(Vector left, string right) {
			return this.EvaluateSSx(left.ToString(), right);
		}

		protected virtual object EvaluateVA(Vector left, Array right) {
			Array result = new Array();
			for(int i = 0; i < right.Count; i++)
				result.Add(this.EvaluateV(left, right[i]));
			return result;
		}

		protected virtual object EvaluatePvI(PointVector left, int right) {
			return this.EvaluatePvDx(left, (double)right);
		}	

		protected virtual object EvaluatePvDx(PointVector left, double right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluatePvPx(PointVector left, PointD right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluatePvVx(PointVector left, Vector right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluatePvPvx(PointVector left, PointVector right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluatePvMx(PointVector left, Matrix right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluatePvS(PointVector left, string right) {
			return this.EvaluateSSx(left.ToString(), right);
		}

		protected virtual object EvaluatePvA(PointVector left, Array right) {
			Array result = new Array();
			for(int i = 0; i < right.Count; i++)
				result.Add(this.EvaluatePv(left, right[i]));
			return result;
		}

		protected virtual object EvaluateMI(Matrix left, int right) {
			return this.EvaluateMDx(left, (double)right);
		}

		protected virtual object EvaluateMDx(Matrix left, double right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateMPx(Matrix left, PointD right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateMVx(Matrix left, Vector right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateMPvx(Matrix left, PointVector right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateMMx(Matrix left, Matrix right) {
			return this.UnknownType(left, right);
		}
		
		protected virtual object EvaluateMS(Matrix left, string right) {
			return this.EvaluateSSx(left.ToString(), right);
		}

		protected virtual object EvaluateMA(Matrix left, Array right) {
			Array result = new Array();
			for(int i = 0; i < right.Count; i++)
				result.Add(this.EvaluateM(left, right[i]));
			return result;
		}

		protected virtual object EvaluateSI(string left, int right) {
			return this.EvaluateSSx(left, right.ToString());
		}

		protected virtual object EvaluateSD(string left, double right) {
			return this.EvaluateSSx(left, right.ToString());
		}

		protected virtual object EvaluateSP(string left, PointD right) {
			return this.EvaluateSSx(left, right.ToString());
		}

		protected virtual object EvaluateSV(string left, Vector right) {
			return this.EvaluateSSx(left, right.ToString());
		}

		protected virtual object EvaluateSPv(string left, PointVector right) {
			return this.EvaluateSSx(left, right.ToString());
		}

		protected virtual object EvaluateSM(string left, Matrix right) {
			return this.EvaluateSSx(left, right.ToString());
		}

		protected virtual object EvaluateSSx(string left, string right) {
			return this.UnknownType(left, right);
		}

		protected virtual object EvaluateSA(string left, Array right) {
			Array result = new Array();
			for(int i = 0; i < right.Count; i++)
				result.Add(this.EvaluateS(left, right[i]));
			return result;
		}

        protected virtual object EvaluateTimeTimex(DateTime left, DateTime right) {
            return this.UnknownType(left, right);
        }
        #endregion

		/// <summary>
		/// V�jimka - nezn�m� typ pro v�po�et v�razu
		/// </summary>
		/// <param name="left">Lev� v�raz</param>
		/// <param name="right">Prav� v�raz</param>
		protected object UnknownType(object left, object right) {
			throw new OperatorException(string.Format(errorMessageUnknownType, this.OperatorName, left.GetType().FullName, right.GetType().FullName));
		}

		private const string errorMessageUnknownType = "Oper�tor '{0}' nelze pou��t pro typy {1} {0} {2}.";
		private const string errorMessageNotEqualLength = "Pro pou�it� oper�toru '{0}' mezi �adami je nutn�, aby d�lky �ad ({1}, {2}) byly shodn�.";

		protected const int comparePriority = 0;
		protected const int powerPriority = 1;
		protected const int multiplePriority = 2;
		protected const int addPriority = 3;
		protected const int maxPriority = 3;
	}
}

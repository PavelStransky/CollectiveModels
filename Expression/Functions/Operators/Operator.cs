using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Priorita operátoru
    /// </summary>
    public enum OperatorPriority {
        AssignmentPriority = 0,
        BoolAddPriority = 1,
        BoolMultiplePriority = 2,
        ComparePriority = 3,
        JoinPriority = 4,
        IntervalPriority = 5,
        AddPriority = 6,
        MultiplePriority = 7,
        PowerPriority = 8,
        MaxPriority = 10
    }
    
    /// <summary>
	/// Tøída pøedcházející všechny operátory
	/// </summary>
	public abstract class Operator: FunctionDefinition {
        // Kompatibilita mezi typy v poøadí, v jakém jsou zadány mezi parametry
        // (reversed znamená, že jsou uchovávány NEPLATNÉ kombinace)
        private ArrayList reversedCompatibility;

        /// <summary>
        /// Priorita operátoru (èím nižší, tím se dìlá døíve)
        /// </summary>
        public abstract OperatorPriority Priority { get;}

        /// <summary>
        /// Maximální priorita
        /// </summary>
        public static OperatorPriority MaxPriority { get { return OperatorPriority.MaxPriority; } }

        public override object Evaluate(Guider guider, ArrayList arguments, bool evaluateArray) {
            return base.Evaluate(guider, arguments, true);      // V pøípadì operátorù vždy poèítáme øadu
        }

        /// <summary>
        /// Pøidá do seznamu zadanou kompatibilitu
        /// </summary>
        /// <param name="t1">Typ 1</param>
        /// <param name="t2">Typ 2</param>
        protected void AddCompatibility(Type t1, Type t2) {
            Type[] types = this.Parameters[0].Types;
            int length = types.Length;

            if(this.reversedCompatibility == null) {
                this.reversedCompatibility = new ArrayList();

                for(int i = 0; i < length; i++)
                    for(int j = i + 1; j < length; j++) {
                        int[] a = new int[2];
                        a[0] = i;
                        a[1] = j;
                        this.reversedCompatibility.Add(a);
                    }
            }

            int[] r = new int[2];
            for(int i = 0; i < length; i++) {
                if(types[i] == t1)
                    r[0] = i;
                if(types[i] == t2)
                    r[1] = i;
            }

            int k = 0;
            bool found = false;
            foreach(int[] a in this.reversedCompatibility) {
                if(a[0] == r[0] && a[1] == r[1]) {
                    found = true;
                    break;
                }
                k++;
            }

            if(found)
                this.reversedCompatibility.RemoveAt(k);
        }

        /// <summary>
        /// Returns the (maximal) lengths of each of the possible types;
        /// if the type is not present, returns -1
        /// It works properly only for functions with one argument
        /// </summary>
        /// <param name="arguments">Arguments of the function</param>
        /// <param name="checkSize">True if different sizes of one type is not allowed</param>
        protected int[] GetTypesLength(ArrayList arguments, bool checkSize) {
            Type[] types = this.Parameters[0].Types;

            int length = types.Length;
            int[] result = new int[2 * length];

            for(int i = 0; i < length; i++) {
                int lx = -1;
                int ly = -1;
                Type type = types[i];

                if(type == typeof(Vector)) {
                    foreach(object o in arguments)
                        if(o is Vector)
                            lx = this.SetLength(lx, (o as Vector).Length, checkSize, type);
                }

                else if(type == typeof(PointVector)) {
                    foreach(object o in arguments)
                        if(o is PointVector)
                            lx = this.SetLength(lx, (o as PointVector).Length, checkSize, type);
                }

                else if(type == typeof(Matrix)) {
                    foreach(object o in arguments)
                        if(o is Matrix) {
                            lx = this.SetLength(lx, (o as Matrix).LengthX, checkSize, type);
                            ly = this.SetLength(ly, (o as Matrix).LengthY, checkSize, type);
                        }
                }

                else if(type == typeof(List)) {
                    foreach(object o in arguments)
                        if(o is List)
                            lx = this.SetLength(lx, (o as List).Count, checkSize, type);
                }

                    // Other type, we must check also parents
                else {
                    if(type.IsInterface) {
                        foreach(object o in arguments)
                            if(o != null) {
                                Type[] interfaces = o.GetType().FindInterfaces(this.InterfaceFilter, type.FullName);
                                if(interfaces != null && interfaces.Length > 0) {
                                    lx = 1;
                                    break;
                                }
                            }
                    }

                    else {
                        foreach(object o in arguments)
                            if(o != null) {
                                Type t = o.GetType();

                                do {
                                    if(t == type) {
                                        lx = 1;
                                        break;
                                    }
                                    t = t.BaseType;
                                } while(t != null);
                            }
                    }
                }

                result[2 * i] = lx;
                result[2 * i + 1] = ly;
            }

            this.CheckCompatibility(result);

            return result;
        }

        /// <summary>
        /// Checks whether among given parameters there are only valid combinations 
        /// </summary>
        /// <param name="lengths">Lengths of each of the parameters</param>
        private void CheckCompatibility(int[] lengths) {
            foreach(int[] i in this.reversedCompatibility) {
                if(lengths[2 * i[0]] >= 0 && lengths[2 * i[1]] >= 0)
                    throw new FunctionDefinitionException(
                        string.Format(Messages.EMParametersCompatibility, this.Name, this.Parameters[0].Types[i[0]].FullName, this.Parameters[0].Types[i[1]].FullName));
            }
        }

        /// <summary>
        /// Sets new length and checks whether the new length is equal to the old one
        /// </summary>
        /// <param name="oldLength">Old length (or -1)</param>
        /// <param name="newLength">New length to be set</param>
        /// <param name="checkSize">True if sizes must be equal</param>
        /// <param name="type">Type of the variable</param>
        /// <returns>New length</returns>
        private int SetLength(int oldLength, int newLength, bool checkSize, Type type) {
            if(checkSize && oldLength != newLength && oldLength >= 0)
                throw new FunctionDefinitionException(string.Format(Messages.EMParametersDifferentLength, this.Name, type.FullName),
                    string.Format(Messages.EMParametersDifferentLengthDetail, oldLength, newLength));

            return System.Math.Max(oldLength, newLength);
        }

        /*
        /// <summary>
        /// Výpoèet výsledku operátoru
        /// </summary>
        /// <param name="left">Levá èást operátoru</param>
        /// <param name="right">Pravá èást operátoru</param>
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
            else if(left is TArray)
                return this.EvaluateA((TArray)left, right);
            else if(left is DateTime)
                return this.EvaluateTime((DateTime)left, right);
            else if(left is bool)
                return this.EvaluateB((bool)left, right);
            else if(left is List)
                return this.EvaluateL((List)left, right);
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
            else if(right is TArray)
                return this.EvaluateIA(left, (TArray)right);
            else if(right is List)
                return this.EvaluateIL(left, (List)right);
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
            else if(right is TArray)
                return this.EvaluateDA(left, (TArray)right);
            else if(right is List)
                return this.EvaluateOL(left, (List)right);
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
            else if(right is TArray)
                return this.EvaluatePA(left, (TArray)right);
            else if(right is List)
                return this.EvaluateOL(left, (List)right);
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
            else if(right is TArray)
                return this.EvaluateVA(left, (TArray)right);
            else if(right is List)
                return this.EvaluateOL(left, (List)right);
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
            else if(right is TArray)
                return this.EvaluatePvA(left, (TArray)right);
            else if(right is List)
                return this.EvaluateOL(left, (List)right);
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
            else if(right is TArray)
                return this.EvaluateMA(left, (TArray)right);
            else if(right is List)
                return this.EvaluateOL(left, (List)right);
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
            else if(right is TArray)
                return this.EvaluateSA(left, (TArray)right);
            else if(right is List)
                return this.EvaluateOL(left, (List)right);
            else
                return this.EvaluateSSx(left, right.ToString());
        }

        protected virtual object EvaluateB(bool left, object right) {
            if(right is bool)
                return this.EvaluateBB(left, (bool)right);
            else if(right is List)
                return this.EvaluateOL(left, (List)right);
            else
                return this.UnknownType(left, right);
        }

        protected virtual object EvaluateL(List left, object right) {
            if(right is int)
                return this.EvaluateLI(left, (int)right);
            else if(right is List)
                return this.EvaluateLL(left, (List)right);
            else if(right is TArray)
                return this.EvaluateLA(left, (TArray)right);
            else
                return this.EvaluateLO(left, right);
        }

        protected virtual object EvaluateTime(DateTime left, object right) {
            if(right is DateTime)
                return this.EvaluateTimeTimex(left, (DateTime)right);
            else
                return this.UnknownType(left, right);
        }

        protected virtual object EvaluateA(TArray left, object right) {
            TArray result = null;

            if(right is TArray) {
                TArray r = right as TArray;

                if(!TArray.IsEqualDimension(left, r))
                    throw new OperatorException(string.Format(Messages.EMOperatorArrayLength, this.OperatorName, left.LengthsString(), r.LengthsString()));

                left.ResetEnumerator();
                int[] index = (int[])left.StartEnumIndex.Clone();
                int[] lengths = left.Lengths;
                int rank = left.Rank;

                do {
                    object o = this.Evaluate(left[index], r[index]);
                    if(result == null)
                        result = new TArray(o.GetType(), lengths);

                    result[index] = o;
                }
                while(TArray.MoveNext(rank, index, left.StartEnumIndex, left.EndEnumIndex));
            }
            else {
                left.ResetEnumerator();
                int[] index = (int[])left.StartEnumIndex.Clone();
                int[] lengths = left.Lengths;
                int rank = left.Rank;

                do {
                    object o = this.Evaluate(left[index], right);
                    if(result == null)
                        result = new TArray(o.GetType(), lengths);

                    result[index] = o;
                }
                while(TArray.MoveNext(rank, index, left.StartEnumIndex, left.EndEnumIndex));
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

        protected virtual object EvaluateIA(int left, TArray right) {
            TArray result = null;

            right.ResetEnumerator();
            int[] index = (int[])right.StartEnumIndex.Clone();
            int[] lengths = right.Lengths;
            int rank = right.Rank;

            do {
                object o = this.EvaluateI(left, right[index]);
                if(result == null)
                    result = new TArray(o.GetType(), lengths);

                result[index] = o;
            }
            while(TArray.MoveNext(rank, index, right.StartEnumIndex, right.EndEnumIndex));

            return result;
        }

        protected virtual object EvaluateIL(int left, List right) {
            return this.EvaluateOL(left, right);
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

        protected virtual object EvaluateDA(double left, TArray right) {
            TArray result = null;

            right.ResetEnumerator();
            int[] index = (int[])right.StartEnumIndex.Clone();
            int[] lengths = right.Lengths;
            int rank = right.Rank;

            do {
                object o = this.EvaluateD(left, right[index]);
                if(result == null)
                    result = new TArray(o.GetType(), lengths);

                result[index] = o;
            }
            while(TArray.MoveNext(rank, index, right.StartEnumIndex, right.EndEnumIndex));

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

        protected virtual object EvaluatePA(PointD left, TArray right) {
            TArray result = null;

            right.ResetEnumerator();
            int[] index = (int[])right.StartEnumIndex.Clone();
            int[] lengths = right.Lengths;
            int rank = right.Rank;

            do {
                object o = this.EvaluateP(left, right[index]);
                if(result == null)
                    result = new TArray(o.GetType(), lengths);

                result[index] = o;
            }
            while(TArray.MoveNext(rank, index, right.StartEnumIndex, right.EndEnumIndex));

            return result;
        }

        protected virtual object EvaluateLI(List left, int right) {
            return this.EvaluateLO(left, right);
        }

        protected virtual object EvaluateLL(List left, List right) {
            return this.EvaluateLO(left, right);
        }

        protected virtual object EvaluateLO(List left, object right) {
            return this.UnknownType(left, right);
        }

        protected virtual object EvaluateOL(object left, List right) {
            return this.UnknownType(left, right);
        }

        protected virtual object EvaluateLA(List left, TArray right) {
            TArray result = null;

            right.ResetEnumerator();
            int[] index = (int[])right.StartEnumIndex.Clone();
            int[] lengths = right.Lengths;
            int rank = right.Rank;

            do {
                object o = this.EvaluateL(left, right[index]);
                if(result == null)
                    result = new TArray(o.GetType(), lengths);

                result[index] = o;
            }
            while(TArray.MoveNext(rank, index, right.StartEnumIndex, right.EndEnumIndex));

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

        protected virtual object EvaluateVA(Vector left, TArray right) {
            TArray result = null;

            right.ResetEnumerator();
            int[] index = (int[])right.StartEnumIndex.Clone();
            int[] lengths = right.Lengths;
            int rank = right.Rank;

            do {
                object o = this.EvaluateV(left, right[index]);
                if(result == null)
                    result = new TArray(o.GetType(), lengths);

                result[index] = o;
            }
            while(TArray.MoveNext(rank, index, right.StartEnumIndex, right.EndEnumIndex));

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

        protected virtual object EvaluatePvA(PointVector left, TArray right) {
            TArray result = null;

            right.ResetEnumerator();
            int[] index = (int[])right.StartEnumIndex.Clone();
            int[] lengths = right.Lengths;
            int rank = right.Rank;

            do {
                object o = this.EvaluatePv(left, right[index]);
                if(result == null)
                    result = new TArray(o.GetType(), lengths);

                result[index] = o;
            }
            while(TArray.MoveNext(rank, index, right.StartEnumIndex, right.EndEnumIndex));

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

        protected virtual object EvaluateMA(Matrix left, TArray right) {
            TArray result = null;

            right.ResetEnumerator();
            int[] index = (int[])right.StartEnumIndex.Clone();
            int[] lengths = right.Lengths;
            int rank = right.Rank;

            do {
                object o = this.EvaluateM(left, right[index]);
                if(result == null)
                    result = new TArray(o.GetType(), lengths);

                result[index] = o;
            }
            while(TArray.MoveNext(rank, index, right.StartEnumIndex, right.EndEnumIndex));

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

        protected virtual object EvaluateSA(string left, TArray right) {
            TArray result = null;

            right.ResetEnumerator();
            int[] index = (int[])right.StartEnumIndex.Clone();
            int[] lengths = right.Lengths;
            int rank = right.Rank;

            do {
                object o = this.EvaluateS(left, right[index]);
                if(result == null)
                    result = new TArray(o.GetType(), lengths);

                result[index] = o;
            }
            while(TArray.MoveNext(rank, index, right.StartEnumIndex, right.EndEnumIndex));

            return result;
        }

        protected virtual object EvaluateBB(bool left, bool right) {
            return this.UnknownType(left, right);
        }

        protected virtual object EvaluateTimeTimex(DateTime left, DateTime right) {
            return this.UnknownType(left, right);
        }
        #endregion

        /// <summary>
        /// Výjimka - neznámý typ pro výpoèet výrazu
        /// </summary>
        /// <param name="left">Levý výraz</param>
        /// <param name="right">Pravý výraz</param>
        protected object UnknownType(object left, object right) {
            throw new OperatorException(string.Format(Messages.EMOperatorUnknownType, this.OperatorName, left.GetType().FullName, right.GetType().FullName));
        }
        */
    }

	/// <summary>
	/// Výjimka ve tøídì Operator
	/// </summary>
	public class OperatorException: ApplicationException {
		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public OperatorException(string message) : base(errMessage + message) {}

		/// <summary>
		/// Konstruktor
		/// </summary>
		/// <param name="message">Text chybového hlášení</param>
		public OperatorException(string message, Exception innerException) : base(errMessage + message, innerException) {}

		private const string errMessage = "Ve tøídì Operator došlo k chybì: ";
	}
}

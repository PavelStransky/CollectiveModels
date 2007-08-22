using System;
using System.Collections;

using PavelStransky.Math;
using PavelStransky.Expression;

namespace PavelStransky.Expression.Functions {
    /// <summary>
    /// Priorita operátoru
    /// </summary>
    public enum OperatorPriority {
        SeparatorPriority = 0,
        AssignmentPriority = 1,
        BoolOrPriority = 2,
        BoolAndPriority = 3,
        ComparePriority = 4,
        JoinPriority = 5,
        IntervalPriority = 6,
        SumPriority = 7,
        ProductPriority = 8,
        PowerPriority = 9,
        MaxPriority = 10
    }
    
    /// <summary>
	/// Tøída pøedcházející všechny operátory
	/// </summary>
	public abstract class Operator: Fnc {
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

                else if(type == typeof(string)) {
                    foreach(object o in arguments)
                        if(o is string)
                            lx = this.SetLength(lx, (o as string).Length, checkSize, type);
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

        /// <summary>
        /// Exception - bad combination of types
        /// </summary>
        /// <param name="left">Levý výraz</param>
        /// <param name="right">Pravý výraz</param>
        protected object BadTypeCompatibility(object left, object right) {
            throw new FunctionDefinitionException(
                string.Format(Messages.EMParametersCompatibility, this.Name, left.GetType().FullName, right.GetType().FullName));
        }
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
